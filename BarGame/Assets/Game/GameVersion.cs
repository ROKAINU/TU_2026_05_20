namespace Game
{
    /// <summary>
    /// ゲームのバージョン情報を定義するクラス。
    /// セーブデータの互換性チェックなどに使用。
    /// </summary>
    public static class GameVersion
    {
        public const int Major = 1;
        public const int Minor = 0;
        public const int Patch = 0;

        public static string Value => $"{Major}.{Minor}.{Patch}";
    }
}