﻿using Microsoft.Extensions.Logging;

namespace MiniTwitch.Common;

public class DefaultMiniTwitchLogger<T> : ILogger<T>, ILogger
{
    public bool Enabled { get; set; } = true;
    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
    public string LoggingHeader { get; set; } = string.Empty;

    static readonly object _lock = new();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        lock (_lock)
        {
            Console.Write($"[{DateTimeOffset.Now:HH:mm:ss} ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(LoggingHeader);
            Console.Write(' ');
            Console.ForegroundColor = logLevel switch
            {
                LogLevel.Trace => ConsoleColor.Gray,
                LogLevel.Debug => ConsoleColor.DarkMagenta,
                LogLevel.Information => ConsoleColor.DarkCyan,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Critical => ConsoleColor.DarkYellow,
                _ => ConsoleColor.Red
            };

            Console.Write(logLevel switch
            {
                LogLevel.Trace => "Trace",
                LogLevel.Debug => "Debug",
                LogLevel.Information => "Information",
                LogLevel.Warning => "Warning",
                LogLevel.Error => "Error",
                LogLevel.Critical => "Critical",
                LogLevel.None => "None",
                _ => "[?????] "
            });

            Console.ResetColor();
            Console.Write("] ");
            string message = formatter(state, exception);
            Console.WriteLine(message);
            if (exception is not null)
                Console.WriteLine(exception);
        }
    }
    public bool IsEnabled(LogLevel logLevel) => this.Enabled && logLevel >= this.MinimumLevel;
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        this.LoggingHeader = state.ToString() ?? "<BAD STATE>";
        return new NoopDisposable(this);
    }

    class NoopDisposable(DefaultMiniTwitchLogger<T> logger) : IDisposable
    {
        readonly DefaultMiniTwitchLogger<T> _logger = logger;
        public void Dispose()
        {
            _logger.LoggingHeader = string.Empty;
        }
    }
}
