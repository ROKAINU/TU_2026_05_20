using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel;
using Game.Kernel.Utils.R3;

namespace Game.Presentation.View
{
    public class SaveSettingData : MonoBehaviour
    {
        private Store<GameSettingState> _settingStore;
        private ISaveService _saveService;

        [Inject]
        internal void Construct(Store<GameSettingState> settingStore, ISaveService saveService)
        {
            _settingStore = settingStore;
            _saveService = saveService;
        }

        public void SaveData()
        {
            SaveDataAsync().Forget();
        }

        public async UniTask SaveDataAsync()
        {
            var currentState = _settingStore.State.CurrentValue;

            var saveData = new SettingSaveData(
                bgmVolume: currentState.BGMVolume.Value,
                seVolume : currentState.SEVolume.Value
            );

            await _saveService.SaveSettingsDataAsync(saveData);
        }
    }
}
