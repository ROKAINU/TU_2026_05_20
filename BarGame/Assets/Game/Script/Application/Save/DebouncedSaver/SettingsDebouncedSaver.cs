#nullable enable
using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel.Utils.R3;

namespace Game.Application
{
    public sealed class SettingsDebouncedSaver : IDisposable
    {
        private readonly ISaveService _saveService;
        private readonly Store<GameSettingState> _settingStore;
        private readonly TimeSpan _delay;

        private CancellationTokenSource? _cts;
        private SettingSaveData _latest;
        private readonly CompositeDisposable _disposables = new();

        public SettingsDebouncedSaver(
            ISaveService saveService,
            Store<GameSettingState> settingStore)
        {
            _saveService = saveService;
            _settingStore = settingStore;
            _delay = TimeSpan.FromMilliseconds(500);
        }

        /// <summary>
        /// 設定の変更を監視し、変更があった場合に一定時間待ってから保存する。変更が頻繁にある場合は、最後の変更から一定時間経過してから保存される。
        /// </summary>
        public void Start()
        {
            _latest = new SettingSaveData(
                bgmVolume: _settingStore.State.CurrentValue.BGMVolume.Value,
                seVolume: _settingStore.State.CurrentValue.SEVolume.Value
            );

            _settingStore.State.Subscribe(state =>
            {
                var next = new SettingSaveData(
                    bgmVolume: state.BGMVolume.Value,
                    seVolume: state.SEVolume.Value
                );
                if (next.Equals(_latest)) return;

                _latest = next;
                ScheduleSave();
            }).AddTo(_disposables);//購読はこのクラスの寿命に合わせて解除する
        }

        /// <summary>
        /// 保存のスケジュールを設定する。既に保存がスケジュールされている場合はキャンセルし、新たに保存をスケジュールする。
        /// </summary>
        private void ScheduleSave()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            SaveAfterDelayAsync(_cts.Token).Forget();
        }

        /// <summary>
        /// 一定時間待ってから保存する非同期メソッド。途中でキャンセルされた場合は、保存を行わない。
        /// </summary>
        /// <param name="ct">キャンセル用のトークン</param>
        private async UniTaskVoid SaveAfterDelayAsync(CancellationToken ct)
        {
            try
            {
                await UniTask.Delay(_delay, cancellationToken: ct);

                // 保存時点の最新値で保存
                var data = new SettingSaveData(
                    bgmVolume: _latest.BGMVolume,
                    seVolume: _latest.SEVolume
                );

                await _saveService.SaveSettingsDataAsync(data, ct);
            }
            catch (OperationCanceledException)
            {
                // debounceによりキャンセルされるのは正常
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 保存を即座に実行する。これにより、現在スケジュールされている保存はキャンセルされ、最新の設定がすぐに保存される。
        /// </summary>
        /// <param name="ct">キャンセル用のトークン</param>
        public async UniTask FlushAsync(CancellationToken ct = default)
        {
            _cts?.Cancel();

            var data = new SettingSaveData(
                bgmVolume: _latest.BGMVolume,
                seVolume: _latest.SEVolume
            );

            await _saveService.SaveSettingsDataAsync(data, ct);
        }

        /// <summary>
        /// 保存のスケジュールをキャンセルし、リソースを解放する。これにより、現在スケジュールされている保存はキャンセルされる。
        /// </summary>
        public void Dispose()
        {
            _disposables.Dispose();
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}