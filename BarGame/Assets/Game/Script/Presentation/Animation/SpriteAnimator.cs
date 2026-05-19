#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Presentation
{
    /// <summary>
    /// シンプルなスプライトアニメーション。
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] frames = Array.Empty<Sprite>();

        [SerializeField]
        private float fps = 12f;

        [SerializeField]
        private bool loop = true;

        private SpriteRenderer? _renderer;
        private CancellationTokenSource? _cts;

        private bool _isPaused;

        public bool IsPlaying { get; private set; }

        public event Action? OnFinished;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();

            if (frames.Length == 0)
            {
                Debug.LogWarning("[SpriteAnimator] No frames assigned.", this);
            }

            if (fps <= 0f)
            {
                Debug.LogWarning("[SpriteAnimator] FPS must be > 0. Using default 12.", this);
                fps = 12f;
            }
        }

        private void OnDestroy()
        {
            Stop();
        }

        public void Play()
        {
            Stop();

            if (frames.Length == 0)
                return;

            _cts = new CancellationTokenSource();

            IsPlaying = true;
            _isPaused = false;

            PlayLoopAsync(_cts.Token).Forget();
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            IsPlaying = false;
            _isPaused = false;
        }

        public void Pause()
        {
            if (!IsPlaying)
                return;

            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        private async UniTaskVoid PlayLoopAsync(CancellationToken ct)
        {
            try
            {
                do
                {
                    for (int i = 0; i < frames.Length; i++)
                    {
                        ct.ThrowIfCancellationRequested();

                        while (_isPaused)
                        {
                            await UniTask.Yield(PlayerLoopTiming.Update, ct);
                        }

                        if (_renderer != null)
                        {
                            _renderer.sprite = frames[i];
                        }

                        await UniTask.Delay(
                            TimeSpan.FromSeconds(1f / fps),
                            cancellationToken: ct);
                    }
                }
                while (loop);

                OnFinished?.Invoke();
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                IsPlaying = false;
            }
        }
    }
}