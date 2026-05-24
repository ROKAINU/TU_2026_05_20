using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;
using Game;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel;

namespace Game.Infrastructure.Save
{
    public class PlayerPrefsSaveService : ISaveService
    {
        #region Key of GameSaveDatas
        private const string RootKey = "GameSaveData";

        // PlayData
        private const string TestScoreKey = "TestScore";
        private const string TestHighScoreKey = "TestHighScore";
        // Setting
        private const string BGMVolumeKey = "BGMVolume";
        private const string SEVolumeKey = "SEVolume";

        // Version
        private const string VersionMajorKey = "Version_Major";
        private const string VersionMinorKey = "Version_Minor";
        private const string VersionPatchKey = "Version_Patch";

        private static string Ppkey(string key) => $"{RootKey}.{key}";
        #endregion

        private readonly LoggerBase _logger;
        private readonly SaveMigrator _saveMigrator;

        public PlayerPrefsSaveService(LoggerBase logger, SaveMigrator saveMigrator)
        {
            _logger = logger;
            _saveMigrator = saveMigrator;
        }

        public Task SaveGameDataAsync(GameSaveData saveData, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return Task.FromCanceled(ct);

            PlayerPrefs.SetInt(Ppkey(VersionMajorKey), GameVersion.Major);
            PlayerPrefs.SetInt(Ppkey(VersionMinorKey), GameVersion.Minor);
            PlayerPrefs.SetInt(Ppkey(VersionPatchKey), GameVersion.Patch);
            PlayerPrefs.SetInt(Ppkey(TestScoreKey), saveData.TestScore);
            PlayerPrefs.SetInt(Ppkey(TestHighScoreKey), saveData.TestHighScore);
            PlayerPrefs.Save();

            return Task.CompletedTask;
        }

        public async Task<GameSaveData> LoadGameDataAsync(CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) throw new OperationCanceledException(ct);
            
            try
            {
                // ── バージョンチェック ────────────────────────
                // 保存済みMajorバージョンを読む（未保存なら-1）
                int savedMajor = PlayerPrefs.GetInt(Ppkey(VersionMajorKey), -1);

                if (savedMajor == -1)
                {
                    // 初回起動：保存データなし → 初期化して返す
                    _logger.LogInfo("[PlayerPrefsSaveService]No save data found. Initializing.");
                    await InitializeAsync(ct);
                    return GameSaveData.Default();
                }

                // ── 通常ロード ────────────────────────────────
                var def = GameSaveData.Default();

                var rawGame  = new GameSaveData(
                    testScore:     PlayerPrefs.GetInt(Ppkey(TestScoreKey),     def.TestScore),
                    testHighScore: PlayerPrefs.GetInt(Ppkey(TestHighScoreKey), def.TestHighScore)
                );

                var rawSettings = await LoadSettingsDataAsync(ct);
 
                // マイグレーション適用
                var (result, migratedGame, migratedSettings) =
                    _saveMigrator.Migrate(savedMajor, rawGame, rawSettings);
 
                switch (result)
                {
                    case SaveMigrator.MigrationResult.Success:
                        // マイグレーション後のデータを保存して返す
                        await SaveGameDataAsync(migratedGame, ct);
                        await SaveSettingsDataAsync(migratedSettings, ct);
                        return migratedGame;
 
                    case SaveMigrator.MigrationResult.Downgrade:
                        // ダウングレードは対応不可 → リセット
                        _logger.LogError("[PlayerPrefsSaveService] Downgrade detected. Resetting save data.");
                        await InitializeAsync(ct);
                        return GameSaveData.Default();
 
                    case SaveMigrator.MigrationResult.AlreadyLatest:
                    default:
                        return rawGame;
                }
            }

            catch (Exception ex)
            {
                _logger.LogException(ex, "An error occurred while loading game data. Returning default values.");
                await InitializeAsync(ct);
                return GameSaveData.Default();
            }
        }

        public Task SaveSettingsDataAsync(SettingSaveData saveData, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return Task.FromCanceled(ct);

            PlayerPrefs.SetFloat(Ppkey(BGMVolumeKey), saveData.BGMVolume);
            PlayerPrefs.SetFloat(Ppkey(SEVolumeKey), saveData.SEVolume);
            PlayerPrefs.Save();

            return Task.CompletedTask;
        }

        public Task<SettingSaveData> LoadSettingsDataAsync(CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return Task.FromCanceled<SettingSaveData>(ct);

            try
            {
                var def = SettingSaveData.Default();

                var data = new SettingSaveData(
                    bgmVolume: PlayerPrefs.GetFloat(Ppkey(BGMVolumeKey), def.BGMVolume),
                    seVolume: PlayerPrefs.GetFloat(Ppkey(SEVolumeKey), def.SEVolume)
                );

                return Task.FromResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "An error occurred while loading settings data. Returning default values.");
                return Task.FromResult(SettingSaveData.Default());
            }
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            await SaveGameDataAsync(GameSaveData.Default(), ct);
            await SaveSettingsDataAsync(SettingSaveData.Default(), ct);
        }
    }
}