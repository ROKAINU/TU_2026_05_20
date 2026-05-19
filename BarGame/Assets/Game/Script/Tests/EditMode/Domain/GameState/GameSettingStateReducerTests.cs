using NUnit.Framework;
using Game.Domain;

namespace Game.Domain.Tests
{
    [TestFixture]
    public class GameSettingStateReducerTests
    {
        [Test]
        public void Initialize_ShouldReturnDefaultState()
        {
            var currentState = new GameSettingState(
                new AudioVolume(0.5f),
                new AudioVolume(0.7f));

            var reducer = GameSettingStateReducer.Initialize();

            var newState = reducer(currentState);

            Assert.AreEqual(GameSettingState.Default.BGMVolume.Value, newState.BGMVolume.Value, 0.0001f);
            Assert.AreEqual(GameSettingState.Default.SEVolume.Value, newState.SEVolume.Value, 0.0001f);
        }

        [Test]
        public void Update_ShouldReplaceStateWithNewState()
        {
            var currentState = new GameSettingState(
                new AudioVolume(0.3f),
                new AudioVolume(0.4f));

            var newState = new GameSettingState(
                new AudioVolume(0.8f),
                new AudioVolume(0.9f));

            var reducer = GameSettingStateReducer.Update(newState);

            var result = reducer(currentState);

            Assert.AreEqual(0.8f, result.BGMVolume.Value, 0.0001f);
            Assert.AreEqual(0.9f, result.SEVolume.Value, 0.0001f);
        }

        [Test]
        public void SetBGMVolume_ShouldSetBGMVolumeValue()
        {
            var currentState = new GameSettingState(
                new AudioVolume(0.5f),
                new AudioVolume(0.5f));

            var reducer = GameSettingStateReducer.SetBGMVolume(new AudioVolume(0.8f));

            var newState = reducer(currentState);

            Assert.AreEqual(0.8f, newState.BGMVolume.Value, 0.0001f);
            Assert.AreEqual(0.5f, newState.SEVolume.Value, 0.0001f);
        }

        [Test]
        public void SetSEVolume_ShouldSetSEVolumeValue()
        {
            var currentState = new GameSettingState(
                new AudioVolume(0.5f),
                new AudioVolume(0.5f));

            var reducer = GameSettingStateReducer.SetSEVolume(new AudioVolume(0.6f));

            var newState = reducer(currentState);

            Assert.AreEqual(0.5f, newState.BGMVolume.Value, 0.0001f);
            Assert.AreEqual(0.6f, newState.SEVolume.Value, 0.0001f);
        }

        [Test]
        public void SetBGMVolume_WithVariousValues_ShouldSetCorrectly()
        {
            var currentState = new GameSettingState(
                new AudioVolume(0.5f),
                new AudioVolume(0.5f));

            var reducer = GameSettingStateReducer.SetBGMVolume(new AudioVolume(0.8f));

            var newState = reducer(currentState);

            Assert.AreEqual(0.8f, newState.BGMVolume.Value, 0.0001f);
        }

        [Test]
        public void SetSEVolume_WithVariousValues_ShouldSetCorrectly()
        {
            var currentState = new GameSettingState(
                new AudioVolume(0.5f),
                new AudioVolume(0.5f));

            var reducer = GameSettingStateReducer.SetSEVolume(new AudioVolume(0.6f));

            var newState = reducer(currentState);

            Assert.AreEqual(0.6f, newState.SEVolume.Value, 0.0001f);
        }

        [Test]
        public void SetBGMVolume_ClampsValuesAbove1()
        {
            var currentState = new GameSettingState(
                new AudioVolume(0.5f),
                new AudioVolume(0.5f));

            var reducer = GameSettingStateReducer.SetBGMVolume(new AudioVolume(1.5f));

            var newState = reducer(currentState);

            Assert.AreEqual(1.0f, newState.BGMVolume.Value, 0.0001f);
        }

        [Test]
        public void SetBGMVolume_ClampsNegativeValues()
        {
            var currentState = new GameSettingState(
                new AudioVolume(0.5f),
                new AudioVolume(0.5f));

            var reducer = GameSettingStateReducer.SetBGMVolume(new AudioVolume(-0.5f));

            var newState = reducer(currentState);

            Assert.AreEqual(0.0f, newState.BGMVolume.Value, 0.0001f);
        }

        [Test]
        public void SetBothVolumes_SequentiallyApplied()
        {
            var currentState = new GameSettingState(
                new AudioVolume(0.5f),
                new AudioVolume(0.5f));

            var bgmReducer = GameSettingStateReducer.SetBGMVolume(new AudioVolume(0.7f));
            var seReducer = GameSettingStateReducer.SetSEVolume(new AudioVolume(0.3f));

            var afterBGM = bgmReducer(currentState);
            var afterBoth = seReducer(afterBGM);

            Assert.AreEqual(0.7f, afterBoth.BGMVolume.Value, 0.0001f);
            Assert.AreEqual(0.3f, afterBoth.SEVolume.Value, 0.0001f);
        }
    }
}