using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ape.Volo.Common.ClassLibrary;

namespace Ape.Volo.Common.Helper;

/// <summary>
/// 日志操作类
/// </summary>
public static class LogHelper
{
    private static readonly UsingLock<object> Lock = new();


    /// <summary>
    /// 文本日志
    /// </summary>
    /// <param name="folder">文件夹</param>
    /// <param name="message">消息</param>   
    public static void WriteError(string message, IEnumerable<string> folder)
    {
        AddLog(message, folder);
    }

    /// <summary>
    /// 写日志文件数据库日志文件
    /// </summary>
    /// <param name="folder">文件夹</param>
    /// <param name="message">消息</param> 
    public static void WriteLog(string message, IEnumerable<string> folder)
    {
        AddLog(message, folder);
    }

    /// <summary>
    /// 文本日志
    /// </summary>
    /// <param name="folder">文件夹</param>
    /// <param name="message">日志存储目录名称</param>
    private static void AddLog(string message, IEnumerable<string> folder)
    {
        try
        {
            var path = Path.Combine(App.WebHostEnvironment.ContentRootPath, "Logs");
            path = folder.Aggregate(path, Path.Combine);

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
                var sw = new StreamWriter(fs);
                sw.Close();
                fs.Close();
            }

            using (Lock.Write())
            {
                using var writer = new StreamWriter(logFilePath, true);
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                writer.WriteLine(message);
                writer.WriteLine(Environment.NewLine);
            }
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// SQL日志
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="dataParas"></param>
    public static void WriteSqlLog(string filename, IEnumerable<string> dataParas)
    {
        try
        {
            var path = Path.Combine(App.WebHostEnvironment.ContentRootPath, "Logs", "Sql");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var logFilePath = Path.Combine(path, $@"{filename}.log");

            var logContent =
                DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm fff") + "\r\n" +
                string.Join("", dataParas) + "\r\n\r\n\r\n\r\n";
            using (Lock.Write())
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
