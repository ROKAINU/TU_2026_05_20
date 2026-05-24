namespace Game.Application.Contracts
{
    /// <summary>
    /// UI表示インターフェース（抽象）
    /// </summary>
    public interface IStoryDisplay
    {
        void DisplayText(string text);
        void DisplayName(string name);
        void DisplayBackground(string backgroundId);
        void DisplayChoices(string[] choices, System.Action<int> onSelected);
        void ClearChoices();
    }
}