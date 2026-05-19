#nullable enable
using System;
using System.Collections.Generic;

namespace Game.Kernel
{
    /// <summary>
    /// 整数座標を表す ValueObject。
    /// Unity非依存。Domain/Application で安全に使用可能。
    /// </summary>
    [System.Serializable]
    public readonly struct Int2 : IEquatable<Int2>
    {
        public readonly int x;
        public readonly int y;

        /// <summary>
        /// 原点 (0, 0)
        /// </summary>
        public static Int2 Zero => new(0, 0);

        /// <summary>
        /// 初期化。
        /// </summary>
        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        #region Equality & Hashing

        public override bool Equals(object? obj) => obj is Int2 other && Equals(other);

        public bool Equals(Int2 other) => x == other.x && y == other.y;

        public override int GetHashCode() => HashCode.Combine(x, y);

        public static bool operator ==(Int2 left, Int2 right) => left.Equals(right);

        public static bool operator !=(Int2 left, Int2 right) => !left.Equals(right);

        #endregion

        #region Arithmetic

        /// <summary>
        /// ベクトル加算。
        /// </summary>
        public static Int2 operator +(Int2 a, Int2 b) => new(a.x + b.x, a.y + b.y);

        /// <summary>
        /// ベクトル減算。
        /// </summary>
        public static Int2 operator -(Int2 a, Int2 b) => new(a.x - b.x, a.y - b.y);

        /// <summary>
        /// スカラー倍。
        /// </summary>
        public static Int2 operator *(Int2 a, int scalar) => new(a.x * scalar, a.y * scalar);

        public static Int2 operator *(int scalar, Int2 a) => a * scalar;

        /// <summary>
        /// スカラー除算。
        /// </summary>
        public static Int2 operator /(Int2 a, int scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Int2 division by zero.");

            return new(a.x / scalar, a.y / scalar);
        }

        /// <summary>
        /// 加法逆元（符号反転）。
        /// </summary>
        public static Int2 operator -(Int2 a) => new(-a.x, -a.y);

        #endregion

        #region Normalization

        /// <summary>
        /// ベクトルを正規化（方向を保ったまま、長さを1に）。
        /// ゼロベクトルの場合はゼロベクトルのままを返す。
        /// </summary>
        public Coord2D Normalize()
        {
            if (x == 0 && y == 0)
                return Coord2D.Zero;

            long magnitudeSquared = MagnitudeSquared;
            float magnitude = MathF.Sqrt(magnitudeSquared);
            return new Coord2D(x / magnitude, y / magnitude);
        }

        #endregion

        #region Distance & Magnitude

        /// <summary>
        /// マンハッタン距離（L1ノルム）。
        /// 直線移動のみの距離。
        /// </summary>
        public int ManhattanDistance(Int2 other)
        {
            return Math.Abs(x - other.x) + Math.Abs(y - other.y);
        }

        /// <summary>
        /// チェビシェフ距離（L∞ノルム、チェス盤距離）。
        /// 斜め移動を含めた最小移動数。
        /// </summary>
        public int ChebyshevDistance(Int2 other)
        {
            return Math.Max(Math.Abs(x - other.x), Math.Abs(y - other.y));
        }

        /// <summary>
        /// 二乗ユークリッド距離（√ を計算しない快速版）。
        /// </summary>
        public long SquaredEuclideanDistance(Int2 other)
        {
            long dx = x - other.x;
            long dy = y - other.y;
            return dx * dx + dy * dy;
        }

        /// <summary>
        /// 原点からの距離の二乗。
        /// </summary>
        public long MagnitudeSquared => (long)x * x + (long)y * y;

        #endregion

        #region Conversion

        /// <summary>
        /// float座標に変換。
        /// </summary>
        public Coord2D ToCoord2D() => new(x, y);

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
            public static readonly Int2 Up = new(0, 1);
            public static readonly Int2 Down = new(0, -1);
            public static readonly Int2 Left = new(-1, 0);
            public static readonly Int2 Right = new(1, 0);

            public static readonly Int2[] All = { Up, Down, Left, Right };
        }

        /// <summary>
        /// 主要8方向（上下左右 + 斜め）。
        /// </summary>
        public static class Directions8
        {
            public static readonly Int2 Up = new(0, 1);
            public static readonly Int2 Down = new(0, -1);
            public static readonly Int2 Left = new(-1, 0);
            public static readonly Int2 Right = new(1, 0);
            public static readonly Int2 UpLeft = new(-1, 1);
            public static readonly Int2 UpRight = new(1, 1);
            public static readonly Int2 DownLeft = new(-1, -1);
            public static readonly Int2 DownRight = new(1, -1);

            public static readonly Int2[] All = { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight };
        }

        #endregion
    }
}