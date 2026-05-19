using System;

namespace Game.Domain
{
    public static class GameMainStateReducer
    {
        public static Func<GameMainState, GameMainState> Initialize() => _ => GameMainState.Default;
        public static Func<GameMainState, GameMainState> Update(GameMainState newState) => _ => newState;

        public static Func<GameMainState, GameMainState> DecreaseRemainingTime(float deltaTime)
            => state => state.WithRemainingTime(state.RemainingTime.Subtract(deltaTime));
    }
}