using System;

namespace Game.Domain
{
    public static class GameGlobalStateReducer
    {
        public static Func<GameGlobalState, GameGlobalState> Initialize() => _ => GameGlobalState.Default;
        public static Func<GameGlobalState, GameGlobalState> Update(GameGlobalState newState) => _ => newState;

        //Template
        public static Func<GameGlobalState, GameGlobalState> AddScore(int value)
            => state =>
            {
                var newScore = state.CurrentScore.Add(value);
                var newHighScore = new Score(Math.Max(state.HighScore.Value, newScore.Value));

                return new GameGlobalState(newScore, newHighScore);
            };
        public static Func<GameGlobalState, GameGlobalState> ClearScore() => state => new GameGlobalState(Score.Zero, state.HighScore);
    }
}