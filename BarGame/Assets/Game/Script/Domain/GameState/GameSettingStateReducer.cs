using System;

namespace Game.Domain
{
    public static class GameSettingStateReducer
    {
        public static Func<GameSettingState, GameSettingState> Initialize() => _ => GameSettingState.Default;
        public static Func<GameSettingState, GameSettingState> Update(GameSettingState newState) => _ => newState;

        public static Func<GameSettingState, GameSettingState> SetBGMVolume(AudioVolume value) => state => state.WithBGMVolume(value);
        public static Func<GameSettingState, GameSettingState> SetSEVolume(AudioVolume value) => state => state.WithSEVolume(value);
    }
}