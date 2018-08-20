using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace FinanceManagement.Utilities
{
    static class Logger
    {
        public enum EventType
        {
            Start,
            Comment,
            Exception,
            Stop
        }

        private static ILogger _logger;
        public static ILogger implementation { get { return _logger ?? (_logger = new FileLogger()); } set { _logger = value; } }
        public static void Start()
        {
            implementation.Start();
        }
        public static void Log(string message, [CallerMemberName] string memberName = "")
        {
            implementation.Log(message, memberName);
        }
        public static void LogException(Exception ex, [CallerMemberName] string memberName = "")
        {
            implementation.LogException(ex, memberName);
        }
        public static void Stop()
        {
            implementation.Stop();
        }
    }

    interface ILogger
    {
        void Start();
        void Log(string message, string callingMethod);
        void LogException(Exception ex, string callingMethod);
        void Stop();
        void Delete(int days);
    }

    class FileLogger : ILogger
    {
        private readonly string _file = string.Concat(DateTime.Today, ".txt");
        private string _saveDirectory;
        private string _saveFile;
        public FileLogger()
        {
            SetDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"), true);
        }
        public FileLogger(string directory)
        {
            SetDirectory(directory, true);
        }
        public void SetDirectory(string path, bool absolute)
        {
            if (absolute)
            {
                _saveDirectory = path;
            }
            else
            {
                _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }
            _saveFile = Path.Combine(_saveDirectory, string.Concat(DateTime.Today.ToString("yyyy-MM-dd"), ".txt"));
            Directory.CreateDirectory(_saveDirectory);
        }
        public void Start()
        {
            using (StreamWriter writer = File.AppendText(_saveFile))
            {
                writer.WriteLineAsync(string.Concat(DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"), ", Beginning logging"));
            }

        }
        public void Log(string message, string memberName)
        {
            using (StreamWriter writer = File.AppendText(_saveFile))
            {
                writer.WriteLineAsync(string.Concat(DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"), ", ", memberName, ", ", message));
            }
        }
        public void LogException(Exception ex, string memberName)
        {
            using (StreamWriter writer = File.AppendText(_saveFile))
            {
                writer.WriteLineAsync(string.Concat(DateTime.Now.ToString("yyyy-MM-ddTHH-mm-s"), ", ", memberName, ", ", ex.ToString()));
            }
        }
        public void Stop()
        {
            using (StreamWriter writer = File.AppendText(_saveFile))
            {
                writer.WriteLineAsync(string.Concat(DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"), ", Ending logging"));
            }
        }
        public void Delete(int days)
        {
            foreach (string file in Directory.GetFiles(_saveDirectory))
            {
                if (File.GetCreationTime(file) < DateTime.Now.AddDays(-days))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
