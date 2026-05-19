using UnityEngine;
using Game.Application.Contracts;
using VContainer;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Application;
using Game.Kernel;
using Game.Kernel.Utils.R3;

namespace Game.Presentation.View
{
    public class ResetData : MonoBehaviour
    {
        private Store<GameGlobalState> _globalStore;
        private ISaveService _saveService;
        private ISaveDataApplier _saveDataApplier;

        [Inject]
        internal void Construct(Store<GameGlobalState> globalStore, ISaveService saveService, ISaveDataApplier saveDataApplier)
        {
            _globalStore = globalStore;
            _saveService = saveService;
            _saveDataApplier = saveDataApplier;
        }

        public void ResetGameData()
        {
            ResetGameDataAsync().Forget();
        }

        public async UniTask ResetGameDataAsync()
        {
            await _saveService.InitializeAsync();
            var gameData = await _saveService.LoadGameDataAsync();
            var settingsData = await _saveService.LoadSettingsDataAsync();
            await _saveDataApplier.ApplyAllDataToStoresAsync(gameData, settingsData);
        }
    }
}