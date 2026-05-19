#nullable enable
using System.Collections.Generic;
using System.Linq;
using Game.Domain;
using Game.Kernel;

namespace Game.Infrastructure.Save
{
    /// <summary>
    /// セーブデータのマイグレーションを管理するクラス。
    /// [設計意図] 複数バージョンをまたぐ場合も順番に適用することで
    ///            v1→v3のような飛び越しマイグレーションに対応する。
    ///            例：v1→v2→v3 と順番に適用される。
    /// </summary>
    public sealed class SaveMigrator
    {
        private readonly IReadOnlyList<ISaveMigration> _migrations;
        private readonly LoggerBase _logger;

        /// <summary>
        /// [拡張方法] 新しいバージョンのマイグレーションを
        ///            migrationsリストに追加するだけでいい。
        ///            FromMajorの昇順に並べること。
        /// </summary>
        public SaveMigrator(LoggerBase logger)
        {
            _logger = logger;
            _migrations = new List<ISaveMigration>
            {
                new Migration_1_to_2(),
                // new Migration_2_to_3(), ← 次のバージョンアップ時に追加
            }
            .OrderBy(m => m.FromMajor)
            .ToList();
        }

        public enum MigrationResult { Success, Downgrade, AlreadyLatest }

        /// <summary>
        /// savedMajorから現在のバージョンまで順番にマイグレーションを適用する。
        /// </summary>
        public (MigrationResult result, GameSaveData gameData, SettingSaveData settingsData) Migrate(
            int savedMajor,
            GameSaveData gameData,
            SettingSaveData settingsData)
        {
            int currentMajor = GameVersion.Major;

            // すでに最新バージョン
            if (savedMajor == currentMajor)
                return (MigrationResult.AlreadyLatest, gameData, settingsData);

            // ダウングレード（保存データの方が新しい）は対応不可
            if (savedMajor > currentMajor)
            {
                _logger.LogError($"[PlayerPrefsSaveService] Downgrade detected. " +
                            $"Saved={savedMajor}, Current={currentMajor}. Cannot migrate.");
                return (MigrationResult.Downgrade, gameData, settingsData);
            }

            // savedMajor → currentMajor まで順番に適用
            var currentGameData = gameData;
            var currentSettingsData = settingsData;

            foreach (var migration in _migrations.Where(m => m.FromMajor >= savedMajor
                                                          && m.FromMajor < currentMajor))
            {
                _logger.LogInfo($"[SaveMigrator] Applying migration v{migration.FromMajor} → v{migration.FromMajor + 1}");
                currentGameData     = migration.MigrateGameData(currentGameData);
                currentSettingsData = migration.MigrateSettingsData(currentSettingsData);
            }

            _logger.LogInfo($"[SaveMigrator] Migration complete. v{savedMajor} → v{currentMajor}");
            return (MigrationResult.Success, currentGameData, currentSettingsData);
        }
    }
}