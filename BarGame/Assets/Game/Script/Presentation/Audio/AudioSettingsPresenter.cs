using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Audio;
using VContainer.Unity;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel;
using Game.Kernel.Utils.R3;

namespace Game.Presentation
{
    /// <summary>
    /// AudioMixerのGroup名をVolumeTypeと同じにしている前提で実装している。
    /// </summary>
    public enum VolumeType { Master, SE, BGM }

    public sealed class AudioSettingsPresenter : IAudioSettingsPresenter, IAsyncStartable, IDisposable
    {
        private readonly Store<GameSettingState> _settingState;
        private readonly ISaveService _saveService;
        private readonly AudioMixer _audioMixer;

        private readonly CompositeDisposable _disposables = new();
        private (float Bgm, float Se) _latest;

        public AudioSettingsPresenter(Store<GameSettingState> globalStore, ISaveService saveService, AudioMixer audioMixer)
        {
            _settingState = globalStore;
            _saveService = saveService;
            _audioMixer = audioMixer;
        }

        public async UniTask StartAsync(CancellationToken ct)
        {
            // 1) 設定ロード → Storeへ反映（Storeを真実にする）
            SettingSaveData save;
            try { save = await _saveService.LoadSettingsDataAsync(ct); }
            catch { save = SettingSaveData.Default(); }

            var se = Mathf.Clamp01(save.SEVolume);
            var bgm = Mathf.Clamp01(save.BGMVolume);

            _settingState.Dispatch(GameSettingStateReducer.SetSEVolume(new AudioVolume(se)));
            _settingState.Dispatch(GameSettingStateReducer.SetBGMVolume(new AudioVolume(bgm)));

            // 2) 現在値で即適用
            var s = _settingState.State.CurrentValue;
            _latest = (s.BGMVolume.Value, s.SEVolume.Value);
            ApplyAll(_latest);

            // 3) Store変更購読 → Mixerへ反映
            _settingState.State.Subscribe(state =>
            {
                var next = (state.BGMVolume.Value, state.SEVolume.Value);
                if (next.Equals(_latest)) return;
                _latest = next;
                ApplyAll(_latest);
            }).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();

        private void ApplyAll((float Bgm, float Se) v)
        {
            _audioMixer.SetFloat(VolumeType.BGM.ToString(), ConvertVolume.PercentToDecibel(v.Bgm));
            _audioMixer.SetFloat(VolumeType.SE.ToString(), ConvertVolume.PercentToDecibel(v.Se));
        }

        public void SetVolume(VolumeType volumeType, float value)
        {
            value = Mathf.Clamp01(value);
            switch (volumeType)
            {
                case VolumeType.BGM:
                    _settingState.Dispatch(GameSettingStateReducer.SetBGMVolume(new AudioVolume(value)));
                    break;
                case VolumeType.SE:
                    _settingState.Dispatch(GameSettingStateReducer.SetSEVolume(new AudioVolume(value)));
                    break;
            }
        }

        public float GetDecibel(VolumeType volumeType)
        {
            _audioMixer.GetFloat(volumeType.ToString(), out float dB);
            return dB;
        }
    }
}