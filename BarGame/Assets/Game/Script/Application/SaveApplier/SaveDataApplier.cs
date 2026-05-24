using System.Threading.Tasks;
using Game.Domain;
using Game.Kernel;

namespace Game.Application
{
    public sealed class SaveDataApplier : ISaveDataApplier
    {
        private readonly IStore<GameGlobalState> _globalStore;
        private readonly IStore<GameSettingState> _settingStore;

        public SaveDataApplier(IStore<GameGlobalState> globalStore, IStore<GameSettingState> settingStore)
        {
            _globalStore = globalStore;
            _settingStore = settingStore;
        }

        /// <summary>
        /// ゲームのセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。
        /// </summary>
        /// <param name="saveData">適用するゲームのセーブデータ</param>
        public Task ApplyGameDataToStoresAsync(GameSaveData saveData)
        {
            var globalCurrent = _globalStore.CurrentState;
            var settingCurrent = _settingStore.CurrentState;

            _globalStore.Dispatch(_ => new GameGlobalState(
                highScore:    new Score(saveData.TestHighScore),
                currentScore: globalCurrent.CurrentScore
            ));
            _settingStore.Dispatch(_ => new GameSettingState(
                seVolume:  settingCurrent.SEVolume,
                bgmVolume: settingCurrent.BGMVolume
            ));
            return Task.CompletedTask;
        }

        /// <summary>
        /// 設定のセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。
        /// </summary>
        /// <param name="saveData">適用する設定のセーブデータ</param>
        public Task ApplySettingsDataToStoresAsync(SettingSaveData saveData)
        {
            var current = _globalStore.CurrentState;

            _globalStore.Dispatch(_ => new GameGlobalState(
                currentScore: current.CurrentScore,
                highScore:    current.HighScore
            ));
            _settingStore.Dispatch(_ => new GameSettingState(
                seVolume:  new AudioVolume(saveData.SEVolume),
                bgmVolume: new AudioVolume(saveData.BGMVolume)
            ));
            return Task.CompletedTask;
        }

        /// <summary>
        /// ゲームのセーブデータと設定のセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。
        /// </summary>
        /// <param name="saveData">適用するゲームのセーブデータ</param>
        /// <param name="settingData">適用する設定のセーブデータ</param>
        public Task ApplyAllDataToStoresAsync(GameSaveData saveData, SettingSaveData settingData)
        {
            var current = _globalStore.CurrentState;

            _globalStore.Dispatch(_ => new GameGlobalState(
                highScore:    new Score(saveData.TestHighScore),
                currentScore: current.CurrentScore
            ));
            _settingStore.Dispatch(_ => new GameSettingState(
                seVolume:  new AudioVolume(settingData.SEVolume),
                bgmVolume: new AudioVolume(settingData.BGMVolume)
            ));
            return Task.CompletedTask;
        }
    }
}