namespace Game.Domain
{
    /// <summary>
    /// ゲームコマンド（Union パターン）
    /// </summary>
    public abstract class GameCommand
    {
        /// <summary>ゲーム再開</summary>
        public sealed class Resume : GameCommand {}

        /// <summary>ゲーム一時停止</summary>
        public sealed class Pause : GameCommand {}

        /// <summary>スコア加算</summary>
        public sealed class AddScore : GameCommand
        {
            public int Amount { get; }

            public AddScore(int amount)
            {
                Amount = amount;
            }
        }

        /// <summary>JSON 表示</summary>
        public sealed class ShowJson : GameCommand
        {
            public int Id { get; }

            public ShowJson(int id)
            {
                Id = id;
            }
        }
    }
}