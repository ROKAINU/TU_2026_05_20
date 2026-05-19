#nullable enable

using System;

namespace Game.Kernel
{
    /// <summary>
    /// ロギング基底クラス。
    /// ILogger のデフォルト実装を提供。
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        public abstract LogLevel CurrentLogLevel { get; set; }

        public abstract void Log(LogLevel level, string message);

        // デフォルト実装を提供
        public virtual void LogException(Exception exception, string? message = null)
        {
            var fullMessage = message != null
                ? $"{message}\n{exception}"
                : exception.ToString();
            Log(LogLevel.Error, fullMessage);
        }

        public virtual void LogFormat(LogLevel level, string format, params object?[] args)
        {
            if (!IsEnabled(level))
                return;

            try
            {
                var message = string.Format(format, args);
                Log(level, message);
            }
            catch
            {
                // ロギング失敗は無視
            }
        }

        public virtual bool IsEnabled(LogLevel level) => level >= CurrentLogLevel;

        #region Convenience Methods

        public virtual void LogDebug(string message)
        {
            if (IsEnabled(LogLevel.Debug))
                Log(LogLevel.Debug, message);
        }

        public virtual void LogInfo(string message)
        {
            if (IsEnabled(LogLevel.Info))
                Log(LogLevel.Info, message);
        }

        public virtual void LogWarning(string message)
        {
            if (IsEnabled(LogLevel.Warning))
                Log(LogLevel.Warning, message);
        }

        public virtual void LogError(string message)
        {
            if (IsEnabled(LogLevel.Error))
                Log(LogLevel.Error, message);
        }

        public virtual void LogFatal(string message)
        {
            if (IsEnabled(LogLevel.Fatal))
                Log(LogLevel.Fatal, message);
        }

        #endregion
    }
}