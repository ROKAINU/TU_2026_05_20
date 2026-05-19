namespace Game.Domain
{
    /// <summary>
    /// ゲームのメインループの結果を表す列挙型。ゲームの状態遷移や終了条件を明確にするために使用される。
    /// </summary>
    public enum GameMainLoopResult
    {
        Cleared,
        GameFinished,
        Canceled,
        ToResult,
        ToTitle,
        Restart,
    }
}