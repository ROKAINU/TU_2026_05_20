#nullable enable
using System;
using System.Collections.Generic;

namespace Game.Kernel
{
    /// <summary>
    /// 値が「ある/ない」を null ではなく型で表すための Option。
    /// - Some(value): 値あり（null は不可）
    /// - None: 値なし
    ///
    /// 設計方針:
    /// - default(Option&lt;T&gt;) は None と同義（struct の既定値混入を許容する）
    /// - Some(null) は禁止（None と null を混同しない）
    /// - 例外を投げる Value だけに頼らないよう、TryGetValue / Match を提供する
    /// </summary>
    public readonly struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T _value;

        /// <summary>値を保持しているかどうか</summary>
        public bool IsSome { get; }

        /// <summary>値を保持していないかどうか</summary>
        public bool IsNone => !IsSome;

        private Option(T value, bool isSome)
        {
            _value = value;
            IsSome = isSome;
        }

        /// <summary>
        /// 値ありを生成する（null は不可）。
        /// </summary>
        public static Option<T> Some(T value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), "Some(value) に null は指定できません。None を使ってください。");

            return new Option<T>(value, isSome: true);
        }

        /// <summary>値なしを表す</summary>
        public static Option<T> None => default;

        /// <summary>
        /// Some の値を取り出す。None の場合は例外。
        /// 例外を避けたい場合は TryGetValue / Match を使用する。
        /// </summary>
        public T Value =>
            IsSome ? _value : throw new InvalidOperationException("Option は None です。Value を参照できません。");

        /// <summary>
        /// 値を安全に取り出す。
        /// </summary>
        public bool TryGetValue(out T value)
        {
            if (IsSome)
            {
                value = _value;
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// パターンマッチ風に分岐して値を取り出す。
        /// </summary>
        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (none is null) throw new ArgumentNullException(nameof(none));

            return IsSome ? some(_value) : none();
        }

        /// <summary>
        /// None の場合のみ fallback を返す。
        /// NOTE: Some(null) を禁止しているため、fallback にも null を渡さない運用を推奨。
        /// </summary>
        public T Or(T fallback)
        {
            if (fallback is null)
                throw new ArgumentNullException(nameof(fallback), "fallback に null は指定できません。");

            return IsSome ? _value : fallback;
        }

        /// <summary>
        /// None の場合のみ fallback を遅延生成する。
        /// </summary>
        public T OrElse(Func<T> fallbackFactory)
        {
            if (fallbackFactory is null) throw new ArgumentNullException(nameof(fallbackFactory));

            if (IsSome) return _value;

            var fallback = fallbackFactory();
            if (fallback is null)
                throw new InvalidOperationException("fallbackFactory が null を返しました。Option は null を値として扱いません。");

            return fallback;
        }

        public bool Equals(Option<T> other)
        {
            if (IsSome != other.IsSome) return false;
            if (!IsSome) return true; // 両方 None
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        public override bool Equals(object? obj) => obj is Option<T> other && Equals(other);

        public override int GetHashCode()
        {
            if (!IsSome) return 0;
            return EqualityComparer<T>.Default.GetHashCode(_value!);
        }

        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
        public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

        public override string ToString() => IsSome ? $"Some({_value})" : "None";
    }
}