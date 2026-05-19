using NUnit.Framework;
using Game.Domain;

namespace Game.Domain.Tests
{
    [TestFixture]
    public class GameSaveDataTests
    {
        [Test]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            int testScore = 150;
            int testHighScore = 500;

            // Act
            var saveData = new GameSaveData(testScore, testHighScore);

            // Assert
            Assert.AreEqual(testScore, saveData.TestScore);
            Assert.AreEqual(testHighScore, saveData.TestHighScore);
        }

        [Test]
        public void Constructor_WithZeroValues_ShouldInitializeCorrectly()
        {
            // Arrange
            int testScore = 0;
            int testHighScore = 0;

            // Act
            var saveData = new GameSaveData(testScore, testHighScore);

            // Assert
            Assert.AreEqual(0, saveData.TestScore);
            Assert.AreEqual(0, saveData.TestHighScore);
        }

        [Test]
        public void FromGlobalState_ShouldConvertCorrectly()
        {
            // Arrange
            var globalState = new GameGlobalState(currentScore: new Score(200), highScore: new Score(600));

            // Act
            var saveData = GameSaveData.FromGlobalState(globalState);

            // Assert
            Assert.AreEqual(200, saveData.TestScore);
            Assert.AreEqual(600, saveData.TestHighScore);
        }

        [Test]
        public void FromGlobalState_WithZeroScores_ShouldConvertCorrectly()
        {
            // Arrange
            var globalState = new GameGlobalState(currentScore: Score.Zero, highScore: Score.Zero);

            // Act
            var saveData = GameSaveData.FromGlobalState(globalState);

            // Assert
            Assert.AreEqual(0, saveData.TestScore);
            Assert.AreEqual(0, saveData.TestHighScore);
        }

        [Test]
        public void FromGlobalState_WithHighScoresOnly_ShouldConvertCorrectly()
        {
            // Arrange
            var globalState = new GameGlobalState(currentScore: new Score(100), highScore: new Score(1000));

            // Act
            var saveData = GameSaveData.FromGlobalState(globalState);

            // Assert
            Assert.AreEqual(100, saveData.TestScore);
            Assert.AreEqual(1000, saveData.TestHighScore);
        }

        [Test]
        public void Default_ShouldReturnZeroValues()
        {
            // Act
            var saveData = GameSaveData.Default();

            // Assert
            Assert.AreEqual(0, saveData.TestScore);
            Assert.AreEqual(0, saveData.TestHighScore);
        }

        [Test]
        public void With_UpdateTestScore_ShouldUpdateOnlyTestScore()
        {
            // Arrange
            var originalData = new GameSaveData(testScore: 100, testHighScore: 500);

            // Act
            var updatedData = originalData.With(testScore: 250);

            // Assert
            Assert.AreEqual(250, updatedData.TestScore);
            Assert.AreEqual(500, updatedData.TestHighScore);
        }

        [Test]
        public void With_UpdateTestHighScore_ShouldUpdateOnlyTestHighScore()
        {
            // Arrange
            var originalData = new GameSaveData(testScore: 100, testHighScore: 500);

            // Act
            var updatedData = originalData.With(testHighScore: 800);

            // Assert
            Assert.AreEqual(100, updatedData.TestScore);
            Assert.AreEqual(800, updatedData.TestHighScore);
        }

        [Test]
        public void With_UpdateBothValues_ShouldUpdateBoth()
        {
            // Arrange
            var originalData = new GameSaveData(testScore: 100, testHighScore: 500);

            // Act
            var updatedData = originalData.With(testScore: 300, testHighScore: 900);

            // Assert
            Assert.AreEqual(300, updatedData.TestScore);
            Assert.AreEqual(900, updatedData.TestHighScore);
        }

        [Test]
        public void With_NoParameters_ShouldReturnSameValues()
        {
            // Arrange
            var originalData = new GameSaveData(testScore: 100, testHighScore: 500);

            // Act
            var updatedData = originalData.With();

            // Assert
            Assert.AreEqual(100, updatedData.TestScore);
            Assert.AreEqual(500, updatedData.TestHighScore);
        }

        [Test]
        public void With_ShouldNotModifyOriginalData()
        {
            // Arrange
            var originalData = new GameSaveData(testScore: 100, testHighScore: 500);

            // Act
            var updatedData = originalData.With(testScore: 250);

            // Assert
            Assert.AreEqual(100, originalData.TestScore);
            Assert.AreEqual(250, updatedData.TestScore);
        }

        [TestCase(0, 0)]
        [TestCase(100, 500)]
        [TestCase(1000, 5000)]
        public void Constructor_WithVariousValues_ShouldInitializeCorrectly(int testScore, int testHighScore)
        {
            // Act
            var saveData = new GameSaveData(testScore, testHighScore);

            // Assert
            Assert.AreEqual(testScore, saveData.TestScore);
            Assert.AreEqual(testHighScore, saveData.TestHighScore);
        }

        [Test]
        public void With_UpdateToZero_ShouldWork()
        {
            // Arrange
            var originalData = new GameSaveData(testScore: 100, testHighScore: 500);

            // Act
            var updatedData = originalData.With(testScore: 0);

            // Assert
            Assert.AreEqual(0, updatedData.TestScore);
            Assert.AreEqual(500, updatedData.TestHighScore);
        }

        [Test]
        public void With_UpdateToLargeValues_ShouldWork()
        {
            // Arrange
            var originalData = new GameSaveData(testScore: 100, testHighScore: 500);

            // Act
            var updatedData = originalData.With(testScore: 999999, testHighScore: 999999);

            // Assert
            Assert.AreEqual(999999, updatedData.TestScore);
            Assert.AreEqual(999999, updatedData.TestHighScore);
        }

        [Test]
        public void FromGlobalState_AndThenWith_ShouldChainCorrectly()
        {
            // Arrange
            var globalState = new GameGlobalState(currentScore: new Score(100), highScore: new Score(500));

            // Act
            var saveData = GameSaveData.FromGlobalState(globalState);
            var updatedData = saveData.With(testScore: 200);

            // Assert
            Assert.AreEqual(200, updatedData.TestScore);
            Assert.AreEqual(500, updatedData.TestHighScore);
        }

        [Test]
        public void Default_AndThenWith_ShouldWork()
        {
            // Act
            var saveData = GameSaveData.Default();
            var updatedData = saveData.With(testScore: 150, testHighScore: 600);

            // Assert
            Assert.AreEqual(150, updatedData.TestScore);
            Assert.AreEqual(600, updatedData.TestHighScore);
        }
    }
}