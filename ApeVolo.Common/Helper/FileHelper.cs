using ApeVolo.Common.Extention;
using System;
using System.IO;
using System.Text;

namespace ApeVolo.Common.Helper
{
    /// <summary>
    /// 文件操作帮助类
    /// </summary>
    public static class FileHelper
    {
        #region 读操作

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="path">文件目录</param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 获取当前程序根目录
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        #endregion

        #region 写操作

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用系统默认编码;若文件不存在则创建新的,若存在则覆盖
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        public static void WriteTxt(string content, string path)
        {
            WriteTxt(content, path, null, null);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义编码;若文件不存在则创建新的,若存在则覆盖
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码</param>
        public static void WriteTxt(string content, string path, Encoding encoding)
        {
            WriteTxt(content, path, encoding, null);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义模式,使用UTF-8编码
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="fileModel">输出方法</param>
        public static void WriteTxt(string content, string path, FileMode fileModel)
        {
            WriteTxt(content, path, Encoding.UTF8, fileModel);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义编码以及写入模式
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="fileModel">写入模式</param>
        public static void WriteTxt(string content, string path, Encoding encoding, FileMode fileModel)
        {
            WriteTxt(content, path, encoding, (FileMode?)fileModel);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义编码以及写入模式
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="fileModel">写入模式</param>
        private static void WriteTxt(string content, string path, Encoding encoding, FileMode? fileModel)
        {
            encoding = encoding ?? Encoding.UTF8;
            fileModel = fileModel ?? FileMode.Create;
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (FileStream fileStream = new FileStream(path, fileModel.Value))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, encoding))
                {
                    streamWriter.Write(content);
                    streamWriter.Flush();
                }
            }
        }

        /// <summary>
        /// 输出日志到指定文件
        /// </summary>
        /// <param name="msg">日志消息</param>
        /// <param name="path">日志文件位置（默认为D:\测试\a.log）</param>
        public static void WriteLog(string msg, string path = @"Log.txt")
        {
            string content = $"{DateTime.Now.ToCstTime().ToString("yyyy-MM-dd HH:mm:ss")}:{msg}";

            WriteTxt(content, $"{GetCurrentDir()}{content}");
        }


        /// <summary>
        /// 流写入
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="fileFullPath">文件完整物理路径</param>
        /// <param name="e">编码</param>
        /// <param name="isAppend">默认追加，false覆盖</param>
        public static void WriteText(string content, string fileFullPath, Encoding e, bool isAppend = true)
        {
            var dn = Path.GetDirectoryName(fileFullPath);
            //检测目录
            if (!Directory.Exists(dn))
            {
                Directory.CreateDirectory(dn);
            }

            //打开方式
            var fm = (!File.Exists(fileFullPath) || !isAppend) ? FileMode.Create : FileMode.Append;

            using var fs = new FileStream(fileFullPath, fm);
            //流写入
            using var sw = new StreamWriter(fs, e);
            sw.WriteLine(content);
        }
        #endregion

        #region
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="fileFullPath">文件完整物理路径</param>
        /// <param name="e">编码 默认UTF8</param>
        /// <returns></returns>
        public static string ReadText(string fileFullPath, Encoding e = null)
        {
            var result = string.Empty;

            if (File.Exists(fileFullPath))
            {
                if (e == null)
                {
                    e = Encoding.UTF8;
                }

                result = File.ReadAllText(fileFullPath, e);
            }

            return result;
        }

        public static string ReadFile(string path, Encoding encode)
        {
            string s = "";
            if (!File.Exists(path))
                s = "不存在相应的目录";
            else
            {
                StreamReader f2 = new StreamReader(path, encode);
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }

            return s;
        }
        #endregion
    }
}
