using NUnit.Framework;
using Game.Domain;

namespace Game.Domain.Tests
{
    [TestFixture]
    public class GameGlobalStateReducerTests
    {
        [Test]
        public void Initialize_ShouldReturnDefaultState()
        {
            var currentState = new GameGlobalState(new Score(100), new Score(500));
            var reducer = GameGlobalStateReducer.Initialize();

            var newState = reducer(currentState);

            Assert.AreEqual(GameGlobalState.Default.CurrentScore.Value, newState.CurrentScore.Value);
            Assert.AreEqual(GameGlobalState.Default.HighScore.Value, newState.HighScore.Value);
        }

        [Test]
        public void Update_ShouldReplaceStateWithNewState()
        {
            var currentState = new GameGlobalState(new Score(50), new Score(300));
            var newState = new GameGlobalState(new Score(200), new Score(400));
            var reducer = GameGlobalStateReducer.Update(newState);

            var result = reducer(currentState);

            Assert.AreEqual(200, result.CurrentScore.Value);
            Assert.AreEqual(400, result.HighScore.Value);
        }

        [Test]
        public void AddScore_ShouldIncreaseCurrentScore()
        {
            var currentState = new GameGlobalState(new Score(100), new Score(500));
            var reducer = GameGlobalStateReducer.AddScore(50);

            var newState = reducer(currentState);

            Assert.AreEqual(150, newState.CurrentScore.Value);
            Assert.AreEqual(500, newState.HighScore.Value);
        }

        [Test]
        public void AddScore_ShouldUpdateHighScoreWhenCurrentScoreExceedsIt()
        {
            var currentState = new GameGlobalState(new Score(100), new Score(300));
            var reducer = GameGlobalStateReducer.AddScore(250);

            var newState = reducer(currentState);

            Assert.AreEqual(350, newState.CurrentScore.Value);
            Assert.AreEqual(350, newState.HighScore.Value);
        }

        [Test]
        public void AddScore_ShouldNotUpdateHighScoreWhenCurrentScoreIsLower()
        {
            var currentState = new GameGlobalState(new Score(100), new Score(500));
            var reducer = GameGlobalStateReducer.AddScore(50);

            var newState = reducer(currentState);

            Assert.AreEqual(150, newState.CurrentScore.Value);
            Assert.AreEqual(500, newState.HighScore.Value);
        }

        [Test]
        public void AddScore_ShouldHandleNegativeValues()
        {
            var currentState = new GameGlobalState(new Score(100), new Score(500));
            var reducer = GameGlobalStateReducer.AddScore(-30);

            var newState = reducer(currentState);

            Assert.AreEqual(70, newState.CurrentScore.Value);
            Assert.AreEqual(500, newState.HighScore.Value);
        }

        [Test]
        public void ClearScore_ShouldResetCurrentScoreToZero()
        {
            var currentState = new GameGlobalState(new Score(250), new Score(500));
            var reducer = GameGlobalStateReducer.ClearScore();

            var newState = reducer(currentState);

            Assert.AreEqual(0, newState.CurrentScore.Value);
            Assert.AreEqual(500, newState.HighScore.Value);
        }

        [Test]
        public void ClearScore_ShouldPreserveHighScore()
        {
            var highScore = 1000;
            var currentState = new GameGlobalState(new Score(500), new Score(highScore));
            var reducer = GameGlobalStateReducer.ClearScore();

            var newState = reducer(currentState);

            Assert.AreEqual(0, newState.CurrentScore.Value);
            Assert.AreEqual(highScore, newState.HighScore.Value);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(100)]
        [TestCase(999)]
        public void AddScore_WithVariousValues_ShouldCalculateCorrectly(int scoreToAdd)
        {
            var initialScore = 50;
            var currentState = new GameGlobalState(new Score(initialScore), Score.Zero);
            var reducer = GameGlobalStateReducer.AddScore(scoreToAdd);

            var newState = reducer(currentState);

            Assert.AreEqual(initialScore + scoreToAdd, newState.CurrentScore.Value);
        }

        [TestCase(100, 50, 150, 500)]
        [TestCase(100, 400, 500, 500)]
        [TestCase(100, -30, 70, 500)]
        public void AddScore_MultipleScenarios(
            int initialScore,
            int scoreToAdd,
            int expectedCurrentScore,
            int expectedHighScore)
        {
            var currentState = new GameGlobalState(new Score(initialScore), new Score(500));
            var reducer = GameGlobalStateReducer.AddScore(scoreToAdd);

            var newState = reducer(currentState);

            Assert.AreEqual(expectedCurrentScore, newState.CurrentScore.Value);
            Assert.AreEqual(expectedHighScore, newState.HighScore.Value);
        }
    }
}