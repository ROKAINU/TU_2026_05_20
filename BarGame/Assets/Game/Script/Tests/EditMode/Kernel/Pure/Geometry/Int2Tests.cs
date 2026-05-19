#nullable enable
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Game.Kernel;

namespace Game.Kernel.Tests
{
    [TestFixture]
    public class Int2Tests
    {
        /// <summary>
        /// 初期化テスト
        /// </summary>
        [Test]
        public void Constructor_WithValues_InitializesCorrectly()
        {
            var coord = new Int2(3, 4);
            Assert.That(coord.x, Is.EqualTo(3));
            Assert.That(coord.y, Is.EqualTo(4));
        }

        /// <summary>
        /// ゼロ定数のテスト
        /// </summary>
        [Test]
        public void Zero_ReturnsOrigin()
        {
            var zero = Int2.Zero;
            Assert.That(zero.x, Is.EqualTo(0));
            Assert.That(zero.y, Is.EqualTo(0));
        }

        /// <summary>
        /// 等値比較：完全に同じ値
        /// </summary>
        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            var a = new Int2(1, 2);
            var b = new Int2(1, 2);
            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
        }

        /// <summary>
        /// 等値比較：異なる値
        /// </summary>
        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var a = new Int2(1, 2);
            var b = new Int2(1, 3);
            Assert.That(a, Is.Not.EqualTo(b));
            Assert.That(a != b, Is.True);
        }

        /// <summary>
        /// 等値比較：負の座標
        /// </summary>
        [Test]
        public void Equals_NegativeCoordinates_ReturnsTrue()
        {
            var a = new Int2(-3, -4);
            var b = new Int2(-3, -4);
            Assert.That(a, Is.EqualTo(b));
        }

        /// <summary>
        /// 等値比較：object型での比較
        /// </summary>
        [Test]
        public void Equals_WithObjectType_ReturnsTrue()
        {
            var a = new Int2(5, 6);
            object b = new Int2(5, 6);
            Assert.That(a.Equals(b), Is.True);
        }

        /// <summary>
        /// 等値比較：nullとの比較
        /// </summary>
        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            var a = new Int2(1, 2);
            Assert.That(a.Equals(null), Is.False);
        }

        /// <summary>
        /// ハッシュ値：同じ値は同じハッシュ
        /// </summary>
        [Test]
        public void GetHashCode_SameValues_SameHash()
        {
            var a = new Int2(1, 2);
            var b = new Int2(1, 2);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        /// <summary>
        /// ハッシュ値：HashSet で使用可能か
        /// </summary>
        [Test]
        public void GetHashCode_UseInHashSet_Works()
        {
            var coord1 = new Int2(1, 2);
            var coord2 = new Int2(1, 2);
            var coord3 = new Int2(2, 3);

            var set = new HashSet<Int2> { coord1, coord2, coord3 };
            
            Assert.That(set.Count, Is.EqualTo(2));
            Assert.That(set.Contains(new Int2(1, 2)), Is.True);
            Assert.That(set.Contains(new Int2(2, 3)), Is.True);
        }

        /// <summary>
        /// 加算：基本的なベクトル加算
        /// </summary>
        [Test]
        public void Addition_TwoVectors_ReturnsSum()
        {
            var a = new Int2(1, 2);
            var b = new Int2(3, 4);
            var result = a + b;

            Assert.That(result, Is.EqualTo(new Int2(4, 6)));
        }

        /// <summary>
        /// 加算：負の値を含む
        /// </summary>
        [Test]
        public void Addition_WithNegativeValues_ReturnsCorrectSum()
        {
            var a = new Int2(5, 3);
            var b = new Int2(-2, -1);
            var result = a + b;

            Assert.That(result, Is.EqualTo(new Int2(3, 2)));
        }

        /// <summary>
        /// 加算：ゼロとの加算
        /// </summary>
        [Test]
        public void Addition_WithZero_ReturnsSameVector()
        {
            var a = new Int2(3, 4);
            var result = a + Int2.Zero;

            Assert.That(result, Is.EqualTo(a));
        }

        /// <summary>
        /// 減算：基本的なベクトル減算
        /// </summary>
        [Test]
        public void Subtraction_TwoVectors_ReturnsDifference()
        {
            var a = new Int2(5, 7);
            var b = new Int2(2, 3);
            var result = a - b;

            Assert.That(result, Is.EqualTo(new Int2(3, 4)));
        }

        /// <summary>
        /// 減算：自分自身からの引き算はゼロ
        /// </summary>
        [Test]
        public void Subtraction_SelfMinusSelf_ReturnsZero()
        {
            var a = new Int2(3, 4);
            var result = a - a;

            Assert.That(result, Is.EqualTo(Int2.Zero));
        }

        /// <summary>
        /// スカラー倍：正の値
        /// </summary>
        [Test]
        public void Multiplication_PositiveScalar_ReturnsScaledVector()
        {
            var a = new Int2(2, 3);
            var result = a * 2;

            Assert.That(result, Is.EqualTo(new Int2(4, 6)));
        }

        /// <summary>
        /// スカラー倍：逆順（scalar * vector）
        /// </summary>
        [Test]
        public void Multiplication_ScalarFirst_ReturnsScaledVector()
        {
            var a = new Int2(2, 3);
            var result = 3 * a;

            Assert.That(result, Is.EqualTo(new Int2(6, 9)));
        }

        /// <summary>
        /// スカラー倍：負の値
        /// </summary>
        [Test]
        public void Multiplication_NegativeScalar_ReturnsInvertedVector()
        {
            var a = new Int2(2, 3);
            var result = a * -1;

            Assert.That(result, Is.EqualTo(new Int2(-2, -3)));
        }

        /// <summary>
        /// スカラー倍：ゼロ倍
        /// </summary>
        [Test]
        public void Multiplication_ZeroScalar_ReturnsZero()
        {
            var a = new Int2(5, 7);
            var result = a * 0;

            Assert.That(result, Is.EqualTo(Int2.Zero));
        }

        /// <summary>
        /// スカラー除算：基本的な除算
        /// </summary>
        [Test]
        public void Division_PositiveScalar_ReturnsDividedVector()
        {
            var a = new Int2(6, 8);
            var result = a / 2;

            Assert.That(result, Is.EqualTo(new Int2(3, 4)));
        }

        /// <summary>
        /// スカラー除算：ゼロ除算例外
        /// </summary>
        [Test]
        public void Division_ZeroScalar_ThrowsDivideByZeroException()
        {
            var a = new Int2(1, 2);
            Assert.Throws<DivideByZeroException>(() =>
            {
                var _ = a / 0;
            });
        }

        /// <summary>
        /// スカラー除算：負の値
        /// </summary>
        [Test]
        public void Division_NegativeScalar_ReturnsDividedVector()
        {
            var a = new Int2(6, 8);
            var result = a / -2;

            Assert.That(result, Is.EqualTo(new Int2(-3, -4)));
        }

        /// <summary>
        /// 符号反転：正の値
        /// </summary>
        [Test]
        public void Negate_PositiveVector_ReturnsNegated()
        {
            var a = new Int2(3, 4);
            var result = -a;

            Assert.That(result, Is.EqualTo(new Int2(-3, -4)));
        }

        /// <summary>
        /// 符号反転：二重反転
        /// </summary>
        [Test]
        public void Negate_Twice_ReturnsSameVector()
        {
            var a = new Int2(3, 4);
            var result = -(-a);

            Assert.That(result, Is.EqualTo(a));
        }

        /// <summary>
        /// 正規化：基本的なベクトルの正規化
        /// </summary>
        [Test]
        public void Normalize_WithValidVector_ReturnsNormalizedVector()
        {
            var vector = new Int2(3, 4);  // 長さ5のベクトル
            const float epsilon = 1e-6f;

            var normalized = vector.Normalize();

            Assert.That(MathF.Abs(normalized.x - 0.6f), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized.y - 0.8f), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：単位ベクトルはそのまま返ること
        /// </summary>
        [Test]
        public void Normalize_WithUnitVector_ReturnsItself()
        {
            var vector = new Int2(1, 0);
            const float epsilon = 1e-6f;

            var normalized = vector.Normalize();

            Assert.That(MathF.Abs(normalized.x - 1f), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized.y - 0f), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：斜めベクトルの正規化
        /// </summary>
        [Test]
        public void Normalize_WithDiagonalVector_ReturnsCorrectDirection()
        {
            var vector = new Int2(1, 1);
            const float epsilon = 1e-6f;
            const float expected = MathF.PI / 4;  // 45度

            var normalized = vector.Normalize();

            float expectedX = MathF.Cos(expected);
            float expectedY = MathF.Sin(expected);
            Assert.That(MathF.Abs(normalized.x - expectedX), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized.y - expectedY), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：負のベクトルの正規化
        /// </summary>
        [Test]
        public void Normalize_WithNegativeVector_ReturnsNormalizedNegativeVector()
        {
            var vector = new Int2(-3, -4);
            const float epsilon = 1e-6f;

            var normalized = vector.Normalize();

            Assert.That(MathF.Abs(normalized.x - (-0.6f)), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized.y - (-0.8f)), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：大きなベクトルの正規化
        /// </summary>
        [Test]
        public void Normalize_WithZeroVector_ReturnsZero()
        {
            var vector = new Int2(0, 0);

            var normalized = vector.Normalize();

            Assert.AreEqual(Coord2D.Zero, normalized);
        }

        /// <summary>
        /// 正規化：大きなベクトルの正規化
        /// </summary>
        [Test]
        public void Normalize_ResultHasUnitMagnitude()
        {
            var vector = new Int2(5, 12);  // 長さ13のベクトル
            const float epsilon = 1e-5f;

            var normalized = vector.Normalize();
            float magnitude = MathF.Sqrt(normalized.x * normalized.x + normalized.y * normalized.y);

            Assert.That(MathF.Abs(magnitude - 1f), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：大きなベクトルの正規化
        /// </summary>
        [Test]
        public void Normalize_WithLargeVector_ReturnsCorrectNormalization()
        {
            var vector = new Int2(300, 400);  // 長さ500のベクトル
            const float epsilon = 1e-6f;

            var normalized = vector.Normalize();

            Assert.That(MathF.Abs(normalized.x - 0.6f), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized.y - 0.8f), Is.LessThan(epsilon));
        }

        /// <summary>
        /// マンハッタン距離：基本的な計算
        /// </summary>
        [Test]
        public void ManhattanDistance_Basic_ReturnsCorrectDistance()
        {
            var from = new Int2(0, 0);
            var to = new Int2(3, 4);
            var distance = from.ManhattanDistance(to);

            Assert.That(distance, Is.EqualTo(7));
        }

        /// <summary>
        /// マンハッタン距離：同一点
        /// </summary>
        [Test]
        public void ManhattanDistance_SamePoint_ReturnsZero()
        {
            var a = new Int2(3, 4);
            var distance = a.ManhattanDistance(a);

            Assert.That(distance, Is.EqualTo(0));
        }

        /// <summary>
        /// マンハッタン距離：対称性
        /// </summary>
        [Test]
        public void ManhattanDistance_IsSymmetric()
        {
            var a = new Int2(1, 2);
            var b = new Int2(4, 6);

            Assert.That(a.ManhattanDistance(b), Is.EqualTo(b.ManhattanDistance(a)));
        }

        /// <summary>
        /// マンハッタン距離：負の座標
        /// </summary>
        [Test]
        public void ManhattanDistance_NegativeCoordinates_ReturnsCorrectDistance()
        {
            var a = new Int2(-3, -4);
            var b = new Int2(0, 0);
            var distance = a.ManhattanDistance(b);

            Assert.That(distance, Is.EqualTo(7));
        }

        /// <summary>
        /// チェビシェフ距離：基本的な計算
        /// </summary>
        [Test]
        public void ChebyshevDistance_Basic_ReturnsCorrectDistance()
        {
            var from = new Int2(0, 0);
            var to = new Int2(3, 4);
            var distance = from.ChebyshevDistance(to);

            Assert.That(distance, Is.EqualTo(4));
        }

        /// <summary>
        /// チェビシェフ距離：同一点
        /// </summary>
        [Test]
        public void ChebyshevDistance_SamePoint_ReturnsZero()
        {
            var a = new Int2(5, 6);
            var distance = a.ChebyshevDistance(a);

            Assert.That(distance, Is.EqualTo(0));
        }

        /// <summary>
        /// チェビシェフ距離：対称性
        /// </summary>
        [Test]
        public void ChebyshevDistance_IsSymmetric()
        {
            var a = new Int2(1, 2);
            var b = new Int2(4, 7);

            Assert.That(a.ChebyshevDistance(b), Is.EqualTo(b.ChebyshevDistance(a)));
        }

        /// <summary>
        /// チェビシェフ距離：水平線
        /// </summary>
        [Test]
        public void ChebyshevDistance_HorizontalLine_ReturnsDeltaX()
        {
            var from = new Int2(0, 5);
            var to = new Int2(3, 5);
            var distance = from.ChebyshevDistance(to);

            Assert.That(distance, Is.EqualTo(3));
        }

        /// <summary>
        /// チェビシェフ距離：垂直線
        /// </summary>
        [Test]
        public void ChebyshevDistance_VerticalLine_ReturnsDeltaY()
        {
            var from = new Int2(5, 0);
            var to = new Int2(5, 3);
            var distance = from.ChebyshevDistance(to);

            Assert.That(distance, Is.EqualTo(3));
        }

        /// <summary>
        /// 二乗ユークリッド距離：基本的な計算
        /// </summary>
        [Test]
        public void SquaredEuclideanDistance_3_4_5Triangle_Returns25()
        {
            var from = new Int2(0, 0);
            var to = new Int2(3, 4);
            var squaredDistance = from.SquaredEuclideanDistance(to);

            Assert.That(squaredDistance, Is.EqualTo(25));
        }

        /// <summary>
        /// 二乗ユークリッド距離：同一点
        /// </summary>
        [Test]
        public void SquaredEuclideanDistance_SamePoint_ReturnsZero()
        {
            var a = new Int2(3, 4);
            var squaredDistance = a.SquaredEuclideanDistance(a);

            Assert.That(squaredDistance, Is.EqualTo(0));
        }

        /// <summary>
        /// 二乗ユークリッド距離：長い距離（long型対応）
        /// </summary>
        [Test]
        public void SquaredEuclideanDistance_LargeDistance_ReturnsLongValue()
        {
            var from = new Int2(0, 0);
            var to = new Int2(100000, 100000);
            var squaredDistance = from.SquaredEuclideanDistance(to);

            long expected = 100000L * 100000L + 100000L * 100000L;
            Assert.That(squaredDistance, Is.EqualTo(expected));
        }

        /// <summary>
        /// 二乗ユークリッド距離：対称性
        /// </summary>
        [Test]
        public void SquaredEuclideanDistance_IsSymmetric()
        {
            var a = new Int2(1, 2);
            var b = new Int2(4, 6);

            Assert.That(a.SquaredEuclideanDistance(b), Is.EqualTo(b.SquaredEuclideanDistance(a)));
        }

        /// <summary>
        /// マグニチュードの二乗：基本的な計算
        /// </summary>
        [Test]
        public void MagnitudeSquared_Returns_X2_Plus_Y2()
        {
            var a = new Int2(3, 4);
            var magnitudeSquared = a.MagnitudeSquared;

            Assert.That(magnitudeSquared, Is.EqualTo(25));
        }

        /// <summary>
        /// マグニチュードの二乗：原点
        /// </summary>
        [Test]
        public void MagnitudeSquared_Zero_ReturnsZero()
        {
            var zero = Int2.Zero;
            Assert.That(zero.MagnitudeSquared, Is.EqualTo(0));
        }

        /// <summary>
        /// マグニチュードの二乗：long型対応
        /// </summary>
        [Test]
        public void MagnitudeSquared_LargeValues_ReturnsLongValue()
        {
            var a = new Int2(100000, 100000);
            var magnitudeSquared = a.MagnitudeSquared;

            long expected = 100000L * 100000L + 100000L * 100000L;
            Assert.That(magnitudeSquared, Is.EqualTo(expected));
        }

        /// <summary>
        /// Coord2Dへの変換：基本的な変換
        /// </summary>
        [Test]
        public void ToCoord2D_ConvertsCorrectly()
        {
            var intCoord = new Int2(3, 4);
            var coord2D = intCoord.ToCoord2D();

            Assert.That(coord2D.x, Is.EqualTo(3f));
            Assert.That(coord2D.y, Is.EqualTo(4f));
        }

        /// <summary>
        /// Coord2Dへの変換：負の座標
        /// </summary>
        [Test]
        public void ToCoord2D_NegativeValues_ConvertsCorrectly()
        {
            var intCoord = new Int2(-3, -4);
            var coord2D = intCoord.ToCoord2D();

            Assert.That(coord2D.x, Is.EqualTo(-3f));
            Assert.That(coord2D.y, Is.EqualTo(-4f));
        }

        /// <summary>
        /// ToString：基本的なフォーマット
        /// </summary>
        [Test]
        public void ToString_ReturnsFormattedString()
        {
            var coord = new Int2(3, 4);
            var str = coord.ToString();

            Assert.That(str, Contains.Substring("3"));
            Assert.That(str, Contains.Substring("4"));
        }

        /// <summary>
        /// 方向4：Up定数
        /// </summary>
        [Test]
        public void Directions4_Up_ReturnsCorrectVector()
        {
            Assert.That(Int2.Directions4.Up, Is.EqualTo(new Int2(0, 1)));
        }

        /// <summary>
        /// 方向4：Down定数
        /// </summary>
        [Test]
        public void Directions4_Down_ReturnsCorrectVector()
        {
            Assert.That(Int2.Directions4.Down, Is.EqualTo(new Int2(0, -1)));
        }

        /// <summary>
        /// 方向4：Left定数
        /// </summary>
        [Test]
        public void Directions4_Left_ReturnsCorrectVector()
        {
            Assert.That(Int2.Directions4.Left, Is.EqualTo(new Int2(-1, 0)));
        }

        /// <summary>
        /// 方向4：Right定数
        /// </summary>
        [Test]
        public void Directions4_Right_ReturnsCorrectVector()
        {
            Assert.That(Int2.Directions4.Right, Is.EqualTo(new Int2(1, 0)));
        }

        /// <summary>
        /// 方向4：All配列
        /// </summary>
        [Test]
        public void Directions4_All_Contains4Elements()
        {
            Assert.That(Int2.Directions4.All.Length, Is.EqualTo(4));
            Assert.That(Int2.Directions4.All, Contains.Item(Int2.Directions4.Up));
            Assert.That(Int2.Directions4.All, Contains.Item(Int2.Directions4.Down));
            Assert.That(Int2.Directions4.All, Contains.Item(Int2.Directions4.Left));
            Assert.That(Int2.Directions4.All, Contains.Item(Int2.Directions4.Right));
        }

        /// <summary>
        /// 方向8：すべての斜めを含む
        /// </summary>
        [Test]
        public void Directions8_All_Contains8Elements()
        {
            Assert.That(Int2.Directions8.All.Length, Is.EqualTo(8));
        }

        /// <summary>
        /// 方向8：斜め方向の確認
        /// </summary>
        [Test]
        public void Directions8_Diagonal_ReturnsCorrectVectors()
        {
            Assert.That(Int2.Directions8.UpLeft, Is.EqualTo(new Int2(-1, 1)));
            Assert.That(Int2.Directions8.UpRight, Is.EqualTo(new Int2(1, 1)));
            Assert.That(Int2.Directions8.DownLeft, Is.EqualTo(new Int2(-1, -1)));
            Assert.That(Int2.Directions8.DownRight, Is.EqualTo(new Int2(1, -1)));
        }

        /// <summary>
        /// 距離比較：マンハッタン距離とチェビシェフ距離の関係
        /// </summary>
        [Test]
        public void DistanceComparison_Manhattan_GreaterOrEqual_Chebyshev()
        {
            var a = new Int2(0, 0);
            var b = new Int2(3, 4);

            var manhattan = a.ManhattanDistance(b);
            var chebyshev = a.ChebyshevDistance(b);

            // マンハッタン距離 >= チェビシェフ距離
            Assert.That(manhattan, Is.GreaterThanOrEqualTo(chebyshev));
        }

        /// <summary>
        /// 複合演算：(a + b) * 2 - c
        /// </summary>
        [Test]
        public void ComplexOperation_MultipleOperations_ReturnsCorrectResult()
        {
            var a = new Int2(1, 2);
            var b = new Int2(3, 4);
            var c = new Int2(2, 3);

            var result = (a + b) * 2 - c;

            var expected = new Int2(6, 9);
            Assert.That(result, Is.EqualTo(expected));
        }

        /// <summary>
        /// 方向移動：位置 + 方向 * 距離
        /// </summary>
        [Test]
        public void DirectionalMovement_Position_Plus_Direction_Times_Distance()
        {
            var position = new Int2(5, 5);
            var distance = 3;
            var newPosition = position + Int2.Directions4.Right * distance;

            Assert.That(newPosition, Is.EqualTo(new Int2(8, 5)));
        }

        /// <summary>
        /// 8方向への移動シミュレーション
        /// </summary>
        [Test]
        public void Directions8_AllDirections_CanMoveInAllDirections()
        {
            var origin = new Int2(5, 5);

            foreach (var direction in Int2.Directions8.All)
            {
                var newPos = origin + direction;
                
                // チェビシェフ距離が1であることを確認
                Assert.That(origin.ChebyshevDistance(newPos), Is.EqualTo(1));
            }
        }

        /// <summary>
        /// 負のint最大値での計算（オーバーフロー回避）
        /// </summary>
        [Test]
        public void SquaredEuclideanDistance_MaxIntValues_DoesNotOverflow()
        {
            var a = new Int2(int.MaxValue, int.MaxValue);
            var b = new Int2(int.MinValue, int.MinValue);

            // long型なので、オーバーフローしない
            var squaredDistance = a.SquaredEuclideanDistance(b);
            Assert.That(squaredDistance, Is.GreaterThan(0));
        }
    }
}