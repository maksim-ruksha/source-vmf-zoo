using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace SourceVmfZoo.Log
{
    public static class LogManager
    {
        public static string LogPath { get; set; }

        private static StringBuilder LogStringBuilder { get; }

        static LogManager()
        {
            LogPath = Directory.GetCurrentDirectory() + "\\" + ConfigurationManager.AppSettings["LogsFileName"];
            LogStringBuilder = new StringBuilder();
        }

        public static void Log(string message)
        {
            LogStringBuilder.Append($"[{DateTime.Now:T}] {message}\n");
        }

        public static void Save()
        {
            File.WriteAllText(LogPath, LogStringBuilder.ToString());
        }
    }
}