using System;
using System.IO;

namespace LosSantosUnderground.Utilities
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        private static readonly string LogDirectory =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Logs");

        private static readonly string LogFile =
            Path.Combine(
                LogDirectory,
                "LosSantosUnderground.log");

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warning(string message)
        {
            Write("WARNING", message);
        }

        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        public static void Debug(string message)
        {
            Write("DEBUG", message);
        }

        private static void Write(
            string level,
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            try
            {
                lock (_lock)
                {
                    Directory.CreateDirectory(
                        LogDirectory);

                    string line =
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                        $"[{level}] {message}";

                    File.AppendAllText(
                        LogFile,
                        line + Environment.NewLine);
                }
            }
            catch
            {
                // Logging must never crash the mod.
            }
        }

        public static void Clear()
        {
            try
            {
                lock (_lock)
                {
                    if (File.Exists(LogFile))
                        File.Delete(LogFile);
                }
            }
            catch
            {
            }
        }

        public static string GetLogFilePath()
        {
            return LogFile;
        }
    }
}