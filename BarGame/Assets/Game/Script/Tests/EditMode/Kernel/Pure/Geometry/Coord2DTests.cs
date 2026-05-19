#nullable enable
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Game.Kernel;

namespace Game.Kernel.Tests
{
    [TestFixture]
    public class Coord2DTests
    {
        /// <summary>
        /// 初期化テスト
        /// </summary>
        [Test]
        public void Constructor_WithValues_InitializesCorrectly()
        {
            var coord = new Coord2D(3.5f, 4.5f);
            Assert.That(coord.x, Is.EqualTo(3.5f));
            Assert.That(coord.y, Is.EqualTo(4.5f));
        }

        /// <summary>
        /// ゼロ定数のテスト
        /// </summary>
        [Test]
        public void Zero_ReturnsOrigin()
        {
            var zero = Coord2D.Zero;
            Assert.That(zero.x, Is.EqualTo(0f));
            Assert.That(zero.y, Is.EqualTo(0f));
        }

        /// <summary>
        /// 等値比較：完全に同じ値
        /// </summary>
        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            var a = new Coord2D(1f, 2f);
            var b = new Coord2D(1f, 2f);
            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
        }

        /// <summary>
        /// 等値比較：異なる値
        /// </summary>
        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var a = new Coord2D(1f, 2f);
            var b = new Coord2D(1f, 3f);
            Assert.That(a, Is.Not.EqualTo(b));
            Assert.That(a != b, Is.True);
        }

        /// <summary>
        /// 等値比較：許容誤差内の違い
        /// </summary>
        [Test]
        public void Equals_WithinEpsilon_ReturnsTrue()
        {
            var a = new Coord2D(1f, 2f);
            var b = new Coord2D(1f + 1e-7f, 2f + 1e-7f);
            Assert.That(a, Is.EqualTo(b));
        }

        /// <summary>
        /// 等値比較：許容誤差を超える違い
        /// </summary>
        [Test]
        public void Equals_BeyondEpsilon_ReturnsFalse()
        {
            var a = new Coord2D(1f, 2f);
            var b = new Coord2D(1f + 1e-5f, 2f);
            Assert.That(a, Is.Not.EqualTo(b));
        }

        /// <summary>
        /// 等値比較：負の座標
        /// </summary>
        [Test]
        public void Equals_NegativeCoordinates_ReturnsTrue()
        {
            var a = new Coord2D(-3.5f, -4.5f);
            var b = new Coord2D(-3.5f, -4.5f);
            Assert.That(a, Is.EqualTo(b));
        }

        /// <summary>
        /// ハッシュ値：同じ値は同じハッシュ
        /// </summary>
        [Test]
        public void GetHashCode_SameValues_SameHash()
        {
            var a = new Coord2D(1f, 2f);
            var b = new Coord2D(1f, 2f);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        /// <summary>
        /// ハッシュ値：HashSet で使用可能か
        /// </summary>
        [Test]
        public void GetHashCode_UseInHashSet_Works()
        {
            var coord1 = new Coord2D(1f, 2f);
            var coord2 = new Coord2D(1f, 2f);
            var coord3 = new Coord2D(2f, 3f);

            var set = new HashSet<Coord2D> { coord1, coord2, coord3 };
            
            // coord1 と coord2 は同じなので、セットには2個の要素がある
            Assert.That(set.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// 加算：基本的なベクトル加算
        /// </summary>
        [Test]
        public void Addition_TwoVectors_ReturnsSum()
        {
            var a = new Coord2D(1f, 2f);
            var b = new Coord2D(3f, 4f);
            var result = a + b;

            Assert.That(result.x, Is.EqualTo(4f));
            Assert.That(result.y, Is.EqualTo(6f));
        }

        /// <summary>
        /// 加算：負の値を含む
        /// </summary>
        [Test]
        public void Addition_WithNegativeValues_ReturnsCorrectSum()
        {
            var a = new Coord2D(5f, 3f);
            var b = new Coord2D(-2f, -1f);
            var result = a + b;

            Assert.That(result, Is.EqualTo(new Coord2D(3f, 2f)));
        }

        /// <summary>
        /// 減算：基本的なベクトル減算
        /// </summary>
        [Test]
        public void Subtraction_TwoVectors_ReturnsDifference()
        {
            var a = new Coord2D(5f, 7f);
            var b = new Coord2D(2f, 3f);
            var result = a - b;

            Assert.That(result.x, Is.EqualTo(3f));
            Assert.That(result.y, Is.EqualTo(4f));
        }

        /// <summary>
        /// 減算：自分自身からの引き算はゼロ
        /// </summary>
        [Test]
        public void Subtraction_SelfMinusSelf_ReturnsZero()
        {
            var a = new Coord2D(3.5f, 4.5f);
            var result = a - a;

            Assert.That(result, Is.EqualTo(Coord2D.Zero));
        }

        /// <summary>
        /// スカラー倍：正の値
        /// </summary>
        [Test]
        public void Multiplication_PositiveScalar_ReturnsScaledVector()
        {
            var a = new Coord2D(2f, 3f);
            var result = a * 2f;

            Assert.That(result.x, Is.EqualTo(4f));
            Assert.That(result.y, Is.EqualTo(6f));
        }

        /// <summary>
        /// スカラー倍：逆順（scalar * vector）
        /// </summary>
        [Test]
        public void Multiplication_ScalarFirst_ReturnsScaledVector()
        {
            var a = new Coord2D(2f, 3f);
            var result = 3f * a;

            Assert.That(result.x, Is.EqualTo(6f));
            Assert.That(result.y, Is.EqualTo(9f));
        }

        /// <summary>
        /// スカラー倍：負の値
        /// </summary>
        [Test]
        public void Multiplication_NegativeScalar_ReturnsInvertedVector()
        {
            var a = new Coord2D(2f, 3f);
            var result = a * -1f;

            Assert.That(result.x, Is.EqualTo(-2f));
            Assert.That(result.y, Is.EqualTo(-3f));
        }

        /// <summary>
        /// スカラー倍：ゼロ倍
        /// </summary>
        [Test]
        public void Multiplication_ZeroScalar_ReturnsZero()
        {
            var a = new Coord2D(5f, 7f);
            var result = a * 0f;

            Assert.That(result, Is.EqualTo(Coord2D.Zero));
        }

        /// <summary>
        /// スカラー除算：基本的な除算
        /// </summary>
        [Test]
        public void Division_PositiveScalar_ReturnsDividedVector()
        {
            var a = new Coord2D(6f, 9f);
            var result = a / 3f;

            Assert.That(result.x, Is.EqualTo(2f));
            Assert.That(result.y, Is.EqualTo(3f));
        }

        /// <summary>
        /// スカラー除算：ゼロ除算例外
        /// </summary>
        [Test]
        public void Division_ZeroScalar_ThrowsDivideByZeroException()
        {
            var a = new Coord2D(1f, 2f);
            Assert.Throws<DivideByZeroException>(() =>
            {
                var _ = a / 0f;
            });
        }

        /// <summary>
        /// 符号反転：正の値
        /// </summary>
        [Test]
        public void Negate_PositiveVector_ReturnsNegated()
        {
            var a = new Coord2D(3f, 4f);
            var result = -a;

            Assert.That(result.x, Is.EqualTo(-3f));
            Assert.That(result.y, Is.EqualTo(-4f));
        }

        /// <summary>
        /// 符号反転：二重反転
        /// </summary>
        [Test]
        public void Negate_Twice_ReturnsSameVector()
        {
            var a = new Coord2D(3f, 4f);
            var result = -(-a);

            Assert.That(result, Is.EqualTo(a));
        }

        /// <summary>
        /// 正規化：基本的なベクトルの正規化
        /// </summary>
        [Test]
        public void Normalize_WithValidVector_ReturnsNormalizedVector()
        {
            var vector = new Coord2D(3f, 4f);  // 長さ5のベクトル
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
            var vector = new Coord2D(1f, 0f);
            const float epsilon = 1e-6f;

            var normalized = vector.Normalize();

            Assert.That(MathF.Abs(normalized.x - 1f), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized.y - 0f), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：斜めベクトルの正規化が正しい方向を返すこと
        /// </summary>
        [Test]
        public void Normalize_WithDiagonalVector_ReturnsCorrectDirection()
        {
            var vector = new Coord2D(1f, 1f);
            const float epsilon = 1e-6f;
            const float expected = MathF.PI / 4;  // 45度

            var normalized = vector.Normalize();

            float expectedX = MathF.Cos(expected);
            float expectedY = MathF.Sin(expected);
            Assert.That(MathF.Abs(normalized.x - expectedX), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized.y - expectedY), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：負のベクトルも正しい結果を返すこと
        /// </summary>
        [Test]
        public void Normalize_WithNegativeVector_ReturnsNormalizedNegativeVector()
        {
            var vector = new Coord2D(-3f, -4f);
            const float epsilon = 1e-6f;

            var normalized = vector.Normalize();

            Assert.That(MathF.Abs(normalized.x - (-0.6f)), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized.y - (-0.8f)), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：ゼロベクトルはゼロのまま返すこと
        /// </summary>
        [Test]
        public void Normalize_WithZeroVector_ReturnsZero()
        {
            var vector = new Coord2D(0f, 0f);

            var normalized = vector.Normalize();

            Assert.AreEqual(Coord2D.Zero, normalized);
        }

        /// <summary>
        /// 正規化：非常に小さなベクトルはゼロベクトルになること
        /// </summary>
        [Test]
        public void Normalize_WithVerySmallVector_ReturnsZero()
        {
            var vector = new Coord2D(1e-7f, 1e-7f);// Epsilonより小さいベクトル

            var normalized = vector.Normalize();

            Assert.AreEqual(Coord2D.Zero, normalized);
        }

        /// <summary>
        /// 正規化：結果の大きさが1になること
        /// </summary>
        [Test]
        public void Normalize_ResultHasUnitMagnitude()
        {
            var vector = new Coord2D(5f, 12f);  // 長さ13のベクトル
            const float epsilon = 1e-5f;

            var normalized = vector.Normalize();
            float magnitude = MathF.Sqrt(normalized.x * normalized.x + normalized.y * normalized.y);

            Assert.That(MathF.Abs(magnitude - 1f), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：大きな値での正規化が正しい結果を返すか
        /// </summary>
        [Test]
        public void Normalize_WithLargeVector_ReturnsCorrectNormalization()
        {
            var vector = new Coord2D(300f, 400f);  // 長さ500のベクトル
            const float epsilon = 1e-6f;

            var normalized = vector.Normalize();

            Assert.That(MathF.Abs(normalized.x - 0.6f), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized.y - 0.8f), Is.LessThan(epsilon));
        }

        /// <summary>
        /// 正規化：方向を保つ（反対方向のベクトルも正規化後は反対になる）
        /// </summary>
        [Test]
        public void Normalize_PreservesDirection_OppositeVectors()
        {
            var vectorA = new Coord2D(3f, 4f);
            var vectorB = new Coord2D(-3f, -4f);

            var normalizedA = vectorA.Normalize();
            var normalizedB = vectorB.Normalize();

            Assert.That(normalizedA.x, Is.EqualTo(-normalizedB.x).Within(1e-6f));
            Assert.That(normalizedA.y, Is.EqualTo(-normalizedB.y).Within(1e-6f));
        }

        /// <summary>
        /// 正規化：複数回の正規化は同じ結果になる（冪等性）
        /// </summary>
        [Test]
        public void Normalize_MultipleNormalizations_AreIdempotent()
        {
            var vector = new Coord2D(7f, 9f);
            const float epsilon = 1e-6f;

            var normalized1 = vector.Normalize();
            var normalized2 = normalized1.Normalize();

            Assert.That(MathF.Abs(normalized1.x - normalized2.x), Is.LessThan(epsilon));
            Assert.That(MathF.Abs(normalized1.y - normalized2.y), Is.LessThan(epsilon));
        }

        /// <summary>
        /// ユークリッド距離：基本的な計算
        /// </summary>
        [Test]
        public void Distance_3_4_5Triangle_Returns5()
        {
            var from = new Coord2D(0f, 0f);
            var to = new Coord2D(3f, 4f);
            var distance = from.Distance(to);

            Assert.That(distance, Is.EqualTo(5f).Within(1e-5f));
        }

        /// <summary>
        /// ユークリッド距離：同一点
        /// </summary>
        [Test]
        public void Distance_SamePoint_ReturnsZero()
        {
            var a = new Coord2D(3.5f, 4.5f);
            var distance = a.Distance(a);

            Assert.That(distance, Is.EqualTo(0f).Within(1e-6f));
        }

        /// <summary>
        /// ユークリッド距離：対称性
        /// </summary>
        [Test]
        public void Distance_IsSymmetric()
        {
            var a = new Coord2D(1f, 2f);
            var b = new Coord2D(4f, 6f);

            Assert.That(a.Distance(b), Is.EqualTo(b.Distance(a)).Within(1e-6f));
        }

        /// <summary>
        /// ユークリッド距離：負の座標
        /// </summary>
        [Test]
        public void Distance_NegativeCoordinates_ReturnsCorrectDistance()
        {
            var a = new Coord2D(-3f, -4f);
            var b = new Coord2D(0f, 0f);
            var distance = a.Distance(b);

            Assert.That(distance, Is.EqualTo(5f).Within(1e-5f));
        }

        /// <summary>
        /// 二乗ユークリッド距離：基本的な計算
        /// </summary>
        [Test]
        public void SquaredDistance_3_4_5Triangle_Returns25()
        {
            var from = new Coord2D(0f, 0f);
            var to = new Coord2D(3f, 4f);
            var squaredDistance = from.SquaredDistance(to);

            Assert.That(squaredDistance, Is.EqualTo(25f).Within(1e-5f));
        }

        /// <summary>
        /// 二乗ユークリッド距離は距離の二乗
        /// </summary>
        [Test]
        public void SquaredDistance_EqualsDistanceSquared()
        {
            var a = new Coord2D(2f, 3f);
            var b = new Coord2D(5f, 7f);

            var distance = a.Distance(b);
            var squaredDistance = a.SquaredDistance(b);

            Assert.That(squaredDistance, Is.EqualTo(distance * distance).Within(1e-5f));
        }

        /// <summary>
        /// マグニチュードの二乗：基本的な計算
        /// </summary>
        [Test]
        public void MagnitudeSquared_Returns_X2_Plus_Y2()
        {
            var a = new Coord2D(3f, 4f);
            var magnitudeSquared = a.MagnitudeSquared;

            Assert.That(magnitudeSquared, Is.EqualTo(25f));
        }

        /// <summary>
        /// マグニチュードの二乗：原点
        /// </summary>
        [Test]
        public void MagnitudeSquared_Zero_ReturnsZero()
        {
            var zero = Coord2D.Zero;
            Assert.That(zero.MagnitudeSquared, Is.EqualTo(0f));
        }

        /// <summary>
        /// Int2への変換：正の値
        /// </summary>
        [Test]
        public void ToInt2_PositiveValues_TruncatesCorrectly()
        {
            var coord = new Coord2D(3.7f, 4.2f);
            var intCoord = coord.ToInt2();

            Assert.That(intCoord.x, Is.EqualTo(3));
            Assert.That(intCoord.y, Is.EqualTo(4));
        }

        /// <summary>
        /// Int2への変換：負の値
        /// </summary>
        [Test]
        public void ToInt2_NegativeValues_TruncatesCorrectly()
        {
            var coord = new Coord2D(-3.7f, -4.2f);
            var intCoord = coord.ToInt2();

            Assert.That(intCoord.x, Is.EqualTo(-3));
            Assert.That(intCoord.y, Is.EqualTo(-4));
        }

        /// <summary>
        /// ToString：基本的なフォーマット
        /// </summary>
        [Test]
        public void ToString_ReturnsFormattedString()
        {
            var coord = new Coord2D(3.5f, 4.5f);
            var str = coord.ToString();

            Assert.That(str, Contains.Substring("3.5"));
            Assert.That(str, Contains.Substring("4.5"));
        }

        /// <summary>
        /// 方向4：Up定数
        /// </summary>
        [Test]
        public void Directions4_Up_ReturnsCorrectVector()
        {
            Assert.That(Coord2D.Directions4.Up, Is.EqualTo(new Coord2D(0f, 1f)));
        }

        /// <summary>
        /// 方向4：Down定数
        /// </summary>
        [Test]
        public void Directions4_Down_ReturnsCorrectVector()
        {
            Assert.That(Coord2D.Directions4.Down, Is.EqualTo(new Coord2D(0f, -1f)));
        }

        /// <summary>
        /// 方向4：Left定数
        /// </summary>
        [Test]
        public void Directions4_Left_ReturnsCorrectVector()
        {
            Assert.That(Coord2D.Directions4.Left, Is.EqualTo(new Coord2D(-1f, 0f)));
        }

        /// <summary>
        /// 方向4：Right定数
        /// </summary>
        [Test]
        public void Directions4_Right_ReturnsCorrectVector()
        {
            Assert.That(Coord2D.Directions4.Right, Is.EqualTo(new Coord2D(1f, 0f)));
        }

        /// <summary>
        /// 方向4：All配列
        /// </summary>
        [Test]
        public void Directions4_All_Contains4Elements()
        {
            Assert.That(Coord2D.Directions4.All.Length, Is.EqualTo(4));
            Assert.That(Coord2D.Directions4.All, Contains.Item(Coord2D.Directions4.Up));
            Assert.That(Coord2D.Directions4.All, Contains.Item(Coord2D.Directions4.Down));
            Assert.That(Coord2D.Directions4.All, Contains.Item(Coord2D.Directions4.Left));
            Assert.That(Coord2D.Directions4.All, Contains.Item(Coord2D.Directions4.Right));
        }

        /// <summary>
        /// 方向8：すべての斜めを含む
        /// </summary>
        [Test]
        public void Directions8_All_Contains8Elements()
        {
            Assert.That(Coord2D.Directions8.All.Length, Is.EqualTo(8));
        }

        /// <summary>
        /// 方向8：斜め方向の確認
        /// </summary>
        [Test]
        public void Directions8_Diagonal_ReturnsCorrectVectors()
        {
            Assert.That(Coord2D.Directions8.UpLeft, Is.EqualTo(new Coord2D(-1f, 1f)));
            Assert.That(Coord2D.Directions8.UpRight, Is.EqualTo(new Coord2D(1f, 1f)));
            Assert.That(Coord2D.Directions8.DownLeft, Is.EqualTo(new Coord2D(-1f, -1f)));
            Assert.That(Coord2D.Directions8.DownRight, Is.EqualTo(new Coord2D(1f, -1f)));
        }

        /// <summary>
        /// 複合演算：(a + b) * 2 - c
        /// </summary>
        [Test]
        public void ComplexOperation_MultipleOperations_ReturnsCorrectResult()
        {
            var a = new Coord2D(1f, 2f);
            var b = new Coord2D(3f, 4f);
            var c = new Coord2D(2f, 3f);

            var result = (a + b) * 2f - c;

            var expected = new Coord2D(6f, 9f);
            Assert.That(result, Is.EqualTo(expected));
        }

        /// <summary>
        /// 方向移動：位置 + 方向 * 距離
        /// </summary>
        [Test]
        public void DirectionalMovement_Position_Plus_Direction_Times_Distance()
        {
            var position = new Coord2D(5f, 5f);
            var distance = 3f;
            var newPosition = position + Coord2D.Directions4.Right * distance;

            Assert.That(newPosition, Is.EqualTo(new Coord2D(8f, 5f)));
        }

        /// <summary>
        /// 大きな値での演算
        /// </summary>
        [Test]
        public void LargeValues_OperationsWork()
        {
            var a = new Coord2D(1e6f, 1e6f);
            var b = new Coord2D(2e6f, 3e6f);

            var sum = a + b;
            Assert.That(sum.x, Is.EqualTo(3e6f).Within(1e-1f));
            Assert.That(sum.y, Is.EqualTo(4e6f).Within(1e-1f));
        }

        /// <summary>
        /// 非常に小さな値での演算
        /// </summary>
        [Test]
        public void SmallValues_OperationsWork()
        {
            var a = new Coord2D(1e-6f, 1e-6f);
            var b = new Coord2D(2e-6f, 3e-6f);

            var sum = a + b;
            Assert.That(sum.x, Is.EqualTo(3e-6f).Within(1e-12f));
            Assert.That(sum.y, Is.EqualTo(4e-6f).Within(1e-12f));
        }
    }
}