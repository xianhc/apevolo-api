using System;
using System.IO;
using ApeVolo.Common.ClassLibrary;

namespace ApeVolo.Common.Helper
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    public class LogHelper
    {
        static UsingLock<object> _lock;
        static string _contentRoot = string.Empty;

        public LogHelper(string contentPath)
        {
            _contentRoot = contentPath;
            _lock = new UsingLock<object>();
        }

        /// <summary>
        /// 写日志文件数据库日志文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="message">消息</param>   
        public static void WriteError(string message, string[] folder)
        {
            AddLog(message, folder);
        }

        /// <summary>
        /// 写日志文件数据库日志文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="message">消息</param> 
        public static void WriteLog(string message, string[] folder)
        {
            AddLog(message, folder);
        }

        /// <summary>
        /// 写日志文件数据库日志文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="message">日志存储目录名称</param>
        private static void AddLog(string message, string[] folder)
        {
            try
            {
                var path = Path.Combine(_contentRoot, "Logs");
                foreach (var t in folder)
                {
                    path = Path.Combine(path, t);
                }

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string logFilePath = Path.Combine(path, $@"{DateTime.Now:yyyyMMdd}.log");
                //只保留30天的日志
                var deletePath = Path.Combine(path, $@"{DateTime.Now.AddDays(-30):yyyyMMdd}.log");
                if (File.Exists(deletePath))
                {
                    File.Delete(deletePath);
                }

                if (!File.Exists(logFilePath))
                {
                    using var fs = new FileStream(logFilePath, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Close();
                    fs.Close();
                }

                using (_lock.Write())
                {
                    using StreamWriter writer = new StreamWriter(logFilePath, true);
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    writer.WriteLine(message);
                    writer.WriteLine(Environment.NewLine);
                }
            }
            catch
            {
                // ignored
            }
        }

        public static void WriteSqlLog(string filename, string[] dataParas, bool isHeader = true)
        {
            try
            {
                var path = Path.Combine(_contentRoot, "SqlLogs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string logFilePath = Path.Combine(path, $@"{filename}.log");

                string logContent = string.Join("\r\n", dataParas);
                if (isHeader)
                {
                    logContent =
                        "--------------------------------\r\n" +
                        DateTime.Now + "|\r\n" +
                        string.Join("\r\n", dataParas) + "\r\n"
                        ;
                }

                using (_lock.Write())
                {
                    File.AppendAllText(logFilePath, logContent);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}