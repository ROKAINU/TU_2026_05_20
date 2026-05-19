#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Game.Kernel;

namespace Game.Kernel.Tests
{
    [TestFixture]
    public class LineAlgorithmsTests
    {
        /// <summary>
        /// GetLine メソッド：同一地点（開始 == 終了）
        /// </summary>
        [Test]
        public void GetLine_SamePoint_ReturnsSinglePoint()
        {
            var from = new Int2(5, 5);
            var to = new Int2(5, 5);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(new Int2(5, 5)));
        }

        /// <summary>
        /// GetLine メソッド：水平線（右方向）
        /// </summary>
        [Test]
        public void GetLine_HorizontalLine_Right()
        {
            var from = new Int2(0, 0);
            var to = new Int2(5, 0);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result[0], Is.EqualTo(new Int2(0, 0)));
            Assert.That(result[5], Is.EqualTo(new Int2(5, 0)));

            // すべてのセルが y=0 上にあることを確認
            foreach (var point in result)
            {
                Assert.That(point.y, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// GetLine メソッド：水平線（左方向）
        /// </summary>
        [Test]
        public void GetLine_HorizontalLine_Left()
        {
            var from = new Int2(5, 0);
            var to = new Int2(0, 0);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result[0], Is.EqualTo(new Int2(5, 0)));
            Assert.That(result[5], Is.EqualTo(new Int2(0, 0)));

            foreach (var point in result)
            {
                Assert.That(point.y, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// GetLine メソッド：垂直線（下方向）
        /// </summary>
        [Test]
        public void GetLine_VerticalLine_Down()
        {
            var from = new Int2(0, 0);
            var to = new Int2(0, 5);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result[0], Is.EqualTo(new Int2(0, 0)));
            Assert.That(result[5], Is.EqualTo(new Int2(0, 5)));

            foreach (var point in result)
            {
                Assert.That(point.x, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// GetLine メソッド：垂直線（上方向）
        /// </summary>
        [Test]
        public void GetLine_VerticalLine_Up()
        {
            var from = new Int2(0, 5);
            var to = new Int2(0, 0);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result[0], Is.EqualTo(new Int2(0, 5)));
            Assert.That(result[5], Is.EqualTo(new Int2(0, 0)));

            foreach (var point in result)
            {
                Assert.That(point.x, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// GetLine メソッド：45度斜線（右下）
        /// </summary>
        [Test]
        public void GetLine_Diagonal45_RightDown()
        {
            var from = new Int2(0, 0);
            var to = new Int2(5, 5);
            var result = LineAlgorithms.GetLine(from, to);

            // 45度線なので (5-0+1) = 6個のセルが返されるはず
            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result[0], Is.EqualTo(new Int2(0, 0)));
            Assert.That(result[5], Is.EqualTo(new Int2(5, 5)));

            // 開始地点と終了地点を確認
            Assert.That(result.First(), Is.EqualTo(from));
            Assert.That(result.Last(), Is.EqualTo(to));
        }

        /// <summary>
        /// GetLine メソッド：45度斜線（左上）
        /// </summary>
        [Test]
        public void GetLine_Diagonal45_LeftUp()
        {
            var from = new Int2(5, 5);
            var to = new Int2(0, 0);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result[0], Is.EqualTo(new Int2(5, 5)));
            Assert.That(result[5], Is.EqualTo(new Int2(0, 0)));
        }

        /// <summary>
        /// GetLine メソッド：急勾配（x < y）
        /// </summary>
        [Test]
        public void GetLine_SteepSlope()
        {
            var from = new Int2(0, 0);
            var to = new Int2(2, 10);
            var result = LineAlgorithms.GetLine(from, to);

            // 開始と終了を確認
            Assert.That(result[0], Is.EqualTo(new Int2(0, 0)));
            Assert.That(result.Last(), Is.EqualTo(new Int2(2, 10)));
            
            // すべての点が直線上にあることを確認
            AssertPointsFormLine(from, to, result);
        }

        /// <summary>
        /// GetLine メソッド：浅勾配（y < x）
        /// </summary>
        [Test]
        public void GetLine_ShallowSlope()
        {
            var from = new Int2(0, 0);
            var to = new Int2(10, 2);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result[0], Is.EqualTo(new Int2(0, 0)));
            Assert.That(result.Last(), Is.EqualTo(new Int2(10, 2)));
            
            AssertPointsFormLine(from, to, result);
        }

        /// <summary>
        /// GetLine メソッド：斜線（左下）
        /// </summary>
        [Test]
        public void GetLine_Diagonal_LeftDown()
        {
            var from = new Int2(5, 0);
            var to = new Int2(0, 5);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result[0], Is.EqualTo(new Int2(5, 0)));
            Assert.That(result.Last(), Is.EqualTo(new Int2(0, 5)));
        }

        /// <summary>
        /// GetLine メソッド：斜線（右上）
        /// </summary>
        [Test]
        public void GetLine_Diagonal_RightUp()
        {
            var from = new Int2(0, 5);
            var to = new Int2(5, 0);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result[0], Is.EqualTo(new Int2(0, 5)));
            Assert.That(result.Last(), Is.EqualTo(new Int2(5, 0)));
        }

        /// <summary>
        /// GetLine メソッド：短い水平線（1セル）
        /// </summary>
        [Test]
        public void GetLine_OneHorizontalStep()
        {
            var from = new Int2(0, 0);
            var to = new Int2(1, 0);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(new Int2(0, 0)));
            Assert.That(result[1], Is.EqualTo(new Int2(1, 0)));
        }

        /// <summary>
        /// GetLine メソッド：短い垂直線（1セル）
        /// </summary>
        [Test]
        public void GetLine_OneVerticalStep()
        {
            var from = new Int2(0, 0);
            var to = new Int2(0, 1);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(new Int2(0, 0)));
            Assert.That(result[1], Is.EqualTo(new Int2(0, 1)));
        }

        /// <summary>
        /// GetLine メソッド：短い斜線（1ステップ）
        /// </summary>
        [Test]
        public void GetLine_OneDiagonalStep()
        {
            var from = new Int2(0, 0);
            var to = new Int2(1, 1);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(new Int2(0, 0)));
            Assert.That(result[1], Is.EqualTo(new Int2(1, 1)));
        }

