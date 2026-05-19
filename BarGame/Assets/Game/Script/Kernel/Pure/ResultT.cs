#nullable enable

using System;

namespace Game.Kernel
{
    /// <summary>
    /// 成功/失敗を型安全に表す結果型（エラーは string）。
    /// 例外を常用しない設計で、呼び出し側にハンドリングを強制できる。
    /// </summary>
    public readonly struct Result<T>
    {
        private readonly T _value;
        private readonly string? _error;

        public bool IsOk { get; }
        public bool IsErr => !IsOk;

        public T Value => IsOk
            ? _value
            : throw new InvalidOperationException("Result is Err. Value is not available.");

        public string Error => IsErr
            ? (_error ?? "Unknown error")
            : throw new InvalidOperationException("Result is Ok. Error is not available.");

        private Result(T value)
        {
            IsOk = true;
            _value = value;
            _error = null;
        }

        private Result(string error)
        {
            IsOk = false;
            _value = default!;
            _error = error;
        }

        public static Result<T> Ok(T value) => new Result<T>(value);
        public static Result<T> Err(string error) => new Result<T>(error);

        public override string ToString()
            => IsOk ? $"Ok({_value})" : $"Err({_error})";

        public TResult Match<TResult>(Func<T, TResult> ok, Func<string, TResult> err)
            => IsOk ? ok(_value) : err(_error ?? "Unknown error");

        public void Switch(Action<T> ok, Action<string> err)
        {
            if (IsOk) ok(_value);
            else err(_error ?? "Unknown error");
        }

        // ----------------------------
        // Added (highest priority): Map / Bind
        // ----------------------------

        /// <summary>
        /// Ok の値だけを変換する。Err はそのまま伝播する。
        /// </summary>
        public Result<TResult> Map<TResult>(Func<T, TResult> map)
        {
            if (map is null) throw new ArgumentNullException(nameof(map));
            return IsOk ? Result<TResult>.Ok(map(_value)) : Result<TResult>.Err(_error ?? "Unknown error");
        }

        /// <summary>
        /// Ok の値を次の Result に連鎖する。Err はそのまま伝播する。
        /// </summary>
        public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> bind)
        {
            if (bind is null) throw new ArgumentNullException(nameof(bind));
            return IsOk ? bind(_value) : Result<TResult>.Err(_error ?? "Unknown error");
        }

        // ----------------------------
        // Added (medium priority): TryGet* / On* hooks
        // ----------------------------

        public bool TryGetValue(out T value)
        {
            if (IsOk)
            {
                value = _value;
                return true;
            }

            value = default!;
            return false;
        }

        public bool TryGetError(out string error)
        {
            if (IsErr)
            {
                error = _error ?? "Unknown error";
                return true;
            }

            error = default!;
            return false;
        }

        /// <summary>
        /// Ok のときだけ副作用を実行し、自分自身を返す（チェーン用）。
        /// </summary>
        public Result<T> OnOk(Action<T> ok)
        {
            if (ok is null) throw new ArgumentNullException(nameof(ok));
            if (IsOk) ok(_value);
            return this;
        }

        /// <summary>
        /// Err のときだけ副作用を実行し、自分自身を返す（チェーン用）。
        /// </summary>
        public Result<T> OnErr(Action<string> err)
        {
            if (err is null) throw new ArgumentNullException(nameof(err));
            if (IsErr) err(_error ?? "Unknown error");
            return this;
        }

        // ----------------------------
        // Interop: convert to Result<T, E>
        // ----------------------------

