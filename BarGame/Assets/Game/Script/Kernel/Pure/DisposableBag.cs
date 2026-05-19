#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;

namespace Game.Kernel
{
    /// <summary>
    /// 複数の <see cref="IDisposable"/>（購読解除など）をまとめて破棄するための袋。
    ///
    /// 方針（安全運用寄り）:
    /// - <see cref="Dispose"/> は例外を投げない（using/finallyから呼ばれることが多いため）
    /// - 例外情報が必要な場合は <see cref="TryDispose"/> / <see cref="TryClear"/> を使用する
    /// - 解除は逆順（LIFO）
    /// - Dispose 済みの bag に追加された disposable はリーク防止のため即 Dispose される
    /// </summary>
    public sealed class DisposableBag : IDisposable
    {
        private readonly object _gate = new object();

        private bool _disposed;
        private List<IDisposable>? _items;

        /// <summary>破棄済みかどうか</summary>
        public bool IsDisposed
        {
            get
            {
                lock (_gate)
                {
                    return _disposed;
                }
            }
        }

        /// <summary>
        /// 現在の登録数。
        /// Dispose 済みの場合は常に 0。
        /// </summary>
        public int Count
        {
            get
            {
                lock (_gate)
                {
                    if (_disposed) return 0;
                    return _items?.Count ?? 0;
                }
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> を登録する。
        /// bag が既に Dispose 済みの場合は、その場で即 Dispose される（リーク防止）。
        /// </summary>
        public void Add(IDisposable disposable)
        {
            if (disposable is null) throw new ArgumentNullException(nameof(disposable));

            bool disposeNow;
            lock (_gate)
            {
                disposeNow = _disposed;
                if (!disposeNow)
                {
                    _items ??= new List<IDisposable>();
                    _items.Add(disposable);
                }
            }

            if (disposeNow)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// 複数まとめて登録する。
        /// NOTE: AddRange は原子性（全部追加 or 全部破棄）を保証しない。
        /// 例: 列挙中に別スレッドが Dispose した場合、途中まで追加され、残りは即 Dispose されることがある。
        /// </summary>
        public void AddRange(IEnumerable<IDisposable> disposables)
        {
            if (disposables is null) throw new ArgumentNullException(nameof(disposables));

            foreach (var d in disposables)
            {
                if (d is null) throw new ArgumentException("コレクションに null が含まれています。", nameof(disposables));
                Add(d);
            }
        }

        /// <summary>
        /// 登録済みの disposable を袋から取り除く（Dispose はしない）。
        /// 取り除けたら true。見つからなければ false。Dispose 済みの場合も false。
        /// </summary>
        public bool Remove(IDisposable disposable)
        {
            if (disposable is null) throw new ArgumentNullException(nameof(disposable));

            lock (_gate)
            {
                if (_disposed) return false;
                if (_items is null) return false;
                return _items.Remove(disposable);
            }
        }

        /// <summary>
        /// Action を <see cref="IDisposable"/> として登録するユーティリティ。
        /// </summary>
        public void Add(Action disposeAction)
        {
            if (disposeAction is null) throw new ArgumentNullException(nameof(disposeAction));
            Add(new AnonymousDisposable(disposeAction));
        }

        /// <summary>
        /// 破棄（登録済みの <see cref="IDisposable"/> を全て解除する）。
        /// 例外は投げない。例外情報が必要なら <see cref="TryDispose"/> を使用する。
        /// </summary>
        public void Dispose()
        {
            TryDispose(out _);
        }

        /// <summary>
        /// 登録済みの <see cref="IDisposable"/> を全て解除する。
        /// - 例外が出ても最後まで解除を継続する
        /// - 例外があれば <see cref="AggregateException"/> として返す（投げない）
        /// </summary>
        public bool TryDispose(out AggregateException? exception)
        {
            List<IDisposable>? toDispose;

            lock (_gate)
            {
                if (_disposed)
                {
                    exception = null;
                    return true;
                }

                _disposed = true;
                toDispose = _items;
                _items = null;
            }

            return DisposeAll(toDispose, out exception);
        }

        /// <summary>
        /// bag に登録したものを全て捨て（Dispose）て、再利用可能な空状態に戻す。
        /// ※ bag 自体は Dispose しない。
        ///
        /// - 例外が出ても最後まで解除を継続する
        /// - 例外があれば <see cref="AggregateException"/> として返す（投げない）
        /// </summary>
        public bool TryClear(out AggregateException? exception)
        {
            List<IDisposable>? toDispose;

            lock (_gate)
            {
                if (_disposed)
                {
                    exception = null;
                    return true;
                }

                toDispose = _items;
                _items = null;
            }

            return DisposeAll(toDispose, out exception);
        }

        /// <summary>Clear の簡易版（例外は捨てる）</summary>
        public void Clear()
        {
            TryClear(out _);
        }

        private static bool DisposeAll(IList<IDisposable>? toDispose, out AggregateException? exception)
        {
            exception = null;

            if (toDispose is null || toDispose.Count == 0)
                return true;

            List<Exception>? errors = null;

            for (int i = toDispose.Count - 1; i >= 0; i--)
            {
                try
                {
                    toDispose[i].Dispose();
                }
                catch (Exception ex)
                {
                    errors ??= new List<Exception>();
                    errors.Add(ex);
                }
            }

            if (errors is { Count: > 0 })
            {
                exception = new AggregateException("DisposableBag の破棄中に例外が発生しました。", errors);
                return false;
            }

            return true;
        }

        private sealed class AnonymousDisposable : IDisposable
        {
            private Action? _action;

            public AnonymousDisposable(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                var action = Interlocked.Exchange(ref _action, null);
                action?.Invoke();
            }
        }
    }
}