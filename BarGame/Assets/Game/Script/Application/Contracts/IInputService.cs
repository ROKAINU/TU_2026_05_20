using Game.Kernel;

namespace Game.Application.Contracts
{
    /// <summary>
    /// ゲーム入力の抽象インターフェース
    /// 実装: RealInputService, MockInputService
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// クリック入力があったか
        /// </summary>
        bool GetAddScoreInput();

        /// <summary>
        /// 移動入力を取得
        /// </summary>
        Coord2D GetMovementInput();

        /// <summary>
        /// このフレームでポーズ入力があるか。
        /// </summary>
        bool IsPausePressed();

        /// <summary>
        /// このフレームでJSON表示入力があるか。
        /// </summary>
        /// <returns>押された数字キーの値</returns>
        public int IsShowJsonPressed();

        /// <summary>
        /// その他必要な入力...
        /// </summary>
    }
}