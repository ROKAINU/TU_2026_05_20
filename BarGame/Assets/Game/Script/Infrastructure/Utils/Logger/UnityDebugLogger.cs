#nullable enable
using System;
using UnityEngine;
using Game.Kernel;

namespace Game.Infrastructure
{
    public class UnityDebugLogger : LoggerBase
    {
        public override LogLevel CurrentLogLevel { get; set; } = LogLevel.Debug;

        public override void LogException(Exception exception, string? message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var prefix = $"[{DateTime.Now:HH:mm:ss}] [Error] ";
                Debug.LogError(prefix + message);
            }

            Debug.LogException(exception);
        }

        public override void Log(LogLevel level, string message)
        {
            if (!IsEnabled(level))
                return;

            var prefix = $"[{DateTime.Now:HH:mm:ss}] [{level}] ";
            var fullMessage = prefix + message;

            switch (level)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                    Debug.Log(fullMessage);
                    break;

                case LogLevel.Warning:
                    Debug.LogWarning(fullMessage);
                    break;

                case LogLevel.Error:
                case LogLevel.Fatal:
                    Debug.LogError(fullMessage);
                    break;
            }
        }
    }
}