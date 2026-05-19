namespace Game.Domain
{
    using Game.Kernel;

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
        /// その他必要な入力...
        /// </summary>
    }
}