using UnityEngine;
using UnityEngine.UI;
using System;
using R3;
using Cysharp.Threading.Tasks;
using VContainer;
using Game.Domain;
using Game.Kernel.Utils.R3;

namespace Game.Presentation.View
{
    public class AudioVolumeChanger : MonoBehaviour
    {
        [Inject] private Store<GameSettingState> _settingStore = null!;
        private CompositeDisposable _disposables = new();

        [SerializeField] private VolumeType volumeType = VolumeType.BGM;
        [SerializeField] private Slider slider;

        private void Start()
        { 
            _settingStore.State.Subscribe(state =>
            {
                switch (volumeType)
                {
                    case VolumeType.BGM:
                        slider.SetValueWithoutNotify(state.BGMVolume.Value);//ループ防止
                        break;
                    case VolumeType.SE:
                        slider.SetValueWithoutNotify(state.SEVolume.Value);//ループ防止
                        break;
                }
            }).AddTo(_disposables);

            if (slider != null)
            {
                slider.value = _settingStore.State.CurrentValue switch
                {
                    { } s when volumeType == VolumeType.BGM => s.BGMVolume.Value,
                    { } s when volumeType == VolumeType.SE => s.SEVolume.Value,
                    _ => throw new ArgumentOutOfRangeException()
                };
                slider.onValueChanged.AddListener(SetVolume);
            }
        }

        // Sliderから呼ばれる
        public void SetVolume(float value)// valueは0.0f〜1.0f想定
        {
            switch (volumeType)
            {
                case VolumeType.BGM:
                    _settingStore.Dispatch(GameSettingStateReducer.SetBGMVolume(new AudioVolume(value)));
                    break;
                case VolumeType.SE:
                    _settingStore.Dispatch(GameSettingStateReducer.SetSEVolume(new AudioVolume(value)));
                    break;
            }
            slider.value = value;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            slider.onValueChanged.RemoveAllListeners();
        }
    }
}