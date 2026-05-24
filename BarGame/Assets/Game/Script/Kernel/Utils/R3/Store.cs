#nullable enable
using System;
using System.Runtime.CompilerServices;
using R3;
using Game.Kernel;

namespace Game.Kernel.Utils.R3
{
    /// <summary>
    /// Redux風の状態管理コンテナ。
    /// 状態の読み取り・変更・購読を一元管理する。
    ///
    /// ⚠️ Unityメインスレッドからのみ使用する前提。
    /// マルチスレッド環境では Dispatch に lock が必要。
    /// </summary>
    public sealed class Store<T> : IStore<T>, IDisposable
    {
        /// <summary>
        /// ⚠️ State への購読リークの責任はユーザー側にあります。
        /// 複数の購読は .AddTo(disposable) で明確にクリーンアップしてください。
        /// </summary>
        private readonly ReactiveProperty<T> _state;

        public T CurrentState => _state.Value;

        // R3内部にも Disposed 判定はあるが、
        // Store独自の ThrowIfDisposed（より明示的なエラーメッセージ）のために保持する
        private bool _isDisposed;

        /// Log出力が必要な際は注入しておく
        private readonly LoggerBase _logger;

        /// <summary>
        /// 現在の状態を読み取り専用で公開する。
        /// ReactiveProperty は ReadOnlyReactiveProperty を継承しているため
        /// キャストのみ。新しいインスタンス・購読・アロケーションは発生しない。
        /// </summary>
        public ReadOnlyReactiveProperty<T> State => _state;

        public Store(T initialState, LoggerBase? logger = null)
        {
            _state = new ReactiveProperty<T>(initialState);
            _logger = logger ?? new NullLogger();
        }

        /// <summary>
        /// Reducer を通じて状態を変更する唯一の手段。
        /// </summary>
        /// <param name="reducer">現在の状態を受け取り、新しい状態を返す関数</param>
        /// <exception cref="ArgumentNullException">reducer が null の場合</exception>
        /// <exception cref="ObjectDisposedException">Dispose 済みの場合</exception>
        public void Dispatch(
            Func<T, T> reducer,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFile = "")
        {
            if (reducer == null) throw new ArgumentNullException(nameof(reducer));
            ThrowIfDisposed();

        #if DEBUG
            var before = _state.Value;
        #endif

            _state.Value = reducer(_state.Value);

        #if DEBUG
            var fileName = System.IO.Path.GetFileNameWithoutExtension(callerFile);
            //_logger.LogDebug($"[Store<{typeof(T).Name}>] {fileName}.{callerName} | {before} → {_state.Value}");
        #endif
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _state.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Store<T>));
        }
    }
}