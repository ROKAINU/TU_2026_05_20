using System.Collections.Generic;
using UnityEngine;

// 生成されたJSONを読み込んでC#オブジェクトに変換するランタイム側のサンプル
namespace Game.Infrastructure
{
    public static class MasterDataLoader
    {
        private const string RESOURCES_BASE = "MasterData";

        /// <summary>
        /// Resources/MasterData/{fileName}.json を読み込む
        /// </summary>
        public static List<T> Load<T>(string fileName)
        {
            var path     = $"{RESOURCES_BASE}/{fileName}";
            var textAsset = Resources.Load<TextAsset>(path);

            if (textAsset == null)
            {
                Debug.LogError($"[MasterDataLoader] JSONが見つかりません: {path}");
                return new List<T>();
            }

            // JsonUtilityはルート配列を直接パースできないのでラッパーを使う
            var json     = $"{{\"items\":{textAsset.text}}}";
            var wrapper  = JsonUtility.FromJson<JsonWrapper<T>>(json);
            return wrapper.items;
        }

        [System.Serializable]
        private class JsonWrapper<T>
        {
            public List<T> items;
        }
    }

    // ---- 使用例 ----
    // var characters = MasterDataLoader.Load<CharacterMasterRecord>("CharacterMaster");
    // var items      = MasterDataLoader.Load<ItemMasterRecord>("ItemMaster");
}


// ===================================================================
// ■ Excelフォーマットサンプル（CharacterMaster シート）
// ===================================================================
//
// | id     | name   | hp     | attack | speed  | isPlayable | #memo    |
// | int    | string | int    | int    | float  | bool       | string   |
// | ID     | 名前   | HP     | 攻撃力  | 速度   | プレイアブル| メモ      |
// | 1      | 勇者   | 100    | 25     | 1.2    | true       | 主人公    |
// | 2      | 魔法使い| 60    | 50     | 1.0    | true       | 後衛      |
// | 3      | ドラゴン| 500   | 80     | 0.8    | false      | ボス      |
//
// → CharacterMaster.json が生成される
// [
//   { "id": 1, "name": "勇者", "hp": 100, "attack": 25, "speed": 1.2, "isPlayable": true },
//   { "id": 2, "name": "魔法使い", "hp": 60, "attack": 50, "speed": 1.0, "isPlayable": true },
//   { "id": 3, "name": "ドラゴン", "hp": 500, "attack": 80, "speed": 0.8, "isPlayable": false }
// ]
//
// ■ スキップ機能
//   列名を "#" で始めると出力しない (例: #memo, #debug)
//   シート名を "#" で始めると出力しない (例: #作業メモ, #旧データ)
//
// ■ 配列型サンプル（ItemMaster シート）
//
// | id  | name   | dropStageIds | tags      |
// | int | string | int[]        | string[]  |
// | ID  | 名前   | ドロップ場所 | タグ      |
// | 1   | 剣     | 1,3,5        | weapon,attack |
//
// → { "id": 1, "name": "剣", "dropStageIds": [1, 3, 5], "tags": ["weapon", "attack"] }