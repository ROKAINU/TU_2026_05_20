#nullable enable
using System;

namespace Game.Kernel
{
    /// <summary>
    /// 浮動小数点座標を表す ValueObject。
    /// Unity非依存。Domain/Application で安全に使用可能。
    /// </summary>
    [System.Serializable]
    public readonly struct Coord2D : IEquatable<Coord2D>
    {
        public readonly float x;
        public readonly float y;

        /// <summary>
        /// 原点 (0, 0)
        /// </summary>
        public static Coord2D Zero => new(0f, 0f);

        /// <summary>
        /// 初期化。
        /// </summary>
        public Coord2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        #region Equality & Hashing

        private const float Epsilon = 1e-6f; // 許容誤差

        /// <summary>
        /// 同値比較（許容誤差を考慮）。
        /// </summary>
        public override bool Equals(object? obj) => obj is Coord2D other && Equals(other);

        public bool Equals(Coord2D other) =>
            MathF.Abs(x - other.x) < Epsilon && MathF.Abs(y - other.y) < Epsilon;

        public override int GetHashCode() => HashCode.Combine(x, y);

        public static bool operator ==(Coord2D left, Coord2D right) => left.Equals(right);

        public static bool operator !=(Coord2D left, Coord2D right) => !left.Equals(right);

        #endregion

        #region Arithmetic

        /// <summary>
        /// ベクトル加算。
        /// </summary>
        public static Coord2D operator +(Coord2D a, Coord2D b) => new(a.x + b.x, a.y + b.y);

        /// <summary>
        /// ベクトル減算。
        /// </summary>
        public static Coord2D operator -(Coord2D a, Coord2D b) => new(a.x - b.x, a.y - b.y);

        /// <summary>
        /// スカラー倍。
        /// </summary>
        public static Coord2D operator *(Coord2D a, float scalar) => new(a.x * scalar, a.y * scalar);

        public static Coord2D operator *(float scalar, Coord2D a) => a * scalar;

        /// <summary>
        /// スカラー除算。
        /// </summary>
        public static Coord2D operator /(Coord2D a, float scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Coord2D division by zero.");

            return new(a.x / scalar, a.y / scalar);
        }

        /// <summary>
        /// 加法逆元（符号反転）。
        /// </summary>
        public static Coord2D operator -(Coord2D a) => new(-a.x, -a.y);

        #endregion

        #region Normalization

        /// <summary>
        /// ベクトルを正規化（方向を保ったまま、長さを1に）。
        /// ゼロベクトルの場合はゼロベクトルのままを返す。
        /// </summary>
        public Coord2D Normalize()
        {
            float magnitudeSquared = MagnitudeSquared;
            
            // ゼロベクトルの場合
            if (magnitudeSquared < Epsilon)
                return Zero;
            
            if (magnitudeSquared == 1f)
                return this; // すでに単位ベクトルならそのまま返す

            float magnitude = MathF.Sqrt(magnitudeSquared);
            return new Coord2D(x / magnitude, y / magnitude);
        }

        #endregion

        #region Distance & Magnitude

        /// <summary>
        /// ユークリッド距離（L2ノルム）。
        /// 直線距離。
        /// </summary>
        public float Distance(Coord2D other)
        {
            float dx = x - other.x;
            float dy = y - other.y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 二乗ユークリッド距離（√ を計算しない快速版）。
        /// </summary>
        public float SquaredDistance(Coord2D other)
        {
            float dx = x - other.x;
            float dy = y - other.y;
            return dx * dx + dy * dy;
        }

        /// <summary>
        /// 原点からの距離の二乗。
        /// </summary>
        public float MagnitudeSquared => x * x + y * y;

        #endregion

        #region Conversion

        /// <summary>
        /// int座標に変換。
        /// </summary>
        public Int2 ToInt2() => new((int)x, (int)y);

        #endregion

        #region String Representation

        public override string ToString() => $"({x}, {y})";

        #endregion

        #region Directions

        /// <summary>
        /// 主要4方向（上下左右）。
        /// </summary>
        public static class Directions4
        {
            public static readonly Coord2D Up = new(0f, 1f);
            public static readonly Coord2D Down = new(0f, -1f);
            public static readonly Coord2D Left = new(-1f, 0f);
            public static readonly Coord2D Right = new(1f, 0f);

            public static readonly Coord2D[] All = { Up, Down, Left, Right };
        }

        /// <summary>
        /// 主要8方向（上下左右 + 斜め）。
        /// </summary>
        public static class Directions8
        {
            public static readonly Coord2D Up = new(0f, 1f);
            public static readonly Coord2D Down = new(0f, -1f);
            public static readonly Coord2D Left = new(-1f, 0f);
            public static readonly Coord2D Right = new(1f, 0f);
            public static readonly Coord2D UpLeft = new(-1f, 1f);
            public static readonly Coord2D UpRight = new(1f, 1f);
            public static readonly Coord2D DownLeft = new(-1f, -1f);
            public static readonly Coord2D DownRight = new(1f, -1f);

            public static readonly Coord2D[] All = { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight };
        }

        #endregion
    }
}