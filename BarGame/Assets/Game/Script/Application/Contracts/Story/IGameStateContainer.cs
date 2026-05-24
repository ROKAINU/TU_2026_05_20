namespace Game.Application.Contracts
{
    /// <summary>
    /// ストーリーで変更されるゲーム状態
    /// </summary>
    public interface IGameStateContainer
    {
        void SetVariable(string key, object value);
        object GetVariable(string key);
    }
}