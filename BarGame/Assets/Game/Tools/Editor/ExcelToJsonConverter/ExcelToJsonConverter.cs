// 依存: ExcelDataReader (NuGet or UnityPackage)
//   → Packages/manifest.json に追加不要。DLLをAssets/Plugins/Editor/に配置する方式を推奨
//   → https://github.com/ExcelDataReader/ExcelDataReader

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEditor;
using UnityEngine;

namespace Game.Tools
{
    /// <summary>
    /// Excelファイルを読み込み、JSON形式のマスターデータを生成するコンバーター
    ///
    /// ■ Excelフォーマット仕様
    ///   行1: フィールド名     (例: id, name, hp, attack)
    ///   行2: 型指定          (例: int, string, float, bool)
    ///   行3: コメント        (例: ID, キャラ名, HP, 攻撃力) ← ゲーム側では無視される
    ///   行4以降: データ本体
    /// 
    /// │ id  │ name     │ level │ hp  │ attack │ skills        │ active │
    /// ├─────┼──────────┼───────┼─────┼────────┼───────────────┼────────┤
    /// │ int │ string   │ int   │ int │ float  │ string[]      │ bool   │
    /// ├─────┼──────────┼───────┼─────┼────────┼───────────────┼────────┤
    /// │ ID  │ 名前     │ レベル │ HP  │ 攻撃力 │ スキル         │ 有効   │
    /// ├─────┼──────────┼───────┼─────┼────────┼───────────────┼────────┤
    /// │ 1   │ Taro     │ 10    │ 100 │ 15.5   │ Fire,Ice      │ true   │
    /// │ 2   │ Hanako   │ 15    │ 150 │ 20.3   │ Heal,Holy,Buf │ true   │
    /// │ 3   │ Jiro     │ 12    │ 120 │ 18.0   │ Dark          │ false  │
    ///
    /// ■ シート名ルール
    ///   通常シート: そのままJSONファイル名になる (例: CharacterMaster → CharacterMaster.json)
    ///   "#" 始まり: スキップ (例: #memo → 出力しない)
    /// 
    /// シート名: ItemMaster
    /// │ item_id │ item_name  │ price │ #internal_id │
    /// ├─────────┼────────────┼───────┼──────────────┤
    /// │ int     │ string     │ int   │ int          │
    /// ├─────────┼────────────┼───────┼──────────────┤
    /// │ 1       │ Sword      │ 100   │ 999          │
    /// │ 2       │ Shield     │ 150   │ 1000         │
    /// 
    /// シート名: #memo （← "#" で始まるのでスキップ）
    /// │ memo        │
    /// ├─────────────┤
    /// │スキップされる│
    /// 
    /// ItemMaster.json が生成（#internal_id フィールドは除外）
    /// #memo シートは出力されない
    ///
    /// ■ フィールド名ルール
    ///   通常フィールド: そのままJSONキーになる
    ///   "#" 始まり: スキップ (例: #comment → 出力しない)
    ///
    /// ■ 対応型
    ///   int, long, float, double, bool, string
    ///   int[], float[], string[] などの配列にも対応（カンマ区切りで配列化）
    /// </summary>
    public static class ExcelToJsonConverter
    {
        // ---- 設定 ----
        private const int ROW_FIELD_NAME = 0;  // フィールド名行（0始まり）
        private const int ROW_TYPE       = 1;  // 型行
        private const int ROW_COMMENT    = 2;  // コメント行（出力しない）
        private const int ROW_DATA_START = 3;  // データ開始行

        // ---- 公開API ----

        /// <summary>
        /// 指定したExcelファイルを変換してJSONを出力する
        /// </summary>
        public static ConvertResult Convert(string excelPath, string outputDirectory)
        {
            var result = new ConvertResult { ExcelPath = excelPath };

            if (!File.Exists(excelPath))
            {
                result.AddError($"Excelファイルが見つかりません: {excelPath}");
                return result;
            }

            Directory.CreateDirectory(outputDirectory);

            try
            {
                // ExcelDataReader はシステムテキストエンコーディングが必要
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = CreateReader(excelPath, stream);

                if (reader == null)
                {
                    result.AddError($"未対応の拡張子です。.xlsx または .xls のみ対応: {excelPath}");
                    return result;
                }

                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = false }
                });

