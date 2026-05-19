#nullable enable

using System;

namespace Game.Kernel
{
    /// <summary>
    /// ロギングレベルの列挙型。
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4,
    }

    /// <summary>
    /// ロギング抽象インターフェース。
    /// Unity非依存の純粋なログ契約。
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 現在のログレベル。このレベル以上のログのみ出力される。
        /// </summary>
        LogLevel CurrentLogLevel { get; set; }

        /// <summary>
        /// ログを出力。
        /// </summary>
        void Log(LogLevel level, string message);

        /// <summary>
        /// 例外をログに記録。
        /// </summary>
        void LogException(Exception exception, string? message = null);

        /// <summary>
        /// フォーマット済みログを出力。
        /// </summary>
        void LogFormat(LogLevel level, string format, params object?[] args);

        /// <summary>
        /// 指定ログレベルが出力対象か確認。
        /// </summary>
        bool IsEnabled(LogLevel level);

        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogFatal(string message);
    }
}