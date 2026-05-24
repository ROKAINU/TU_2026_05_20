// Unity標準のEditorWindowで実装。
// メニュー: Tools > Excel To Json
// ToolbarExtenderを使いたい場合は下部コメントを参照。

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Tools
{
    public class ExcelToJsonEditorWindow : EditorWindow
    {
        // ---- 設定の永続化キー ----
        private const string PREF_EXCEL_PATH  = "ExcelToJson_ExcelPath";
        private const string PREF_OUTPUT_PATH = "ExcelToJson_OutputPath";

        private string _excelPath  = "";
        private string _outputPath = "Assets/Resources/MasterData";

        private Vector2 _scroll;
        private ConvertResult _lastResult;

        // ---- メニュー登録 ----
        [MenuItem("Tools/Excel To Json")]
        public static void Open()
        {
            var window = GetWindow<ExcelToJsonEditorWindow>("Excel → JSON");
            window.minSize = new Vector2(500, 400);
            window.Show();
        }

        // ---- ライフサイクル ----

        private void OnEnable()
        {
            _excelPath  = EditorPrefs.GetString(PREF_EXCEL_PATH,  "");
            _outputPath = EditorPrefs.GetString(PREF_OUTPUT_PATH, "Assets/Resources/MasterData");
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(PREF_EXCEL_PATH,  _excelPath);
            EditorPrefs.SetString(PREF_OUTPUT_PATH, _outputPath);
        }

        // ---- GUI ----

        private void OnGUI()
        {
            DrawHeader();
            EditorGUILayout.Space(8);
            DrawPathSettings();
            EditorGUILayout.Space(8);
            DrawConvertButton();
            EditorGUILayout.Space(8);
            DrawResult();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Excel → JSON コンバーター", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Excelのマスターデータを JsonファイルへDします。", EditorStyles.miniLabel);
            EditorGUILayout.LabelField(
                "フォーマット  行1:フィールド名 / 行2:型 / 行3:コメント / 行4〜:データ",
                EditorStyles.miniLabel);
        }

        private void DrawPathSettings()
        {
            EditorGUILayout.LabelField("設定", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                _excelPath = EditorGUILayout.TextField("Excelファイル", _excelPath);
                if (GUILayout.Button("参照", GUILayout.Width(50)))
                {
                    var path = EditorUtility.OpenFilePanel("Excelファイルを選択", "", "xlsx,xls");
                    if (!string.IsNullOrEmpty(path)) _excelPath = path;
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                _outputPath = EditorGUILayout.TextField("出力先フォルダ", _outputPath);
                if (GUILayout.Button("参照", GUILayout.Width(50)))
                {
                    var path = EditorUtility.OpenFolderPanel("出力先を選択", "Assets", "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        // Assetsからの相対パスに変換
                        _outputPath = AbsoluteToAssetPath(path);
                    }
                }
            }
        }

        private void DrawConvertButton()
        {
            var canConvert = !string.IsNullOrEmpty(_excelPath) && File.Exists(_excelPath);

            using (new EditorGUI.DisabledGroupScope(!canConvert))
            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    fontSize  = 13,
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 36
                };

                if (GUILayout.Button("変換実行", style))
                    RunConvert();
            }

            if (!canConvert && !string.IsNullOrEmpty(_excelPath))
                EditorGUILayout.HelpBox("Excelファイルが見つかりません。", MessageType.Warning);
        }

        private void DrawResult()
        {
            if (_lastResult == null) return;

            EditorGUILayout.LabelField("変換結果", EditorStyles.boldLabel);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));

            // エラー表示
            foreach (var err in _lastResult.Errors)
                EditorGUILayout.HelpBox(err, MessageType.Error);

            // シートごとの結果
            foreach (var sheet in _lastResult.SheetResults)
            {
                var icon    = sheet.Success ? "✅" : "❌";
                var message = $"{icon} [{sheet.SheetName}]  {sheet.Message}";
                var style   = sheet.Success ? EditorStyles.label : EditorStyles.boldLabel;
                EditorGUILayout.LabelField(message, style);
            }

            EditorGUILayout.EndScrollView();

            // 出力先を開くボタン
            if (!_lastResult.HasError)
            {
                EditorGUILayout.Space(4);
                if (GUILayout.Button("出力フォルダを開く"))
                    EditorUtility.RevealInFinder(_outputPath);
            }
        }

        // ---- 変換処理 ----

        private void RunConvert()
        {
            var absoluteOutput = AssetPathToAbsolute(_outputPath);

            EditorUtility.DisplayProgressBar("Excel → JSON", "変換中...", 0.5f);
            try
            {
                _lastResult = ExcelToJsonConverter.Convert(_excelPath, absoluteOutput);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            // Unityにファイル変更を通知
            AssetDatabase.Refresh();

            if (_lastResult.HasError)
                Debug.LogError("[ExcelToJson] 変換中にエラーが発生しました。");
            else
                Debug.Log($"[ExcelToJson] 変換完了。出力先: {_outputPath}");

            Repaint();
        }

        // ---- パス変換ユーティリティ ----

        private static string AbsoluteToAssetPath(string absolutePath)
        {
            var projectPath = Path.GetFullPath(Application.dataPath + "/..");
            absolutePath    = Path.GetFullPath(absolutePath);
            if (absolutePath.StartsWith(projectPath))
                return absolutePath.Substring(projectPath.Length + 1).Replace("\\", "/");
            return absolutePath;
        }

        private static string AssetPathToAbsolute(string assetPath)
        {
            if (Path.IsPathRooted(assetPath)) return assetPath;
            return Path.GetFullPath(Path.Combine(Application.dataPath + "/..", assetPath));
        }
    }
}


// ===================================================================
// ToolbarExtender でツールバーに追加したい場合はこちら
// ===================================================================
// 依存: https://github.com/marijnz/unity-toolbar-extender
//
// using UnityToolbarExtender;
//
// [InitializeOnLoad]
// public static class ExcelToJsonToolbarButton
// {
//     static ExcelToJsonToolbarButton()
//     {
//         ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
//     }
//
//     private static void OnToolbarGUI()
//     {
//         GUILayout.FlexibleSpace();
//         if (GUILayout.Button(new GUIContent("XLS→JSON", "Excelをマスターデータに変換"),
//             EditorStyles.toolbarButton, GUILayout.Width(80)))
//         {
//             ExcelToJsonEditorWindow.Open();
//         }
//     }
// }