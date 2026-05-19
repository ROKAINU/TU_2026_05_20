using Cysharp.Threading.Tasks;
using VContainer;
using Game.Domain;
using Game.Kernel.Utils.R3;

namespace Game.Application
{
    public sealed class SaveDataApplier : ISaveDataApplier
    {
        private readonly Store<GameGlobalState> _globalStore;
        private readonly Store<GameSettingState> _settingStore;

        public SaveDataApplier(Store<GameGlobalState> globalStore, Store<GameSettingState> settingStore)
        {
            _globalStore = globalStore;
            _settingStore = settingStore;
        }

        /// <summary>
        /// ゲームのセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。
        /// </summary>
        /// <param name="saveData">適用するゲームのセーブデータ</param>
        public UniTask ApplyGameDataToStoresAsync(GameSaveData saveData)
        {
            var globalCurrent = _globalStore.State.CurrentValue;
            var settingCurrent = _settingStore.State.CurrentValue;

            var newGlobalState = new GameGlobalState(
                highScore: new Score(saveData.TestHighScore),
                currentScore: globalCurrent.CurrentScore
            );

            var newSettingState = new GameSettingState(
                seVolume: settingCurrent.SEVolume,
                bgmVolume: settingCurrent.BGMVolume
            );

            _globalStore.Dispatch(GameGlobalStateReducer.Update(newGlobalState));
            _settingStore.Dispatch(GameSettingStateReducer.Update(newSettingState));
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 設定のセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。
        /// </summary>
        /// <param name="saveData">適用する設定のセーブデータ</param>
        public UniTask ApplySettingsDataToStoresAsync(SettingSaveData saveData)
        {
            var current = _globalStore.State.CurrentValue;

            var newGlobalState = new GameGlobalState(
                currentScore: current.CurrentScore,
                highScore: current.HighScore
            );

            var newSettingState = new GameSettingState(
                seVolume: new AudioVolume(saveData.SEVolume),
                bgmVolume: new AudioVolume(saveData.BGMVolume)
            );

            _globalStore.Dispatch(GameGlobalStateReducer.Update(newGlobalState));
            _settingStore.Dispatch(GameSettingStateReducer.Update(newSettingState));
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// ゲームのセーブデータと設定のセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。
        /// </summary>
        /// <param name="saveData">適用するゲームのセーブデータ</param>
        /// <param name="settingData">適用する設定のセーブデータ</param>
        public UniTask ApplyAllDataToStoresAsync(GameSaveData saveData, SettingSaveData settingData)
        {
            var current = _globalStore.State.CurrentValue;

            var newGlobalState = new GameGlobalState(
                highScore: new Score(saveData.TestHighScore),
                currentScore: current.CurrentScore
            );

            var newSettingState = new GameSettingState(
                seVolume: new AudioVolume(settingData.SEVolume),
                bgmVolume: new AudioVolume(settingData.BGMVolume)
            );

            _globalStore.Dispatch(GameGlobalStateReducer.Update(newGlobalState));
            _settingStore.Dispatch(GameSettingStateReducer.Update(newSettingState));
            return UniTask.CompletedTask;
        }
    }
}