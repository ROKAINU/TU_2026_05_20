using Cysharp.Threading.Tasks;
using System.Threading;
using Game.Domain;
using Game.Application.Contracts;

namespace Game.Application.Contracts.Cysharp
{ 
    /// <summary>
    /// ストーリーシナリオを実行するユースケース
    /// Ink知らない、Unity知らない、純粋なロジック
    /// </summary>
    public interface IStoryRunner
    {
        UniTask RunScenarioAsync(
            IScenario scenario,
            IGameStateContainer gameState,
            IStoryDisplay display,
            IAdvanceInput input,
            CancellationToken cancellationToken);
    }
}