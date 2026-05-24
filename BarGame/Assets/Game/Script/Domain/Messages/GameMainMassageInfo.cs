namespace Game.Domain
{
    /// <summary>
    /// ゲーム開始メッセージ
    /// </summary>
    public readonly struct GameStartedMessage
    {
    }

    /// <summary>
    /// ゲーム一時停止メッセージ
    /// </summary>
    public readonly struct GamePauseMessage
    {
        public bool IsPausing { get; }

        public GamePauseMessage(bool isPausing)
        {
            IsPausing = isPausing;
        }
    }

    /// <summary>
    /// ゲーム終了メッセージ
    /// </summary>
    public readonly struct GameFinishedMessage
    {
        public GameMainLoopResult Result { get; }
        
        public GameFinishedMessage(GameMainLoopResult result)
        {
            Result = result;
        }
    }
}