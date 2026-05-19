#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Game.Application;
using Game.Application.Contracts;
using Game.Domain;
using Game.Kernel.Utils.R3;

namespace Game.Application.Tests
{
    public sealed class SettingsDebouncedSaverTests
    {
        // -----------------------------------------------------------------------
        // Start
        // -----------------------------------------------------------------------

        [Test]
        public async Task Start_WhenSettingChanged_SavesAfterDelay()
        {
            var saveService = new FakeSaveService();

            var store = new Store<GameSettingState>(
                GameSettingState.Default);

            var saver = new SettingsDebouncedSaver(
                saveService,
                store);

            saver.Start();

            store.Dispatch(_ => new GameSettingState(
                bgmVolume: new AudioVolume(0.8f),
                seVolume: new AudioVolume(0.2f)));

            await UniTask.Delay(700);

            Assert.That(saveService.SavedSettings.Count, Is.EqualTo(1));

            var saved = saveService.SavedSettings[0];

            Assert.That(saved.BGMVolume, Is.EqualTo(0.8f));
            Assert.That(saved.SEVolume, Is.EqualTo(0.2f));

            saver.Dispose();
        }

        [Test]
        public async Task Start_MultipleChanges_OnlyLatestSaved()
        {
            var saveService = new FakeSaveService();

            var store = new Store<GameSettingState>(
                GameSettingState.Default);

            var saver = new SettingsDebouncedSaver(
                saveService,
                store);

            saver.Start();

            store.Dispatch(_ => new GameSettingState(
                bgmVolume: new AudioVolume(0.6f),
                seVolume: new AudioVolume(0.5f)));

            await UniTask.Delay(100);

            store.Dispatch(_ => new GameSettingState(
                bgmVolume: new AudioVolume(0.7f),
                seVolume: new AudioVolume(0.4f)));

            await UniTask.Delay(100);

            store.Dispatch(_ => new GameSettingState(
                bgmVolume: new AudioVolume(0.9f),
                seVolume: new AudioVolume(0.1f)));

            await UniTask.Delay(700);

            Assert.That(saveService.SavedSettings.Count, Is.EqualTo(1));

            var saved = saveService.SavedSettings[0];

            Assert.That(saved.BGMVolume, Is.EqualTo(0.9f));
            Assert.That(saved.SEVolume, Is.EqualTo(0.1f));

            saver.Dispose();
        }

        // -----------------------------------------------------------------------
        // FlushAsync
        // -----------------------------------------------------------------------

        [Test]
        public async Task FlushAsync_SavesImmediately()
        {
            var saveService = new FakeSaveService();

            var store = new Store<GameSettingState>(
                GameSettingState.Default);

            var saver = new SettingsDebouncedSaver(
                saveService,
                store);

            saver.Start();

            store.Dispatch(_ => new GameSettingState(
                bgmVolume: new AudioVolume(1.0f),
                seVolume: new AudioVolume(0.0f)));

            await saver.FlushAsync().AsTask();

            Assert.That(saveService.SavedSettings.Count, Is.EqualTo(1));

            var saved = saveService.SavedSettings[0];

            Assert.That(saved.BGMVolume, Is.EqualTo(1.0f));
            Assert.That(saved.SEVolume, Is.EqualTo(0.0f));

            saver.Dispose();
        }

        // -----------------------------------------------------------------------
        // Dispose
        // -----------------------------------------------------------------------

        [Test]
        public async Task Dispose_CancelsScheduledSave()
        {
            var saveService = new FakeSaveService();

            var store = new Store<GameSettingState>(
                GameSettingState.Default);

            var saver = new SettingsDebouncedSaver(
                saveService,
                store);

            saver.Start();

            store.Dispatch(_ => new GameSettingState(
                bgmVolume: new AudioVolume(0.9f),
                seVolume: new AudioVolume(0.9f)));

            saver.Dispose();

            await UniTask.Delay(700);

            Assert.That(saveService.SavedSettings.Count, Is.EqualTo(0));
        }

        // -----------------------------------------------------------------------
        // Fake
        // -----------------------------------------------------------------------

        private sealed class FakeSaveService : ISaveService
        {
            public readonly List<SettingSaveData> SavedSettings = new();

            public UniTask SaveGameDataAsync(
                GameSaveData saveData,
                CancellationToken ct = default)
            {
                return UniTask.CompletedTask;
            }

            public UniTask SaveSettingsDataAsync(
                SettingSaveData saveData,
                CancellationToken ct = default)
            {
                SavedSettings.Add(saveData);
                return UniTask.CompletedTask;
            }

            public UniTask<GameSaveData> LoadGameDataAsync(
                CancellationToken ct = default)
            {
                return UniTask.FromResult(GameSaveData.Default());
            }

            public UniTask<SettingSaveData> LoadSettingsDataAsync(
                CancellationToken ct = default)
            {
                return UniTask.FromResult(SettingSaveData.Default());
            }

            public UniTask InitializeAsync(
                CancellationToken ct = default)
            {
                return UniTask.CompletedTask;
            }
        }
    }
}