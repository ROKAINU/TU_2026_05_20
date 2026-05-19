using NUnit.Framework;
using Game.Domain;

namespace Game.Domain.Tests
{
    [TestFixture]
    public class GameMainStateReducerTests
    {
        [Test]
        public void Initialize_ShouldReturnDefaultState()
        {
            var currentState = new GameMainState(new RemainingTime(30f));
            var reducer = GameMainStateReducer.Initialize();

            var newState = reducer(currentState);

            Assert.AreEqual(GameMainState.Default.RemainingTime.Value, newState.RemainingTime.Value, 0.0001f);
        }

        [Test]
        public void Update_ShouldReplaceStateWithNewState()
        {
            var currentState = new GameMainState(new RemainingTime(30f));
            var newState = new GameMainState(new RemainingTime(45f));
            var reducer = GameMainStateReducer.Update(newState);

            var result = reducer(currentState);

            Assert.AreEqual(45f, result.RemainingTime.Value, 0.0001f);
        }

        [Test]
        public void DecreaseRemainingTime_ShouldDecreaseTimeByDeltaTime()
        {
            var currentState = new GameMainState(new RemainingTime(30f));
            var deltaTime = 0.016f;
            var reducer = GameMainStateReducer.DecreaseRemainingTime(deltaTime);

            var newState = reducer(currentState);

            Assert.AreEqual(30f - deltaTime, newState.RemainingTime.Value, 0.0001f);
        }

        [Test]
        public void DecreaseRemainingTime_ShouldHandleLargeTimeDelta()
        {
            var currentState = new GameMainState(new RemainingTime(10f));
            var deltaTime = 5f;
            var reducer = GameMainStateReducer.DecreaseRemainingTime(deltaTime);

            var newState = reducer(currentState);

            Assert.AreEqual(5f, newState.RemainingTime.Value, 0.0001f);
        }

        // ★修正：負数は禁止なので0になる
        [Test]
        public void DecreaseRemainingTime_ShouldClampAtZero()
        {
            var currentState = new GameMainState(new RemainingTime(2f));
            var deltaTime = 5f;
            var reducer = GameMainStateReducer.DecreaseRemainingTime(deltaTime);

            var newState = reducer(currentState);

            Assert.AreEqual(0f, newState.RemainingTime.Value, 0.0001f);
        }

        [TestCase(0.016f)]
        [TestCase(0.033f)]
        [TestCase(0.1f)]
        [TestCase(1f)]
        public void DecreaseRemainingTime_WithVariousDeltaTimes_ShouldCalculateCorrectly(float deltaTime)
        {
            var initialTime = 60f;
            var currentState = new GameMainState(new RemainingTime(initialTime));
            var reducer = GameMainStateReducer.DecreaseRemainingTime(deltaTime);

            var newState = reducer(currentState);

            Assert.AreEqual(initialTime - deltaTime, newState.RemainingTime.Value, 0.0001f);
        }

        [TestCase(60f, 0.016f, 59.984f)]
        [TestCase(30f, 1f, 29f)]
        [TestCase(5f, 5f, 0f)]
        [TestCase(10f, 15f, 0f)] // ★修正（-5ではなく0）
        public void DecreaseRemainingTime_MultipleScenarios(float initialTime, float deltaTime, float expectedTime)
        {
            var currentState = new GameMainState(new RemainingTime(initialTime));
            var reducer = GameMainStateReducer.DecreaseRemainingTime(deltaTime);

            var newState = reducer(currentState);

            Assert.AreEqual(expectedTime, newState.RemainingTime.Value, 0.0001f);
        }

        [Test]
        public void DecreaseRemainingTime_ShouldHandleZeroDeltaTime()
        {
            var currentState = new GameMainState(new RemainingTime(30f));
            var reducer = GameMainStateReducer.DecreaseRemainingTime(0f);

            var newState = reducer(currentState);

            Assert.AreEqual(30f, newState.RemainingTime.Value, 0.0001f);
        }

        [Test]
        public void DecreaseRemainingTime_ShouldHandleVerySmallDeltaTime()
        {
            var currentState = new GameMainState(new RemainingTime(60f));
            var deltaTime = 0.0001f;
            var reducer = GameMainStateReducer.DecreaseRemainingTime(deltaTime);

            var newState = reducer(currentState);

            Assert.AreEqual(60f - deltaTime, newState.RemainingTime.Value, 0.00001f);
        }

        [Test]
        public void DecreaseRemainingTime_MultipleCallsShouldStackCorrectly()
        {
            var currentState = new GameMainState(new RemainingTime(30f));
            var reducer1 = GameMainStateReducer.DecreaseRemainingTime(0.016f);
            var reducer2 = GameMainStateReducer.DecreaseRemainingTime(0.016f);

            var afterFirst = reducer1(currentState);
            var afterSecond = reducer2(afterFirst);

            Assert.AreEqual(30f - 0.032f, afterSecond.RemainingTime.Value, 0.0001f);
        }
    }
}