        /// <summary>
        /// string エラー Result を型付きエラー Result に変換する。
        /// </summary>
        public Result<T, TError> ToResult<TError>(Func<string, TError> mapError)
        {
            if (mapError is null) throw new ArgumentNullException(nameof(mapError));
            return IsOk
                ? Result<T, TError>.Ok(_value)
                : Result<T, TError>.Err(mapError(_error ?? "Unknown error"));
        }
    }

    /// <summary>
    /// 成功/失敗を型安全に表す結果型（エラー型 E を持つ）。
    /// string 版と並行運用できるように、API は Result&lt;T&gt; と近い形にする。
    /// </summary>
    public readonly struct Result<T, E>
    {
        private readonly T _value;
        private readonly E _error;

        public bool IsOk { get; }
        public bool IsErr => !IsOk;

        public T Value => IsOk
            ? _value
            : throw new InvalidOperationException("Result is Err. Value is not available.");

        public E Error => IsErr
            ? _error
            : throw new InvalidOperationException("Result is Ok. Error is not available.");

        private Result(T value)
        {
            IsOk = true;
            _value = value;
            _error = default!;
        }

        private Result(E error)
        {
            IsOk = false;
            _value = default!;
            _error = error;
        }

        public static Result<T, E> Ok(T value) => new Result<T, E>(value);
        public static Result<T, E> Err(E error) => new Result<T, E>(error);

        public override string ToString()
            => IsOk ? $"Ok({_value})" : $"Err({_error})";

        public TResult Match<TResult>(Func<T, TResult> ok, Func<E, TResult> err)
            => IsOk ? ok(_value) : err(_error);

        public void Switch(Action<T> ok, Action<E> err)
        {
            if (IsOk) ok(_value);
            else err(_error);
        }

        // ----------------------------
        // Added (highest priority): Map / Bind
        // ----------------------------

        /// <summary>
        /// Ok の値だけを変換する。Err はそのまま伝播する。
        /// </summary>
        public Result<TResult, E> Map<TResult>(Func<T, TResult> map)
        {
            if (map is null) throw new ArgumentNullException(nameof(map));
            return IsOk ? Result<TResult, E>.Ok(map(_value)) : Result<TResult, E>.Err(_error);
        }

        /// <summary>
        /// Ok の値を次の Result に連鎖する。Err はそのまま伝播する。
        /// </summary>
        public Result<TResult, E> Bind<TResult>(Func<T, Result<TResult, E>> bind)
        {
            if (bind is null) throw new ArgumentNullException(nameof(bind));
            return IsOk ? bind(_value) : Result<TResult, E>.Err(_error);
        }

        // ----------------------------
        // Added (medium priority): MapError / TryGet* / On* hooks
        // ----------------------------

        /// <summary>
        /// Err の値だけを変換する（エラー型の正規化、変換、集約などに使う）。
        /// </summary>
        public Result<T, E2> MapError<E2>(Func<E, E2> mapError)
        {
            if (mapError is null) throw new ArgumentNullException(nameof(mapError));
            return IsOk ? Result<T, E2>.Ok(_value) : Result<T, E2>.Err(mapError(_error));
        }

        public bool TryGetValue(out T value)
        {
            if (IsOk)
            {
                value = _value;
                return true;
            }

            value = default!;
            return false;
        }

        public bool TryGetError(out E error)
        {
            if (IsErr)
            {
                error = _error;
                return true;
            }

            error = default!;
            return false;
        }

        /// <summary>
        /// Ok のときだけ副作用を実行し、自分自身を返す（チェーン用）。
        /// </summary>
        public Result<T, E> OnOk(Action<T> ok)
        {
            if (ok is null) throw new ArgumentNullException(nameof(ok));
            if (IsOk) ok(_value);
            return this;
        }

        /// <summary>
        /// Err のときだけ副作用を実行し、自分自身を返す（チェーン用）。
        /// </summary>
        public Result<T, E> OnErr(Action<E> err)
        {
            if (err is null) throw new ArgumentNullException(nameof(err));
            if (IsErr) err(_error);
            return this;
        }

        // ----------------------------
        // Interop: convert to string-error Result<T>
        // ----------------------------

        /// <summary>
        /// 型付きエラー Result を string エラー Result に落とす。
        /// </summary>
        public Result<T> ToStringError(Func<E, string> toMessage)
        {
            if (toMessage is null) throw new ArgumentNullException(nameof(toMessage));
            return IsOk ? Result<T>.Ok(_value) : Result<T>.Err(toMessage(_error));
        }
    }

    /// <summary>
    /// Result の関数型ユーティリティ（必要なら拡張で利用）。
    /// ※インスタンスメソッドで揃えつつ、"パイプ" 表記をしたい場合に使える。
    /// </summary>
    public static class ResultExtensions
    {
        // Result<T>
        public static Result<TResult> Map<T, TResult>(this Result<T> self, Func<T, TResult> map) => self.Map(map);
        public static Result<TResult> Bind<T, TResult>(this Result<T> self, Func<T, Result<TResult>> bind) => self.Bind(bind);

        // Result<T,E>
        public static Result<TResult, E> Map<T, TResult, E>(this Result<T, E> self, Func<T, TResult> map) => self.Map(map);
        public static Result<TResult, E> Bind<T, TResult, E>(this Result<T, E> self, Func<T, Result<TResult, E>> bind) => self.Bind(bind);
        public static Result<T, E2> MapError<T, E, E2>(this Result<T, E> self, Func<E, E2> mapError) => self.MapError(mapError);
    }
}