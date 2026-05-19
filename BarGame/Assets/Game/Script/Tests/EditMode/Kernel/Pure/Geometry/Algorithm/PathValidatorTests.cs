#nullable enable
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Game.Kernel;

namespace Game.Kernel.Tests
{
    [TestFixture]
    public class PathValidatorTests
    {
        /// <summary>
        /// パスが null の場合のテスト
        /// </summary>
        [Test]
        public void Validate_NullPath_ReturnsError()
        {
            var result = PathValidator.Validate(null!, x => false);
            Assert.That(result.IsErr, Is.True);
        }

        /// <summary>
        /// パスが空の場合のテスト
        /// </summary>
        [Test]
        public void Validate_EmptyPath_ReturnsError()
        {
            var path = new List<Int2>();
            var result = PathValidator.Validate(path, x => false);
            Assert.That(result.IsErr, Is.True);
        }

        /// <summary>
        /// 単一セルのパス：検証なしの場合
        /// </summary>
        [Test]
        public void Validate_SingleCell_WithoutValidation_ReturnsOk()
        {
            var path = new List<Int2> { new(0, 0) };
            var result = PathValidator.Validate(path, x => false, validateStartCell: false, validateEndCell: false);
            Assert.That(result.IsErr, Is.False);
        }

        /// <summary>
        /// 単一セルのパス：開始セル検証有効、セルがブロックされている場合
        /// </summary>
        [Test]
        public void Validate_SingleCell_StartValidationEnabled_CellBlocked_ReturnsError()
        {
            var path = new List<Int2> { new(0, 0) };
            var result = PathValidator.Validate(path, x => true, validateStartCell: true);
            Assert.That(result.IsErr, Is.True);
        }

        /// <summary>
        /// 単一セルのパス：開始セル検証有効、セルがブロックされていない場合
        /// </summary>
        [Test]
        public void Validate_SingleCell_StartValidationEnabled_CellNotBlocked_ReturnsOk()
        {
            var path = new List<Int2> { new(0, 0) };
            var result = PathValidator.Validate(path, x => false, validateStartCell: true);
            Assert.That(result.IsErr, Is.False);
        }

        /// <summary>
        /// 開始セルがブロックされている場合
        /// </summary>
        [Test]
        public void Validate_StartCellBlocked_ReturnsError()
        {
            var path = new List<Int2> { new(0, 0), new(1, 0) };
            var blockedCells = new HashSet<Int2> { new(0, 0) };
            var result = PathValidator.Validate(path, cell => blockedCells.Contains(cell), validateStartCell: true);
            Assert.That(result.IsErr, Is.True);
        }

        /// <summary>
        /// 終了セルがブロックされている場合
        /// </summary>
        [Test]
        public void Validate_EndCellBlocked_ReturnsError()
        {
            var path = new List<Int2> { new(0, 0), new(1, 0), new(2, 0) };
            var blockedCells = new HashSet<Int2> { new(2, 0) };
            var result = PathValidator.Validate(path, cell => blockedCells.Contains(cell), validateEndCell: true);
            Assert.That(result.IsErr, Is.True);
        }

        /// <summary>
        /// 中間セルがブロックされている場合
        /// </summary>
        [Test]
        public void Validate_MiddleCellBlocked_ReturnsError()
        {
            var path = new List<Int2> { new(0, 0), new(1, 0), new(2, 0) };
            var blockedCells = new HashSet<Int2> { new(1, 0) };
            var result = PathValidator.Validate(path, cell => blockedCells.Contains(cell));
            Assert.That(result.IsErr, Is.True);
        }

        /// <summary>
        /// 隣接していないセル間の移動（エラーケース）
        /// </summary>
        [Test]
        public void Validate_NonAdjacentCells_ReturnsError()
        {
            var path = new List<Int2> { new(0, 0), new(2, 0) };
            var result = PathValidator.Validate(path, x => false);
            Assert.That(result.IsErr, Is.True);
        }

        /// <summary>
        /// 重複セルが存在する場合
        /// </summary>
        [Test]
        public void Validate_DuplicateCells_ReturnsError()
        {
            var path = new List<Int2> { new(0, 0), new(1, 0), new(1, 0) };
            var result = PathValidator.Validate(path, x => false);
            Assert.That(result.IsErr, Is.True);
        }

        /// <summary>
        /// 有効な水平移動パス
        /// </summary>
        [Test]
        public void Validate_ValidHorizontalPath_ReturnsOk()
        {
            var path = new List<Int2> { new(0, 0), new(1, 0), new(2, 0), new(3, 0) };
            var result = PathValidator.Validate(path, x => false);
            Assert.That(result.IsErr, Is.False);
        }

