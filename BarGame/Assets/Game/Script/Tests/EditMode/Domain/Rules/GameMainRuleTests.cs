using NUnit.Framework;
using Game.Domain;

namespace Game.Domain.Tests
{
    [TestFixture]
    public class GameMainRuleTests
    {
        [Test]
        public void IsTimeUp_WithPositiveTime_ShouldReturnFalse()
        {
            // Arrange
            var state = new GameMainState(new RemainingTime(10f));

            // Act
            var result = GameMainRule.IsTimeUp(state);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsTimeUp_WithSmallPositiveTime_ShouldReturnFalse()
        {
            // Arrange
            var state = new GameMainState(new RemainingTime(0.1f));

            // Act
            var result = GameMainRule.IsTimeUp(state);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsTimeUp_WithZeroTime_ShouldReturnTrue()
        {
            // Arrange
            var state = new GameMainState(new RemainingTime(0f));

            // Act
            var result = GameMainRule.IsTimeUp(state);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsTimeUp_WithNegativeTime_ShouldReturnTrue()
        {
            // Arrange
            var state = new GameMainState(new RemainingTime(-1f));

            // Act
            var result = GameMainRule.IsTimeUp(state);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsTimeUp_WithLargeNegativeTime_ShouldReturnTrue()
        {
            // Arrange
            var state = new GameMainState(new RemainingTime(-100f));

            // Act
            var result = GameMainRule.IsTimeUp(state);

            // Assert
            Assert.IsTrue(result);
        }

        [TestCase(1f, false)]
        [TestCase(5f, false)]
        [TestCase(30f, false)]
        [TestCase(60f, false)]
        [TestCase(0f, true)]
        [TestCase(-0.1f, true)]
        [TestCase(-1f, true)]
        [TestCase(-60f, true)]
        public void IsTimeUp_WithVariousValues_ShouldReturnCorrectResult(float remainingTime, bool expectedResult)
        {
            // Arrange
            var state = new GameMainState(new RemainingTime(remainingTime));

            // Act
            var result = GameMainRule.IsTimeUp(state);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void IsTimeUp_WithVerySmallPositiveValue_ShouldReturnFalse()
        {
            // Arrange
            var state = new GameMainState(new RemainingTime(0.001f));

            // Act
            var result = GameMainRule.IsTimeUp(state);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsTimeUp_WithVerySmallNegativeValue_ShouldReturnTrue()
        {
            // Arrange
            var state = new GameMainState(new RemainingTime(-0.001f));

            // Act
            var result = GameMainRule.IsTimeUp(state);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsTimeUp_BoundaryBetweenTrueAndFalse_AtZero()
        {
            // Arrange
            var positiveState = new GameMainState(new RemainingTime(0.0001f));
            var zeroState = new GameMainState(new RemainingTime(0f));
            var negativeState = new GameMainState(new RemainingTime(-0.0001f));

            // Act
            var positiveResult = GameMainRule.IsTimeUp(positiveState);
            var zeroResult = GameMainRule.IsTimeUp(zeroState);
            var negativeResult = GameMainRule.IsTimeUp(negativeState);

            // Assert
            Assert.IsFalse(positiveResult);
            Assert.IsTrue(zeroResult);
            Assert.IsTrue(negativeResult);
        }
    }
}