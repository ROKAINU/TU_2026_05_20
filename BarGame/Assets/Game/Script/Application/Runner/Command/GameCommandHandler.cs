using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Kernel;
using Game.Kernel.Utils.Cysharp;


namespace Game.Application.Runner
{
    /// <summary>
    /// ゲームコマンドを処理するハンドラー。
    /// </summary>
    public sealed class GameCommandHandler : ICommandHandler<GameCommand>
    {
        private readonly IGameCommandProcessor[] _processors;
        private readonly LoggerBase _logger;

        public GameCommandHandler(
            IGameCommandProcessor[] processors,
            LoggerBase logger)
        {
            _processors = processors;
            _logger     = logger;
        }

        /// <summary>
        /// ゲームコマンドを処理する。ここでは、ゲームの状態を変更するコマンドを処理する。
        /// </summary>
        /// <param name="command">処理するゲームコマンド</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns></returns>
        /// awaitを用いる場合public async UniTask/await UniTask.CompletedTask;にそれぞれ変更する。
        public UniTask HandleAsync(GameCommand command, CancellationToken token)
        {
            foreach (var processor in _processors)
            {
                if (processor.CanProcess(command))
                    return processor.ProcessAsync(command, token);
            }
            
            // コマンドが見つからない場合はログして無視
            _logger.LogWarning($"[GameCommandHandler] Unknown command: {command.GetType().Name}");
            return UniTask.CompletedTask;
        }
    }
}