        /// <summary>
        /// 有効な垂直移動パス
        /// </summary>
        [Test]
        public void Validate_ValidVerticalPath_ReturnsOk()
        {
            var path = new List<Int2> { new(0, 0), new(0, 1), new(0, 2), new(0, 3) };
            var result = PathValidator.Validate(path, x => false);
            Assert.That(result.IsErr, Is.False);
        }

        /// <summary>
        /// 有効な斜め移動パス（許可）
        /// </summary>
        [Test]
        public void Validate_ValidDiagonalPath_AllowDiagonal_ReturnsOk()
        {
            var path = new List<Int2> { new(0, 0), new(1, 1), new(2, 2) };
            var result = PathValidator.Validate(path, x => false, allowDiagonal: true);
            Assert.That(result.IsErr, Is.False);
        }

        /// <summary>
        /// 斜め移動パスで隅が両方ブロックされている場合
        /// </summary>
        [Test]
        public void Validate_DiagonalPath_BothCornersBlocked_ReturnsError()
        {
            var path = new List<Int2> { new(0, 0), new(1, 1) };
            var blockedCells = new HashSet<Int2> { new(0, 1), new(1, 0) };
            var result = PathValidator.Validate(path, cell => blockedCells.Contains(cell), allowDiagonal: true);
            Assert.That(result.IsErr, Is.True);
        }

        /// <summary>
        /// 斜め移動パスで片方の隅だけブロックされている場合（通過可能）
        /// </summary>
        [Test]
        public void Validate_DiagonalPath_OneCornerBlocked_ReturnsOk()
        {
            var path = new List<Int2> { new(0, 0), new(1, 1) };
            var blockedCells = new HashSet<Int2> { new(0, 1) };  // もう一つは空いている
            var result = PathValidator.Validate(path, cell => blockedCells.Contains(cell), allowDiagonal: true);
            Assert.That(result.IsErr, Is.False);
        }

        /// <summary>
        /// 複雑なパス：水平、垂直、斜めの混合
        /// </summary>
        [Test]
        public void Validate_ComplexPath_Mixed_ReturnsOk()
        {
            var path = new List<Int2>
            {
                new(0, 0),
                new(1, 0),
                new(2, 0),
                new(2, 1),
                new(3, 2),
                new(3, 3)
            };
            var result = PathValidator.Validate(path, x => false, allowDiagonal: true);
            Assert.That(result.IsErr, Is.False);
        }

        /// <summary>
        /// cellFilter が null の場合は ArgumentNullException をスロー
        /// </summary>
        [Test]
        public void Validate_NullCellFilter_ThrowsArgumentNullException()
        {
            var path = new List<Int2> { new(0, 0), new(1, 0) };
            Assert.Throws<ArgumentNullException>(() =>
            {
                PathValidator.Validate(path, null!);
            });
        }

        /// <summary>
        /// 複数パラメータの組み合わせテスト
        /// </summary>
        [TestCase(true, true, false)]   // allowDiagonal=true, validateStartCell=true, validateEndCell=false
        [TestCase(true, false, true)]   // allowDiagonal=true, validateStartCell=false, validateEndCell=true
        [TestCase(false, true, true)]   // allowDiagonal=false, validateStartCell=true, validateEndCell=true
        public void Validate_VariousParameters_ValidPath_ReturnsOk(bool allowDiagonal, bool validateStart, bool validateEnd)
        {
            var path = new List<Int2> { new(0, 0), new(1, 1), new(2, 2) };
            var result = PathValidator.Validate(path, x => false, allowDiagonal, validateStart, validateEnd);
            Assert.That(result.IsErr, Is.False);
        }

        /// <summary>
        /// L字パス
        /// </summary>
        [Test]
        public void Validate_LShapedPath_ReturnsOk()
        {
            var path = new List<Int2>
            {
                new(0, 0),
                new(1, 0),
                new(2, 0),
                new(2, 1),
                new(2, 2)
            };
            var result = PathValidator.Validate(path, x => false);
            Assert.That(result.IsErr, Is.False);
        }

        /// <summary>
        /// ジグザグパス
        /// </summary>
        [Test]
        public void Validate_ZigzagPath_ReturnsOk()
        {
            var path = new List<Int2>
            {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(2, 1),
                new(2, 2)
            };
            var result = PathValidator.Validate(path, x => false);
            Assert.That(result.IsErr, Is.False);
        }
    }
}