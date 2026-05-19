#nullable enable
using Game.Domain;

namespace Game.Infrastructure.Save
{
    /// <summary>
    /// v1 → v2 のマイグレーション実装例。
    /// [拡張方法] バージョンが上がるたびにこのクラスをコピーして
    ///            FromMajorと変換処理を書き換える。
    /// </summary>
    public sealed class Migration_1_to_2 : ISaveMigration
    {
        public int FromMajor => 1;
 
        public GameSaveData MigrateGameData(GameSaveData data)
        {
            // 例：v2でTestScoreの上限が変わった場合
            // var clampedScore = Math.Min(data.TestScore, NewMaxScore);
            // return new GameSaveData(clampedScore, data.TestHighScore);
 
            // 変換不要な場合はそのまま返す
            return data;
        }
 
        public SettingSaveData MigrateSettingsData(SettingSaveData data)
        {
            // 設定データの変換が不要ならそのまま返す
            return data;
        }
    }
}