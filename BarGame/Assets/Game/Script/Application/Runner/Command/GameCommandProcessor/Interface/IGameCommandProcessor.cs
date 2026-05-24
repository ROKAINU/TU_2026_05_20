using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;

namespace Game.Application.Runner
{
    /// <summary>
    /// ゲームコマンドを処理する戦略インターフェース
    /// </summary>
    public interface IGameCommandProcessor
    {
        bool CanProcess(GameCommand command);
        UniTask ProcessAsync(GameCommand command, CancellationToken token);
    }
}