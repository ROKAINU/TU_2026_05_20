namespace Game.Kernel
{
    public sealed class NullLogger : LoggerBase
    {
        public override LogLevel CurrentLogLevel { get; set; } = LogLevel.Fatal;

        public override void Log(LogLevel level, string message)
        {
            // 何もしない
        }
    }
}