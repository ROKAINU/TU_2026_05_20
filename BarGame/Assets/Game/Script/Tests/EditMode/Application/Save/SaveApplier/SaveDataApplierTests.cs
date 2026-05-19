#nullable enable
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Cysharp.Threading.Tasks;
using R3;
using Game.Domain;
using Game.Application;
using Game.Kernel.Utils.R3;

namespace Game.Application.Tests
{
    public sealed class SaveDataApplierTests
    {
        // -----------------------------------------------------------------------
        // ApplyGameDataToStoresAsync
        // -----------------------------------------------------------------------

        [Test]
        public async Task ApplyGameDataToStoresAsync_UpdatesHighScore()
        {
            var globalStore = new Store<GameGlobalState>(
                new GameGlobalState(
                    highScore: new Score(10),
                    currentScore: new Score(5)));

            var settingStore = new Store<GameSettingState>(
                new GameSettingState(
                    bgmVolume: new AudioVolume(0.5f),
                    seVolume: new AudioVolume(0.5f)));

            var applier = new SaveDataApplier(
                globalStore,
                settingStore);

            var saveData = new GameSaveData(
                testScore: 100,
                testHighScore: 999);

            await applier.ApplyGameDataToStoresAsync(saveData).AsTask();

            var global = globalStore.State.CurrentValue;

            Assert.That(global.HighScore.Value, Is.EqualTo(999));
            Assert.That(global.CurrentScore.Value, Is.EqualTo(5));
        }

        [Test]
        public async Task ApplyGameDataToStoresAsync_DoesNotChangeSettings()
        {
            var globalStore = new Store<GameGlobalState>(
                GameGlobalState.Default);

            var settingStore = new Store<GameSettingState>(
                new GameSettingState(
                    bgmVolume: new AudioVolume(0.8f),
                    seVolume: new AudioVolume(0.2f)));

            var applier = new SaveDataApplier(
                globalStore,
                settingStore);

            await applier.ApplyGameDataToStoresAsync(GameSaveData.Default()).AsTask();

            var setting = settingStore.State.CurrentValue;

            Assert.That(setting.BGMVolume.Value, Is.EqualTo(0.8f));
            Assert.That(setting.SEVolume.Value, Is.EqualTo(0.2f));
        }

        // -----------------------------------------------------------------------
        // ApplySettingsDataToStoresAsync
        // -----------------------------------------------------------------------

        [Test]
        public async Task ApplySettingsDataToStoresAsync_UpdatesSettings()
        {
            var globalStore = new Store<GameGlobalState>(
                GameGlobalState.Default);

            var settingStore = new Store<GameSettingState>(
                GameSettingState.Default);

            var applier = new SaveDataApplier(
                globalStore,
                settingStore);

            var saveData = new SettingSaveData(
                bgmVolume: 1.0f,
                seVolume: 0.1f);

            await applier.ApplySettingsDataToStoresAsync(saveData).AsTask();

            var setting = settingStore.State.CurrentValue;

            Assert.That(setting.BGMVolume.Value, Is.EqualTo(1.0f));
            Assert.That(setting.SEVolume.Value, Is.EqualTo(0.1f));
        }

        [Test]
        public async Task ApplySettingsDataToStoresAsync_DoesNotChangeGlobalState()
        {
            var globalStore = new Store<GameGlobalState>(
                new GameGlobalState(
                    highScore: new Score(123),
                    currentScore: new Score(456)));

            var settingStore = new Store<GameSettingState>(
                GameSettingState.Default);

            var applier = new SaveDataApplier(
                globalStore,
                settingStore);

            await applier.ApplySettingsDataToStoresAsync(
                new SettingSaveData(
                    bgmVolume: 0.2f,
                    seVolume: 0.3f))
                .AsTask();

            var global = globalStore.State.CurrentValue;

            Assert.That(global.HighScore.Value, Is.EqualTo(123));
            Assert.That(global.CurrentScore.Value, Is.EqualTo(456));
        }

        // -----------------------------------------------------------------------
        // ApplyAllDataToStoresAsync
        // -----------------------------------------------------------------------

        [Test]
        public async Task ApplyAllDataToStoresAsync_UpdatesBothStores()
        {
            var globalStore = new Store<GameGlobalState>(
                new GameGlobalState(
                    highScore: new Score(0),
                    currentScore: new Score(50)));

            var settingStore = new Store<GameSettingState>(
                GameSettingState.Default);

            var applier = new SaveDataApplier(
                globalStore,
                settingStore);

            var gameData = new GameSaveData(
                testScore: 10,
                testHighScore: 777);

            var settingData = new SettingSaveData(
                bgmVolume: 0.9f,
                seVolume: 0.4f);

            await applier
                .ApplyAllDataToStoresAsync(gameData, settingData)
                .AsTask();

            var global = globalStore.State.CurrentValue;
            var setting = settingStore.State.CurrentValue;

            Assert.That(global.HighScore.Value, Is.EqualTo(777));
            Assert.That(global.CurrentScore.Value, Is.EqualTo(50));

            Assert.That(setting.BGMVolume.Value, Is.EqualTo(0.9f));
            Assert.That(setting.SEVolume.Value, Is.EqualTo(0.4f));
        }
    }
}