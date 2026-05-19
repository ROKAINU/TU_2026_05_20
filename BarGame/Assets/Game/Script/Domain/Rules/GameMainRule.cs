namespace Game.Domain
{
    /// <summary>
    /// GameMainState に関するゲームルール
    /// </summary>
    public static class GameMainRule
    {
        /// <summary>制限時間切れか</summary>
        public static bool IsTimeUp(GameMainState state)
            => state.RemainingTime.Value <= 0f;
    }
}