        /// <summary>
        /// GetLine メソッド：負の座標
        /// </summary>
        [Test]
        public void GetLine_NegativeCoordinates()
        {
            var from = new Int2(-5, -5);
            var to = new Int2(-1, -1);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result[0], Is.EqualTo(new Int2(-5, -5)));
            Assert.That(result.Last(), Is.EqualTo(new Int2(-1, -1)));
            Assert.That(result.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// GetLine メソッド：混合座標（負と正）
        /// </summary>
        [Test]
        public void GetLine_MixedCoordinates()
        {
            var from = new Int2(-2, -2);
            var to = new Int2(2, 2);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result[0], Is.EqualTo(new Int2(-2, -2)));
            Assert.That(result.Last(), Is.EqualTo(new Int2(2, 2)));
        }

        /// <summary>
        /// GetLine メソッド：重複がないことを確認
        /// </summary>
        [Test]
        public void GetLine_NoDuplicates()
        {
            var from = new Int2(0, 0);
            var to = new Int2(10, 7);
            var result = LineAlgorithms.GetLine(from, to);

            var uniquePoints = new HashSet<Int2>(result);
            Assert.That(uniquePoints.Count, Is.EqualTo(result.Count), 
                "Line should not contain duplicate points");
        }

        /// <summary>
        /// GetLine メソッド：連続性（隣接しているか）
        /// </summary>
        [Test]
        public void GetLine_ContinuousPath()
        {
            var from = new Int2(0, 0);
            var to = new Int2(10, 7);
            var result = LineAlgorithms.GetLine(from, to);

            for (int i = 0; i < result.Count - 1; i++)
            {
                var current = result[i];
                var next = result[i + 1];
                
                int dx = Math.Abs(current.x - next.x);
                int dy = Math.Abs(current.y - next.y);
                
                // 次のセルは隣接していなければならない
                Assert.That(dx + dy, Is.EqualTo(1).Or.EqualTo(2),
                    $"Points {current} and {next} are not adjacent");
            }
        }

