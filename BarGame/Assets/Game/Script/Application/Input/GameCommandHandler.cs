using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Kernel.Utils.Cysharp;
using Game.Kernel.Utils.R3;
using Game.Kernel;
using VContainer;


namespace Game.Application
{
    /// <summary>
    /// ゲームコマンドを処理するハンドラー。
    /// </summary>
    public sealed class GameCommandHandler : ICommandHandler<GameCommand>
    {
        private readonly Store<GameGlobalState> _globalStore;

        public GameCommandHandler(Store<GameGlobalState> globalStore)
        {
            _globalStore = globalStore;
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
            switch (command.Type)
            {
                case GameCommandType.AddScore:
                    _globalStore.Dispatch(GameGlobalStateReducer.AddScore(1));
                    break;
                default:
                    break;
            }

            return UniTask.CompletedTask;
        }
    }

    /// <summary>
    /// ゲームコマンドの種類
    /// </summary>
    public enum GameCommandType
    {
        Resume,
        Pause,
        AddScore
        // 他のコマンドタイプを追加可能
    }

    /// <summary>
    /// 実際に実行されるゲームコマンド
    /// </summary>
    public readonly struct GameCommand
    {
        public GameCommandType Type { get; }
        public GameCommand(GameCommandType type) => Type = type;
    }
}