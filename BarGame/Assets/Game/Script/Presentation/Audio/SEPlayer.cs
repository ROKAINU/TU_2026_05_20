using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using VContainer;
using UnityEngine;
using Game.Kernel;

namespace Game.Presentation
{
    /// <summary>
    /// SE（効果音）プレイヤー。
    /// AudioSource をプール管理し、複数の SE を効率的に再生。
    /// </summary>
    public class SEPlayer : MonoBehaviour
    {
        private ObjectPool<AudioSource> _pool = null!;
        private readonly int _defaultPoolSize = 10;
        private readonly HashSet<AudioSource> _activeInPool = new();
        private bool _isDisposed = false;
        private int _seInstanceCount = 0;

        private IRandom _random = null!;
        private LoggerBase _logger = null!;

        [Inject]
        public void Construct(
            IRandom random,
            LoggerBase logger)
        {
            _random = random;
            _logger = logger;
        }

        #region Initialization and Pool Management

        private void Awake()
        {
            InitializePool();
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        /// <summary>
        /// AudioSource プール を初期化。
        /// </summary>
        private void InitializePool()
        {
            var templateSource = GetComponent<AudioSource>();
            
            if (templateSource == null)
            {
                _logger.LogError("[SEPlayer] AudioSource component not found on this GameObject.");
                enabled = false;
                return;
            }

            AudioSource FactoryMethod()
            {
                var go = new GameObject($"SE_{_seInstanceCount++}");
                go.transform.SetParent(transform);

                var source = go.AddComponent<AudioSource>();
                CopyAudioSourceSettings(templateSource, source);
                source.playOnAwake = false;

                return source;
            }

            void OnGetCallback(AudioSource source)
            {
                source.gameObject.SetActive(true);
            }

            void OnReturnCallback(AudioSource source)
            {
                source.Stop();
                source.clip = null;
                source.gameObject.SetActive(false);
            }

            _pool = new ObjectPool<AudioSource>(
                factory: FactoryMethod,
                onGet: OnGetCallback,
                onReturn: OnReturnCallback,
                initialCapacity: _defaultPoolSize
            );

            _pool.Preload(_defaultPoolSize);
        }

        /// <summary>
        /// SEPlayer をシャットダウン。
        /// </summary>
        private void Shutdown()
        {
            _isDisposed = true;
            _activeInPool.Clear();
            _pool?.Dispose();
        }

        #endregion

        #region Public API

        /// <summary>
        /// SE を再生。再生終了後は自動的にプールに返却。
        /// </summary>
        public void PlaySE(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            ThrowIfNotInitialized();

            if (clip == null)
            {
                _logger.LogWarning("[SEPlayer] AudioClip is null.");
                return;
            }

            var source = _pool.Get();
            _activeInPool.Add(source);
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();

            ReturnWhenFinishedAsync(source).Forget();
        }

        /// <summary>
        /// SE をランダムなピッチで再生。
        /// </summary>
        public void PlaySEWithRandomPitch(AudioClip clip, float volume = 1f, PitchSetting pitchSetting = new())
        {
            ThrowIfNotInitialized();

            if (clip == null)
            {
                _logger.LogWarning("[SEPlayer] AudioClip is null.");
                return;
            }

            var source = _pool.Get();
            _activeInPool.Add(source);
            source.clip = clip;
            source.volume = volume;
            source.pitch = _random.Range(
                pitchSetting.basePitch - pitchSetting.pitchRange,
                pitchSetting.basePitch + pitchSetting.pitchRange
            );
            source.Play();

            ReturnWhenFinishedAsync(source).Forget();
        }

        /// /// <summary>
        /// SE を繰り返し再生。
        /// </summary>
        /// <param name="clip">再生する AudioClip</param>
        /// <param name="playCount">再生回数</param>
        /// <param name="interval">再生間隔（秒）</param>  // ← 単位を明記
        /// <param name="volume">音量（0～1）</param>
        /// <param name="pitchRandomize">ピッチをランダムにするか</param>
        /// <param name="pitchSetting">ピッチ設定</param>
        public async Task PlaySERepeatedlyAsync(
            AudioClip clip,
            int playCount,
            float interval,
            float volume = 1f,
            bool pitchRandomize = true,
            PitchSetting pitchSetting = new())
        {
            ThrowIfNotInitialized();

            if (clip == null)
            {
                _logger.LogWarning("[SEPlayer] AudioClip is null.");
                return;
            }

            if (playCount <= 0)
            {
                _logger.LogWarning("[SEPlayer] playCount must be greater than 0.");
                return;
            }

            for (int i = 0; i < playCount; i++)
            {
                if (_isDisposed)
                    break;

                if (pitchRandomize)
                {
                    PlaySEWithRandomPitch(clip, volume, pitchSetting);
                }
                else
                {
                    PlaySE(clip, volume, pitchSetting.basePitch);
                }

                await UniTask.Delay((int)(interval * 1000));
            }
        }

        /// <summary>
        /// 指定の clip を再生している SE を停止。
        /// </summary>
        public void StopSE(AudioClip clip)
        {
            ThrowIfNotInitialized();

            if (clip == null)
                return;

            var toStop = new List<AudioSource>();
            foreach (var source in _activeInPool)
            {
                if (source != null && source.clip == clip && source.isPlaying)
                {
                    toStop.Add(source);
                }
            }

            foreach (var source in toStop)
            {
                source.Stop();
                RemoveFromPoolSafe(source);
            }
        }

        /// <summary>
        /// すべての SE を停止。
        /// </summary>
        public void StopAllSE()
        {
            ThrowIfNotInitialized();

            _pool.ReturnAll();
            _activeInPool.Clear();
        }

        #endregion

        #region Utility

        /// <summary>
        /// AudioSource が再生終了するまで待機。
        /// </summary>
        private async UniTaskVoid ReturnWhenFinishedAsync(AudioSource source)
        {
            if (source.clip == null)
            {
                RemoveFromPoolSafe(source);
                return;
            }

            float clipLength = source.clip.length;
            float elapsedTime = 0f;

            while (!_isDisposed && source.isPlaying && elapsedTime < clipLength * 1.1f)
            {
                await UniTask.Yield();
                elapsedTime += Time.deltaTime;
            }

            // Dispose 済みならスキップ（_activeInPool.Clear() で処理される）
            if (_isDisposed)
                return;

            if (source != null && source.gameObject != null)
            {
                RemoveFromPoolSafe(source);
            }
        }

        /// <summary>
        /// _activeInPool から削除してプールに返却（統一的な処理）
        /// </summary>
        private void RemoveFromPoolSafe(AudioSource source)
        {
            if (_activeInPool.Remove(source))
            {
                try
                {
                    _pool.Return(source);
                }
                catch (ObjectDisposedException)
                {
                    // _pool が既に Dispose されている
                }
            }
        }

        /// <summary>
        /// SEPlayer が初期化されているか確認。
        /// </summary>
        private void ThrowIfNotInitialized()
        {
            if (_pool == null || _isDisposed)
                throw new InvalidOperationException("[SEPlayer] SEPlayer is not initialized or has been destroyed.");
        }

        /// <summary>
        /// AudioSource の設定をコピー（テンプレート → ターゲット）
        /// </summary>
        private void CopyAudioSourceSettings(AudioSource from, AudioSource to)
        {
            to.outputAudioMixerGroup = from.outputAudioMixerGroup;
            to.bypassEffects = from.bypassEffects;
            to.bypassListenerEffects = from.bypassListenerEffects;
            to.bypassReverbZones = from.bypassReverbZones;
            to.loop = from.loop;
            to.priority = from.priority;
            to.volume = from.volume;
            to.pitch = from.pitch;
            to.panStereo = from.panStereo;
            to.spatialBlend = from.spatialBlend;
            to.reverbZoneMix = from.reverbZoneMix;
            to.dopplerLevel = from.dopplerLevel;
            to.spread = from.spread;
            to.minDistance = from.minDistance;
            to.maxDistance = from.maxDistance;
            to.rolloffMode = from.rolloffMode;
        }

        #endregion
    }

    /// <summary>
    /// ピッチ設定
    /// </summary>
    public struct PitchSetting
    {
        public float basePitch;
        public float pitchRange;

        public PitchSetting(float basePitch = 1f, float pitchRange = 0.1f)
        {
            this.basePitch = basePitch;
            this.pitchRange = pitchRange;
        }
    }
}