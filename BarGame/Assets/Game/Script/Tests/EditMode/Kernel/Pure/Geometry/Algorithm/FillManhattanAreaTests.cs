#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Game.Kernel;

namespace Game.Kernel.Tests
{
    [TestFixture]
    public class FillManhattanAreaTests
    {
        /// <summary>
        /// Fill メソッド：grid が null の場合
        /// </summary>
        [Test]
        public void Fill_NullGrid_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                FillManhattanArea.Fill<int>(null!, 0, 0, 1, 42);
            });
        }

        /// <summary>
        /// Fill メソッド：cx が範囲外（負）の場合
        /// </summary>
        [Test]
        public void Fill_CxOutOfBounds_Negative_ThrowsArgumentOutOfRangeException()
        {
            var grid = new int[5, 5];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FillManhattanArea.Fill(grid, -1, 2, 1, 42);
            });
        }

        /// <summary>
        /// Fill メソッド：cx が範囲外（グリッド幅以上）の場合
        /// </summary>
        [Test]
        public void Fill_CxOutOfBounds_TooLarge_ThrowsArgumentOutOfRangeException()
        {
            var grid = new int[5, 5];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FillManhattanArea.Fill(grid, 5, 2, 1, 42);
            });
        }

        /// <summary>
        /// Fill メソッド：cy が範囲外（負）の場合
        /// </summary>
        [Test]
        public void Fill_CyOutOfBounds_Negative_ThrowsArgumentOutOfRangeException()
        {
            var grid = new int[5, 5];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FillManhattanArea.Fill(grid, 2, -1, 1, 42);
            });
        }

        /// <summary>
        /// Fill メソッド：cy が範囲外（グリッド高さ以上）の場合
        /// </summary>
        [Test]
        public void Fill_CyOutOfBounds_TooLarge_ThrowsArgumentOutOfRangeException()
        {
            var grid = new int[5, 5];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FillManhattanArea.Fill(grid, 2, 5, 1, 42);
            });
        }

        /// <summary>
        /// Fill メソッド：半径 0（中心のみ）
        /// </summary>
        [Test]
        public void Fill_Radius0_FillsCenterOnly()
        {
            var grid = new int[5, 5];
            FillManhattanArea.Fill(grid, 2, 2, 0, 42);

            // 中心だけ埋まっていることを確認
            Assert.That(grid[2, 2], Is.EqualTo(42));
            
            // 周辺は埋まっていないことを確認
            Assert.That(grid[1, 2], Is.EqualTo(0));
            Assert.That(grid[3, 2], Is.EqualTo(0));
            Assert.That(grid[2, 1], Is.EqualTo(0));
            Assert.That(grid[2, 3], Is.EqualTo(0));
        }

        /// <summary>
        /// Fill メソッド：半径 1（菱形）
        /// </summary>
        [Test]
        public void Fill_Radius1_FillsDiamond()
        {
            var grid = new int[5, 5];
            FillManhattanArea.Fill(grid, 2, 2, 1, 42);

            // 菱形の形状を確認
            // 中心
            Assert.That(grid[2, 2], Is.EqualTo(42));
            // 上下左右
            Assert.That(grid[1, 2], Is.EqualTo(42));
            Assert.That(grid[3, 2], Is.EqualTo(42));
            Assert.That(grid[2, 1], Is.EqualTo(42));
            Assert.That(grid[2, 3], Is.EqualTo(42));
            
            // 斜め（距離2）は埋まっていない
            Assert.That(grid[1, 1], Is.EqualTo(0));
            Assert.That(grid[3, 3], Is.EqualTo(0));
        }

        /// <summary>
        /// Fill メソッド：半径 2
        /// </summary>
        [Test]
        public void Fill_Radius2_FillsLargerDiamond()
        {
            var grid = new int[7, 7];
            FillManhattanArea.Fill(grid, 3, 3, 2, 99);

            // 中心
            Assert.That(grid[3, 3], Is.EqualTo(99));
            
            // 距離1
            Assert.That(grid[2, 3], Is.EqualTo(99));
            Assert.That(grid[4, 3], Is.EqualTo(99));
            Assert.That(grid[3, 2], Is.EqualTo(99));
            Assert.That(grid[3, 4], Is.EqualTo(99));
            
            // 距離2
            Assert.That(grid[1, 3], Is.EqualTo(99));
            Assert.That(grid[5, 3], Is.EqualTo(99));
            Assert.That(grid[3, 1], Is.EqualTo(99));
            Assert.That(grid[3, 5], Is.EqualTo(99));
            Assert.That(grid[2, 2], Is.EqualTo(99));
            Assert.That(grid[4, 4], Is.EqualTo(99));
            
            // 距離3以上は埋まっていない
            Assert.That(grid[0, 3], Is.EqualTo(0));
            Assert.That(grid[1, 1], Is.EqualTo(0));
        }

        /// <summary>
        /// Fill メソッド：グリッドの隅での塗りつぶし
        /// </summary>
        [Test]
        public void Fill_CornerPosition_FillsPartialArea()
        {
            var grid = new int[5, 5];
            FillManhattanArea.Fill(grid, 0, 0, 2, 77);

            // グリッド内のみ埋まっていることを確認
            Assert.That(grid[0, 0], Is.EqualTo(77));
            Assert.That(grid[1, 0], Is.EqualTo(77));
            Assert.That(grid[0, 1], Is.EqualTo(77));
            Assert.That(grid[2, 0], Is.EqualTo(77));
            
            // グリッド外のはずの場所は埋まっていない
            Assert.That(grid[0, 2], Is.EqualTo(77));  // これは距離2
            Assert.That(grid[4, 4], Is.EqualTo(0));   // 遠い場所は埋まっていない
        }

        /// <summary>
        /// Fill メソッド：異なる値型でのテスト
        /// </summary>
        [Test]
        public void Fill_WithString_FillsCorrectly()
        {
            var grid = new string[3, 3]!;
            FillManhattanArea.Fill(grid, 1, 1, 1, "filled");

            Assert.That(grid[1, 1], Is.EqualTo("filled"));
            Assert.That(grid[0, 1], Is.EqualTo("filled"));
            Assert.That(grid[2, 1], Is.EqualTo("filled"));
            Assert.That(grid[1, 0], Is.EqualTo("filled"));
            Assert.That(grid[1, 2], Is.EqualTo("filled"));
        }

        /// <summary>
        /// FillPosition メソッド：grid が null の場合
        /// </summary>
        [Test]
        public void FillPosition_NullGrid_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                FillManhattanArea.FillPosition<int>(null!, 0, 0, 1);
            });
        }

        /// <summary>
        /// FillPosition メソッド：cx が範囲外の場合
        /// </summary>
        [Test]
        public void FillPosition_CxOutOfBounds_ThrowsArgumentOutOfRangeException()
        {
            var grid = new int[5, 5];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FillManhattanArea.FillPosition(grid, -1, 2, 1);
            });
        }

        /// <summary>
        /// FillPosition メソッド：cy が範囲外の場合
        /// </summary>
        [Test]
        public void FillPosition_CyOutOfBounds_ThrowsArgumentOutOfRangeException()
        {
            var grid = new int[5, 5];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FillManhattanArea.FillPosition(grid, 2, -1, 1);
            });
        }

        /// <summary>
        /// FillPosition メソッド：半径 0（中心のみ）
        /// </summary>
        [Test]
        public void FillPosition_Radius0_ReturnsCenterOnly()
        {
            var grid = new int[5, 5];
            var positions = FillManhattanArea.FillPosition(grid, 2, 2, 0);

            Assert.That(positions.Count, Is.EqualTo(1));
            Assert.That(positions[0], Is.EqualTo(new Int2(2, 2)));
        }

        /// <summary>
        /// FillPosition メソッド：半径 1
        /// </summary>
        [Test]
        public void FillPosition_Radius1_ReturnsFivePositions()
        {
            var grid = new int[5, 5];
            var positions = FillManhattanArea.FillPosition(grid, 2, 2, 1);

            Assert.That(positions.Count, Is.EqualTo(5));
            
            // 期待される座標
            var expected = new[]
            {
                new Int2(2, 2),  // 中心
                new Int2(1, 2),  // 左
                new Int2(3, 2),  // 右
                new Int2(2, 1),  // 上
                new Int2(2, 3)   // 下
            };

            foreach (var pos in expected)
            {
                Assert.That(positions, Contains.Item(pos));
            }
        }

        /// <summary>
        /// FillPosition メソッド：半径 2
        /// </summary>
        [Test]
        public void FillPosition_Radius2_ReturnsCorrectCount()
        {
            var grid = new int[7, 7];
            var positions = FillManhattanArea.FillPosition(grid, 3, 3, 2);

            // マンハッタン距離が2以下のセルの個数を確認
            // 中心: 1
            // 距離1: 4
            // 距離2: 8
            // 合計: 13
            Assert.That(positions.Count, Is.EqualTo(13));
        }

        /// <summary>
        /// FillPosition メソッド：グリッドの隅
        /// </summary>
        [Test]
        public void FillPosition_CornerPosition_ReturnsOnlyInBoundsPositions()
        {
            var grid = new int[5, 5];
            var positions = FillManhattanArea.FillPosition(grid, 0, 0, 2);

            // すべての座標がグリッド内か確認
            foreach (var pos in positions)
            {
                Assert.That(pos.x, Is.GreaterThanOrEqualTo(0));
                Assert.That(pos.x, Is.LessThan(5));
                Assert.That(pos.y, Is.GreaterThanOrEqualTo(0));
                Assert.That(pos.y, Is.LessThan(5));
            }

            // 中心を含む
            Assert.That(positions, Contains.Item(new Int2(0, 0)));
        }

        /// <summary>
        /// FillPosition メソッド：返されたすべての座標がマンハッタン距離以内か検証
        /// </summary>
        [Test]
        public void FillPosition_AllPositions_WithinManhattanDistance()
        {
            var grid = new int[10, 10];
            int cx = 5, cy = 5, r = 3;
            var positions = FillManhattanArea.FillPosition(grid, cx, cy, r);

            foreach (var pos in positions)
            {
                int distance = Math.Abs(pos.x - cx) + Math.Abs(pos.y - cy);
                Assert.That(distance, Is.LessThanOrEqualTo(r), 
                    $"Position {pos} has Manhattan distance {distance} > {r}");
            }
        }

        /// <summary>
        /// FillPosition メソッド：パラメータ化テスト（様々な半径）
        /// </summary>
        [TestCase(0, 1)]
        [TestCase(1, 5)]
        [TestCase(2, 13)]
        [TestCase(3, 25)]
        public void FillPosition_VariousRadii_ReturnsExpectedCount(int radius, int expectedCount)
        {
            var grid = new int[10, 10];
            var positions = FillManhattanArea.FillPosition(grid, 5, 5, radius);
            Assert.That(positions.Count, Is.EqualTo(expectedCount));
        }

        /// <summary>
        /// Fill と FillPosition の整合性チェック
        /// </summary>
        [Test]
        public void Fill_AndFillPosition_AreConsistent()
        {
            var grid = new int[10, 10];
            int cx = 5, cy = 5, radius = 2;

            // FillPosition で座標リストを取得
            var positions = FillManhattanArea.FillPosition(grid, cx, cy, radius);

            // Fill でグリッドを埋める
            FillManhattanArea.Fill(grid, cx, cy, radius, 42);

            // 返された座標がすべて埋まっているか確認
            foreach (var pos in positions)
            {
                Assert.That(grid[pos.x, pos.y], Is.EqualTo(42),
                    $"Position {pos} should be filled");
            }

            // 埋まっているセルがすべて返されたリストに含まれているか確認
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (grid[x, y] == 42)
                    {
                        Assert.That(positions, Contains.Item(new Int2(x, y)),
                            $"Position ({x}, {y}) is filled but not in FillPosition result");
                    }
                }
            }
        }

        /// <summary>
        /// FillPosition メソッド：複数回の呼び出しが同じ結果を返すか
        /// </summary>
        [Test]
        public void FillPosition_MultipleCalls_ReturnConsistentResults()
        {
            var grid = new int[10, 10];
            var positions1 = FillManhattanArea.FillPosition(grid, 5, 5, 3);
            var positions2 = FillManhattanArea.FillPosition(grid, 5, 5, 3);

            Assert.That(positions1.Count, Is.EqualTo(positions2.Count));
            foreach (var pos in positions1)
            {
                Assert.That(positions2, Contains.Item(pos));
            }
        }

        /// <summary>
        /// グリッドの辺での塗りつぶし（半径が大きい場合）
        /// </summary>
        [Test]
        public void Fill_LargeRadius_ClampsToGridBounds()
        {
            var grid = new int[5, 5];
            FillManhattanArea.Fill(grid, 2, 2, 10, 55);

            // グリッド内のすべてのセルが埋まっていることを確認
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    Assert.That(grid[x, y], Is.EqualTo(55),
                        $"Cell ({x}, {y}) should be filled with large radius");
                }
            }
        }
    }
}