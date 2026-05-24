using System.Collections.Generic;

namespace Game.Application.Contracts
{
    /// <summary>
    /// ストーリーの抽象表現（Ink非依存）
    /// </summary>
    public interface IScenario
    {
        bool CanContinue { get; }
        bool HasChoices { get; }
        string GetNextLine();
        IReadOnlyList<string> GetChoices();
        void SelectChoice(int index);
    }
}