                foreach (DataTable sheet in dataSet.Tables)
                {
                    // "#" 始まりのシートはスキップ
                    if (sheet.TableName.StartsWith("#")) continue;

                    var sheetResult = ConvertSheet(sheet, outputDirectory);
                    result.SheetResults.Add(sheetResult);

                    if (!sheetResult.Success)
                        result.HasError = true;
                }
            }
            catch (Exception e)
            {
                result.AddError($"変換中に例外が発生しました: {e.Message}\n{e.StackTrace}");
            }

            return result;
        }

        // ---- 内部処理 ----

        private static IExcelDataReader CreateReader(string path, Stream stream)
        {
            var ext = Path.GetExtension(path).ToLower();
            return ext switch
            {
                ".xlsx" => ExcelReaderFactory.CreateOpenXmlReader(stream),
                ".xls"  => ExcelReaderFactory.CreateBinaryReader(stream),
                _       => null
            };
        }

        private static SheetConvertResult ConvertSheet(DataTable sheet, string outputDirectory)
        {
            var result = new SheetConvertResult { SheetName = sheet.TableName };

            if (sheet.Rows.Count <= ROW_DATA_START)
            {
                result.Message = "データ行がありません（スキップ）";
                result.Success = true;
                return result;
            }

            // フィールド名と型を取得
            var fieldNames = GetRowValues(sheet, ROW_FIELD_NAME);
            var fieldTypes = GetRowValues(sheet, ROW_TYPE);

            // フィールドのスキップ判定リスト
            var skipColumns = new bool[fieldNames.Count];
            for (int i = 0; i < fieldNames.Count; i++)
                skipColumns[i] = string.IsNullOrEmpty(fieldNames[i]) || fieldNames[i].StartsWith("#");

            // データ行をJSONに変換
            var records = new List<string>();
            for (int rowIdx = ROW_DATA_START; rowIdx < sheet.Rows.Count; rowIdx++)
            {
                var row = sheet.Rows[rowIdx];

                // 全カラムが空の行はスキップ
                bool isEmpty = true;
                foreach (var cell in row.ItemArray)
                    if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString())) { isEmpty = false; break; }
                if (isEmpty) continue;

                var fields = new List<string>();
                for (int colIdx = 0; colIdx < fieldNames.Count; colIdx++)
                {
                    if (skipColumns[colIdx]) continue;

                    var fieldName = fieldNames[colIdx];
                    var typeName  = colIdx < fieldTypes.Count ? fieldTypes[colIdx] : "string";
                    var rawValue  = colIdx < row.ItemArray.Length ? row[colIdx]?.ToString() ?? "" : "";

                    var jsonValue = ConvertValue(rawValue, typeName);
                    fields.Add($"    \"{EscapeJson(fieldName)}\": {jsonValue}");
                }

                records.Add("  {\n" + string.Join(",\n", fields) + "\n  }");
            }

            var json = "[\n" + string.Join(",\n", records) + "\n]";

            // 出力
            var outputPath = Path.Combine(outputDirectory, sheet.TableName + ".json");
            File.WriteAllText(outputPath, json, new UTF8Encoding(false));

            result.Success     = true;
            result.OutputPath  = outputPath;
            result.RecordCount = records.Count;
            result.Message     = $"{records.Count} 件を出力: {outputPath}";
            return result;
        }

        private static string ConvertValue(string raw, string typeName)
        {
            typeName = typeName.Trim().ToLower();

            bool isArray = typeName.EndsWith("[]");
            string baseType = isArray ? typeName.Substring(0, typeName.Length - 2) : typeName;

            if (isArray)
                return ToJsonArray(raw, baseType);

            return typeName switch
            {
                "int"      => TryParseInt(raw),
                "long"     => TryParseLong(raw),
                "float"    => TryParseFloat(raw),
                "double"   => TryParseDouble(raw),
                "bool"     => TryParseBool(raw),
                "string"   => $"\"{EscapeJson(raw)}\"",
                _          => $"\"{EscapeJson(raw)}\""  // デフォルトはstring
            };
        }

        private static string TryParseInt(string raw)
            => int.TryParse(raw, out var v) ? v.ToString() : "0";

        private static string TryParseLong(string raw)
            => long.TryParse(raw, out var v) ? v.ToString() : "0";

        private static string TryParseFloat(string raw)
            => float.TryParse(raw, out var v) ? v.ToString("G") : "0.0";

        private static string TryParseDouble(string raw)
            => double.TryParse(raw, out var v) ? v.ToString("G") : "0.0";

        private static string TryParseBool(string raw)
        {
            raw = raw.Trim().ToLower();
            return (raw == "true" || raw == "1" || raw == "yes") ? "true" : "false";
        }

        private static string ToJsonArray(string raw, string elementType)
        {
            if (string.IsNullOrWhiteSpace(raw)) 
                return "[]";
            
            var parts = raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var elements = new List<string>();
            
            foreach (var part in parts)
                elements.Add(ConvertValue(part.Trim(), elementType));
            
            return "[" + string.Join(", ", elements) + "]";
        }

        private static List<string> GetRowValues(DataTable sheet, int rowIndex)
        {
            var result = new List<string>();
            if (rowIndex >= sheet.Rows.Count) return result;
            foreach (var item in sheet.Rows[rowIndex].ItemArray)
                result.Add(item?.ToString() ?? "");
            return result;
        }

        private static string EscapeJson(string s)
            => s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
    }

    // ---- 結果クラス ----

    public class ConvertResult
    {
        public string ExcelPath { get; set; }
        public bool HasError { get; set; }
        public List<SheetConvertResult> SheetResults { get; } = new();
        public List<string> Errors { get; } = new();

        public void AddError(string msg) { Errors.Add(msg); HasError = true; }
    }

    public class SheetConvertResult
    {
        public string SheetName   { get; set; }
        public bool   Success     { get; set; }
        public string OutputPath  { get; set; }
        public string Message     { get; set; }
        public int    RecordCount { get; set; }
    }
}