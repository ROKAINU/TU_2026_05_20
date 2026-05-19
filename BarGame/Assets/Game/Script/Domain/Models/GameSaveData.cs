namespace Game.Domain
{
    public readonly struct GameSaveData
    {
        //PlayData
        public int TestScore { get; }
        public int TestHighScore { get; }
        
        public GameSaveData(
            int testScore,
            int testHighScore)
        {
            TestScore = testScore;
            TestHighScore = testHighScore;
        }

        /// <summary>
        /// GameGlobalState から GameSaveData を生成
        /// </summary>
        public static GameSaveData FromGlobalState(GameGlobalState globalState)
        {
            return new GameSaveData(
                testScore: globalState.CurrentScore.Value,
                testHighScore: globalState.HighScore.Value
            );
        }

        public static GameSaveData Default() => new GameSaveData(
            testScore: 0,
            testHighScore: 0
        );

        public GameSaveData With(
            int? testScore = null,
            int? testHighScore = null)
        {
            return new GameSaveData(
            testScore: testScore ?? TestScore,
            testHighScore: testHighScore ?? TestHighScore
            );
        }
    }
}