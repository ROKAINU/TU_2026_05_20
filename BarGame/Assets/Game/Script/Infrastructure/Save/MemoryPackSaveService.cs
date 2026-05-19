#nullable enable
using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using MemoryPack;
using UnityEngine;
using Game.Application.Contracts;
using Game.Domain;
using Game.Kernel;

namespace Game.Infrastructure.Save
{
    public class MemoryPackSaveService : ISaveService
    {
        private readonly LoggerBase _logger;
        private readonly SaveMigrator _saveMigrator;
        private readonly string _savePath;
        private SaveFileRoot? _cache;
        private readonly SemaphoreSlim _writeLock = new(1, 1);

        public MemoryPackSaveService(
            LoggerBase logger,
            SaveMigrator saveMigrator)
        {
            _logger = logger;
            _saveMigrator = saveMigrator;
            _savePath = Path.Combine(UnityEngine.Application.persistentDataPath, "save.dat");
        }

        private async UniTask<SaveFileRoot> GetOrLoadRootAsync(CancellationToken ct)
        {
            if (_cache != null) return _cache;

            // ロード結果を一時変数に受けて、二重呼び出しでも既存キャッシュを優先する
            var loaded = await LoadRootAsync(ct);
            _cache ??= loaded;
            return _cache;
        }

        public async UniTask SaveGameDataAsync(
            GameSaveData saveData,
            CancellationToken ct = default)
        {
            var root = await GetOrLoadRootAsync(ct);
            root.GameData = saveData.ToSave();
            root.Version = SaveHub.CurrentVersionToSave();
            await SaveRootAsync(root, ct);
        }

        public async UniTask<GameSaveData> LoadGameDataAsync(CancellationToken ct = default)
        {
            var root = await GetOrLoadRootAsync(ct);
            return root.GameData.ToGame();
        }

        public async UniTask SaveSettingsDataAsync(
            SettingSaveData saveData,
            CancellationToken ct = default)
        {
            var root = await GetOrLoadRootAsync(ct);
            root.SettingData = saveData.ToSave();
            root.Version = SaveHub.CurrentVersionToSave();
            await SaveRootAsync(root, ct);
        }

        public async UniTask<SettingSaveData> LoadSettingsDataAsync(CancellationToken ct = default)
        {
            var root = await GetOrLoadRootAsync(ct);
            return root.SettingData.ToGame();
        }

        public async UniTask InitializeAsync(CancellationToken ct = default)
        {
            _cache = null;
            var root = new SaveFileRoot();
            await SaveRootAsync(root, ct);
        }

        private async UniTask SaveRootAsync(
            SaveFileRoot root,
            CancellationToken ct)
        {
            await _writeLock.WaitAsync(ct);
            try
            {
                byte[] bytes = MemoryPackSerializer.Serialize(root);
                await File.WriteAllBytesAsync(_savePath, bytes, ct);
                _cache = root;  // 問題3: 書き込み成功後にキャッシュを更新
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "[MemoryPackSaveService] Save failed.");
                throw;
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private async UniTask<SaveFileRoot> LoadRootAsync(CancellationToken ct)
        {
            if (!File.Exists(_savePath))
            {
                _logger.LogInfo("[MemoryPackSaveService] No save file found. Creating default.");
                var def = new SaveFileRoot();
                await SaveRootAsync(def, ct);
                return def;
            }

            try
            {
                byte[] bytes = await File.ReadAllBytesAsync(_savePath, ct);
                var root = MemoryPackSerializer.Deserialize<SaveFileRoot>(bytes);
                if (root == null)
                    throw new Exception("Save deserialize failed.");
                return root;
            }
            catch (Exception ex)
            {
                // 問題1: 壊れたファイルをデフォルトで上書きして次回起動時の失敗を防ぐ
                _logger.LogException(ex, "[MemoryPackSaveService] Load failed. Overwriting with default.");
                var def = new SaveFileRoot();
                await SaveRootAsync(def, ct);
                return def;
            }
        }
    }
}