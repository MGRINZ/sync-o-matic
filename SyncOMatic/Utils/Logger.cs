using System;
using System.IO;

namespace SyncOMatic.Utils
{
    public static class Logger
    {
        private static readonly string logfile = "log.log";

        private static void Log(string msg, string tag)
        {
            DateTime date = DateTime.Now;
            File.AppendAllText(logfile, $"{date.ToString()}: [{tag}] {msg}\n");
        }

        private static void LogException(Exception e, string tag)
        {
            string msg = $"{e.GetType().Name} {e.Message}\n{e.StackTrace}";
            Log(msg, tag);
        }

        public static void LogInfo(string msg)
        {
            Log(msg, "INFO");
        }
        public static void LogInfo(Exception e)
        {
            LogException(e, "INFO");
        }

        public static void LogWarning(string msg)
        {
            Log(msg, "WARNING");
        }
        public static void LogWarning(Exception e)
        {
            LogException(e, "WARNING");
        }

        public static void LogError(string msg)
        {
            Log(msg, "ERROR");
        }

        public static void LogError(Exception e)
        {
            LogException(e, "ERROR");
        }
    }
}