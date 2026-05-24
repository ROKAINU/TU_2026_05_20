using System.Threading;
using System.Threading.Tasks;
using Game.Domain;

namespace Game.Application.Contracts
{
    /// <summary>ゲーム内の各フェーズが実装するインターフェース</summary>
    public interface IInGamePhase
    {
        /// <summary>
        /// フェーズの毎フレーム更新
        /// </summary>
        /// <param name="dt">経過時間 DeltaTime</param>
        /// <param name="ct">キャンセル トークン</param>
        /// <returns>ゲーム終了結果。継続中は null</returns>
        Task<GameMainLoopResult?> TickAsync(float dt, CancellationToken ct);
    }
}