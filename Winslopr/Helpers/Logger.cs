using System;
using System.Collections.Generic;

namespace Winslopr
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Custom
    }

    public class LogEntry
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Framework-agnostic logger. Stores entries in memory and raises events
    /// so any UI (WinUI, console, etc.) can subscribe and display them.
    /// </summary>
    public static class Logger
    {
        private static readonly List<LogEntry> _entries = new();

        public static event Action<LogEntry> LogAdded;

        public static event Action LogCleared;

        public static IReadOnlyList<LogEntry> Entries => _entries;

        public static string FullText
        {
            get
            {
                var sb = new System.Text.StringBuilder();
                foreach (var e in _entries)
                    sb.AppendLine(e.Message);
                return sb.ToString();
            }
        }

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            var entry = new LogEntry
            {
                Message = message,
                Level = level,
                Timestamp = DateTime.Now
            };
            _entries.Add(entry);
            LogAdded?.Invoke(entry);
        }

        public static void BeginSection(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                sectionName = "Unnamed Section";

            Log(string.Empty);
            Log("===== " + sectionName.ToUpper() +
                " (" + DateTime.Now.ToString("HH:mm:ss") + ") =====");
            Log(string.Empty);
        }

        public static void Clear()
        {
            _entries.Clear();
            LogCleared?.Invoke();
        }
    }
}