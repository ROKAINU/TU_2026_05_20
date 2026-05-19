#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using LitMotion;
using LitMotion.Extensions;
using Game.Kernel;

namespace Game.Presentation
{
    /// <summary>
    /// BGM再生管理クラス
    /// 機能: 再生、停止、音量フェード、ピッチ変更
    /// </summary>

    [RequireComponent(typeof(AudioSource))]
    public class BGMPlayer : MonoBehaviour
    {
        private AudioSource _audioSource = null!;
                
        // 状態管理フラグ
        private MotionHandle _volumeMotion;
        private MotionHandle _pitchMotion;

        // プロパティ: 外部から状態確認可能にする
        public bool IsPlaying => _audioSource.isPlaying;
        public float CurrentVolume => _audioSource.volume;
        public float CurrentPitch => _audioSource.pitch;

        private LoggerBase _logger = null!;

        [Inject]
        public void Construct(LoggerBase logger)
        {
            _logger = logger;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// AudioSource の初期設定をリセット
        /// </summary>
        public void InitializeSettings()
        {
            _audioSource.volume = 1f;
            _audioSource.pitch = 1f;
        }

        /// <summary>
        /// BGMの再生を停止
        /// </summary>
        public void Stop()
        {
            _audioSource.Stop();
        }
        
        /// <summary>
        /// 新しいBGMクリップを再生
        /// </summary>
        /// <param name="clip">再生するAudioClip</param>
        public void PlayBGM(AudioClip? clip = null)
        {
            if (clip == null && _audioSource.clip == null)
            {
                _logger.LogError("PlayBGM : AudioClip が null です");
                return;
            }

            Stop();
            InitializeSettings();
            if (clip != null)
                _audioSource.clip = clip;
            _audioSource.Play();

            _logger.LogInfo($"BGM 再生開始: {_audioSource.clip.name} (IsPlaying: {_audioSource.isPlaying})");
        }

        /// <summary>
        /// 新しいBGMクリップをそのまま再生
        /// </summary>
        /// <param name="clip">再生するAudioClip</param>
        /// <param name="volume">0～1の音量値</param>
        public void PlayClipRaw(AudioClip clip, float volume)
        {
            _audioSource.Stop();
            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.Play();
        }

        /// <summary>
        /// スナップショットを返す関数
        /// </summary>
        /// <returns>スナップショット</returns>
        public BGMSnapshot CaptureSnapshot()
        {
            return new BGMSnapshot(_audioSource.clip, CurrentVolume, CurrentPitch);
        }

        /// <summary>
        /// 音量を変更
        /// </summary>
        /// <param name="volume">0～1の音量値</param>

        public void VolumeChange(float volume)
        {
            _audioSource.volume = volume;
        }
        
        /// <summary>
        /// 音量をTween（フェード）
        /// 同時実行は防止される
        /// </summary>
        /// <param name="duration">フェード時間（秒）</param>
        /// <param name="targetVolume">目標の音量値（0～1）</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask TweenVolume(float duration, float targetVolume, CancellationToken cancellationToken = default)
        {
            try
            {
                // 前回Tweenを停止
                _volumeMotion.TryCancel();

                _volumeMotion = LMotion
                    .Create(_audioSource.volume, targetVolume, duration)
                    .Bind(x => _audioSource.volume = x);

                await _volumeMotion.ToUniTask(cancellationToken);

                _logger.LogInfo($"[BGMPlayer] Volume tween completed : {targetVolume:P0}");
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("[BGMPlayer] Volume tween was canceled.");

                throw;
            }
        }

        /// <summary>
        /// ピッチを変更
        /// </summary>
        /// <param name="pitch">ピッチ値</param>

        public void PitchChange(float pitch)
        {
            _audioSource.pitch = pitch;
        }

        /// <summary>
        /// ピッチをTween(フェード)
        /// 同時実行は防止される
        /// </summary>
        /// <param name="duration">トゥイーン時間（秒）</param>
        /// <param name="targetPitch">目標のピッチ値</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask TweenPitch(float duration, float targetPitch, CancellationToken cancellationToken = default)
        {
            try
            {
                _pitchMotion.TryCancel();

                _pitchMotion = LMotion
                    .Create(_audioSource.pitch, targetPitch, duration)
                    .Bind(x => _audioSource.pitch = x);

                await _pitchMotion.ToUniTask(cancellationToken);

                _logger.LogInfo($"[BGMPlayer] Pitch tween completed : {targetPitch:F2}x");
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("[BGMPlayer] Pitch tween was canceled.");

                throw;
            }
        }
    }
    public readonly struct BGMSnapshot
    {
        public AudioClip? Clip    { get; }
        public float Volume  { get; }
        public float Pitch  { get; }

        public BGMSnapshot(AudioClip? clip, float volume, float pitch)
        {
            Clip   = clip;
            Volume = volume;
            Pitch = pitch;
        }
    }
}