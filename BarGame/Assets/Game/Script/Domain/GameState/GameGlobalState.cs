namespace Game.Domain
{
    public readonly struct GameGlobalState
    {
        public Score CurrentScore { get; }
        public Score HighScore { get; }

        public GameGlobalState(Score currentScore, Score highScore)
        {
            CurrentScore = currentScore;
            HighScore = highScore;
        }

        public GameGlobalState WithCurrentScore(Score value) => new(value, HighScore);
        public GameGlobalState WithHighScore(Score value) => new(CurrentScore, value);

        public static readonly GameGlobalState Default = new(Score.Zero, Score.Zero);
    }

    public readonly struct Score
    {
        public int Value { get; }

        public Score(int value)
        {
            Value = System.Math.Max(0, value);
        }

        public Score Add(int value) => new(Value + value);

        public static Score Zero => new(0);
    }
}