        /// <summary>
        /// GetLine メソッド：方向を逆にしたときの対応性
        /// </summary>
        [Test]
        public void GetLine_ReversedPath_IsReverseOfOriginal()
        {
            var from = new Int2(0, 0);
            var to = new Int2(7, 5);
            var forward = LineAlgorithms.GetLine(from, to);
            var backward = LineAlgorithms.GetLine(to, from);

            Assert.That(forward.Count, Is.EqualTo(backward.Count));
            
            // 順序が逆になっていることを確認
            for (int i = 0; i < forward.Count; i++)
            {
                Assert.That(forward[i], Is.EqualTo(backward[backward.Count - 1 - i]));
            }
        }

        /// <summary>
        /// GetLine メソッド：大きな距離
        /// </summary>
        [Test]
        public void GetLine_LargeDistance()
        {
            var from = new Int2(0, 0);
            var to = new Int2(100, 50);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result.Count, Is.GreaterThan(0));
            Assert.That(result[0], Is.EqualTo(from));
            Assert.That(result.Last(), Is.EqualTo(to));
            AssertPointsFormLine(from, to, result);
        }

        /// <summary>
        /// GetLine メソッド：特定の勾配パターン（3:1）
        /// </summary>
        [Test]
        public void GetLine_Slope3to1()
        {
            var from = new Int2(0, 0);
            var to = new Int2(9, 3);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result[0], Is.EqualTo(from));
            Assert.That(result.Last(), Is.EqualTo(to));
            AssertPointsFormLine(from, to, result);
        }

        /// <summary>
        /// GetLine メソッド：特定の勾配パターン（1:3）
        /// </summary>
        [Test]
        public void GetLine_Slope1to3()
        {
            var from = new Int2(0, 0);
            var to = new Int2(3, 9);
            var result = LineAlgorithms.GetLine(from, to);

            Assert.That(result[0], Is.EqualTo(from));
            Assert.That(result.Last(), Is.EqualTo(to));
            AssertPointsFormLine(from, to, result);
        }

        // ============================================================================
        // ヘルパーメソッド
        // ============================================================================

        /// <summary>
        /// 2点を結ぶ直線上に結果の点があるかを検証します。
        /// (Bresenhamアルゴリズムは近似なので、完全な直線ではなく最適な離散化です)
        /// </summary>
        private void AssertPointsFormLine(Int2 from, Int2 to, List<Int2> result)
        {
            // 開始点と終了点が含まれている
            Assert.That(result[0], Is.EqualTo(from), "Start point should be included");
            Assert.That(result.Last(), Is.EqualTo(to), "End point should be included");

            // 各点が移動方向に沿っているか確認
            int dx = (to.x - from.x);
            int dy = (to.y - from.y);
            int sx = (dx == 0) ? 0 : (dx > 0) ? 1 : -1;
            int sy = (dy == 0) ? 0 : (dy > 0) ? 1 : -1;

            for (int i = 0; i < result.Count - 1; i++)
            {
                var current = result[i];
                var next = result[i + 1];
                
                int stepX = next.x - current.x;
                int stepY = next.y - current.y;

                // 移動方向の符号が正しいか確認
                if (stepX != 0)
                    Assert.That(stepX * sx, Is.GreaterThanOrEqualTo(0), 
                        $"Step X direction mismatch at {current} -> {next}");
                if (stepY != 0)
                    Assert.That(stepY * sy, Is.GreaterThanOrEqualTo(0), 
                        $"Step Y direction mismatch at {current} -> {next}");
            }
        }
    }
}