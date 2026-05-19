#nullable enable
using Game.Domain;
 
namespace Game.Infrastructure.Save
{
    /// <summary>
    /// セーブデータのバージョン間マイグレーションを定義するインターフェース。
    /// [設計意図] バージョンアップごとに1クラス1責務で変換処理を分離する。
    ///            新しいバージョンへの移行はこのインターフェースを実装して追加するだけでいい。
    /// </summary>
    public interface ISaveMigration
    {
        /// <summary>このマイグレーションが対応するFromバージョン（Major）</summary>
        int FromMajor { get; }
 
        /// <summary>GameSaveDataのマイグレーション</summary>
        GameSaveData MigrateGameData(GameSaveData data);
 
        /// <summary>SettingSaveDataのマイグレーション</summary>
        SettingSaveData MigrateSettingsData(SettingSaveData data);
    }
}