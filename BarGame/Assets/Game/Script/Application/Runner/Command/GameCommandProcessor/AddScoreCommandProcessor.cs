using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Kernel;
using Game.Kernel.Utils.Cysharp;

namespace Game.Application.Runner
{
    /// <summary>
    /// AddScore コマンド処理
    /// </summary>
    public sealed class AddScoreCommandProcessor : IGameCommandProcessor
    {
        private readonly IStore<GameGlobalState> _globalStore;

        public AddScoreCommandProcessor(IStore<GameGlobalState> globalStore)
            => _globalStore = globalStore;

        public bool CanProcess(GameCommand command)
            => command is GameCommand.AddScore;

        public UniTask ProcessAsync(GameCommand command, CancellationToken token)
        {
            if (command is GameCommand.AddScore addScore)
                _globalStore.Dispatch(GameGlobalStateReducer.AddScore(addScore.Amount));

            return UniTask.CompletedTask;
        }
    }
}