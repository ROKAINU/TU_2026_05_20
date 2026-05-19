namespace Game.Domain
{
    public readonly struct GameMainState
    {
        //Template
        public RemainingTime RemainingTime { get; }

        public GameMainState(RemainingTime remainingTime)
        {
            RemainingTime = remainingTime;
        }

        public GameMainState WithRemainingTime(RemainingTime value) => new(value);

        public static readonly GameMainState Default = new(new RemainingTime(3f));
    }

    public readonly struct RemainingTime
    {
        public float Value { get; }

        public RemainingTime(float value)
        {
            Value = System.Math.Max(0, value);
        }

        public RemainingTime Subtract(float delta) => new(Value - delta);
    }
}