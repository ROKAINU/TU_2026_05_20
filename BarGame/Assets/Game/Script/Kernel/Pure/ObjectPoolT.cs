#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Kernel;

namespace Game.Kernel
{
    /// <summary>
    /// 汎用オブジェクトプール（Queue + HashSet ベース）。
    /// O(1) の Get/Return を実現し、高効率なオブジェクト再利用を提供。
    /// 
    /// ⚠️ シングルスレッド（Unity メインスレッド）のみ対応。
    /// ⚠️ Clear()/Dispose() 中は Get/Return/ReturnAll を呼べません。
    /// </summary>
    public sealed class ObjectPool<T> : IDisposable where T : class
    {
        private readonly Func<T> _factory;
        private readonly Action<T>? _onGet;
        private readonly Action<T>? _onReturn;
        private readonly Queue<T> _available;
        private readonly HashSet<T> _active;
        private readonly ILogger _logger;
        private bool _isDisposed;
        private bool _isClearing;

        public int AvailableCount => _available.Count;
        public int ActiveCount => _active.Count;
        public int TotalCount => _available.Count + _active.Count;

        public ObjectPool(
            Func<T> factory,
            Action<T>? onGet = null,
            Action<T>? onReturn = null,
            int initialCapacity = 10,
            ILogger? logger = null)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (initialCapacity < 0)
                throw new ArgumentException("Initial capacity must be non-negative", nameof(initialCapacity));

            _factory = factory;
            _onGet = onGet;
            _onReturn = onReturn;
            _available = new Queue<T>(initialCapacity);
            _active = new HashSet<T>();
            _logger = logger ?? new NullLogger();
        }

        /// <summary>
        /// プールを事前に初期化。失敗時は新規オブジェクトを破棄します。
        /// ⚠️ _factory() が例外を投げた場合、それまでに作成されたオブジェクトは破棄されません。
        /// ユーザーは Preload() の例外をキャッチして、必要に応じてクリーンアップしてください。
        /// </summary>
        public void Preload(int count)
        {
            ThrowIfDisposed();

            if (count < 0)
                throw new ArgumentException("Count must be non-negative", nameof(count));

            var created = new List<T>(count);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var obj = _factory();
                    created.Add(obj);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"[ObjectPool<{typeof(T).Name}>] Preload failed at {created.Count}/{count}. {ex.Message}"
                );
                throw;
            }

            foreach (var obj in created)
            {
                _available.Enqueue(obj);
            }
        }

        /// <summary>
        /// オブジェクトを取得（O(1)）。
        /// </summary>
        public T Get()
        {
            ThrowIfDisposed();

            if (_isClearing)
                throw new InvalidOperationException(
                    $"[ObjectPool<{typeof(T).Name}>] Cannot Get() while Clear() is in progress."
                );

            T obj;
            
            if (_available.Count > 0)
            {
                obj = _available.Dequeue();
            }
            else
            {
                try
                {
                    obj = _factory();
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        $"[ObjectPool<{typeof(T).Name}>] Factory failed to create object: {ex.Message}"
                    );
                    throw;
                }
            }

            _active.Add(obj);

            try
            {
                _onGet?.Invoke(obj);
            }
            catch
            {
                _active.Remove(obj);
                _available.Enqueue(obj);
                throw;
            }

            return obj;
        }

        /// <summary>
        /// オブジェクトをプールに戻す（O(1)）。
        /// </summary>
        public void Return(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            ThrowIfDisposed();

            if (_isClearing)
                throw new InvalidOperationException(
                    $"[ObjectPool<{typeof(T).Name}>] Cannot Return() while Clear() is in progress."
                );

            if (!_active.Remove(obj))
            {
                throw new InvalidOperationException(
                    $"[ObjectPool<{typeof(T).Name}>] Object was not acquired from this pool."
                );
            }

            try
            {
                _onReturn?.Invoke(obj);
            }
            finally
            {
                _available.Enqueue(obj);
            }
        }

        /// <summary>
        /// すべての Active オブジェクトを Return でプールに戻す。
        /// _onReturn が Get() を呼んでも対応。
        /// 性能最適化：初期 snapshot を取り、複数回実行は無限ループ検出時のみ。
        /// </summary>
        public void ReturnAll()
        {
            ThrowIfDisposed();

            if (_isClearing)
                throw new InvalidOperationException(
                    $"[ObjectPool<{typeof(T).Name}>] Cannot ReturnAll() while Clear() is in progress."
                );

            if (_active.Count == 0)
                return;

            var errors = 0;

            // 初期 snapshot を一度だけ取得
            var toReturn = new List<T>(_active);
            foreach (var obj in toReturn)
            {
                try
                {
                    Return(obj);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        $"[ObjectPool<{typeof(T).Name}>] Error returning object: {ex.Message}"
                    );
                    errors++;
                }
            }

            // _onReturn が Get() を呼んだ場合、残りがあれば再処理
            if (_active.Count > 0)
            {
                ReturnRemaining(ref errors);
            }

            if (errors > 0)
            {
                _logger.LogWarning(
                    $"[ObjectPool<{typeof(T).Name}>] ReturnAll completed with {errors} errors"
                );
            }
        }

        /// <summary>
        /// 残りの Active オブジェクトを Return する（無限ループ検出付き）。
        /// </summary>
        private void ReturnRemaining(ref int errors)
        {
            var previousActiveCount = -1;
            var noProgressIterations = 0;
            const int maxNoProgressIterations = 1000;

            while (_active.Count > 0)
            {
                var currentActiveCount = _active.Count;

                if (currentActiveCount == previousActiveCount)
                {
                    noProgressIterations++;
                    if (noProgressIterations >= maxNoProgressIterations)
                    {
                        _logger.LogError(
                            $"[ObjectPool<{typeof(T).Name}>] ReturnAll detected infinite loop. " +
                            $"Remaining active: {_active.Count}. " +
                            $"_onReturn callback might be causing issues."
                        );
                        break;
                    }
                }
                else
                {
                    noProgressIterations = 0;
                    previousActiveCount = currentActiveCount;
                }

                var toReturn = new List<T>(_active);
                foreach (var obj in toReturn)
                {
                    try
                    {
                        Return(obj);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            $"[ObjectPool<{typeof(T).Name}>] Error returning object: {ex.Message}"
                        );
                        errors++;
                    }
                }
            }
        }

        /// <summary>
        /// プール内のすべてのオブジェクトをクリア。
        /// </summary>
        public void Clear()
        {
            ThrowIfDisposed();

            if (_isClearing)
                throw new InvalidOperationException(
                    "Clear is already in progress. Nested Clear is not allowed."
                );

            _isClearing = true;
            try
            {
                var allObjects = new List<T>(_available.Count + _active.Count);

                while (_available.Count > 0)
                    allObjects.Add(_available.Dequeue());

                var activeList = _active.ToList();
                _active.Clear();

                foreach (var obj in activeList)
                    allObjects.Add(obj);

                if (_onReturn != null)
                {
                    foreach (var obj in allObjects)
                    {
                        try
                        {
                            _onReturn(obj);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                $"[ObjectPool<{typeof(T).Name}>] Error in onReturn: {ex.Message}"
                            );
                        }
                    }
                }
            }
            finally
            {
                _isClearing = false;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            try
            {
                Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"[ObjectPool<{typeof(T).Name}>] Error during Dispose: {ex.Message}"
                );
            }
            finally
            {
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(ObjectPool<T>));
        }
    }
}