using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ApeVolo.Common.Helper;

public class CmdHelper
{
    /// <summary>  
    /// Windows操作系统，执行cmd命令
    /// 多命令请使用批处理命令连接符：  
    /// <![CDATA[  
    /// &:同时执行两个命令  
    /// |:将上一个命令的输出,作为下一个命令的输入  
    /// &&：当&&前的命令成功时,才执行&&后的命令  
    /// ||：当||前的命令失败时,才执行||后的命令
    /// ]]>
    /// </summary>  
    public static string Run(string cmdText, string cmdPath = "cmd.exe")
    {
        if (cmdPath == "cmd.exe")
        {
            cmdPath = Path.Combine(Environment.SystemDirectory, cmdPath);
        }

        string strOutput = "";

        //说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态  
        var cmd = cmdText + " &exit";
        using (var p = new Process())
        {
            p.StartInfo.FileName = cmdPath;
            p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动  
            p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息  
            p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息  
            p.StartInfo.RedirectStandardError = true; //重定向标准错误输出  
            p.StartInfo.CreateNoWindow = true; //不显示程序窗口  
            p.Start(); //启动程序  

            //向cmd窗口写入命令  
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;
            strOutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit(); //等待程序执行完退出进程  
            p.Close();
        }

        return strOutput;
    }

#if !NET40
    /// <summary>
    /// Linux操作系统，执行Shell
    /// 【 using https://github.com/phil-harmoniq/Shell.NET 】
    /// </summary>
    /// <param name="cmd"></param>
    public static BashResult Shell(string cmd)
    {
        return new Bash().Command(cmd);
    }

    /// <summary>
    /// 返回结果
    /// </summary>
    public class BashResult
    {
        /// <summary>
        /// The command's standard output as a string. (if redirected)</summary>
        public string Output { get; private set; }

        /// <summary>
        /// The command's error output as a string. (if redirected)</summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// The command's exit code as an integer.</summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// An array of the command's output split by newline characters. (if redirected)</summary>
        public string[] Lines => Output?.Split(Environment.NewLine.ToCharArray());

        internal BashResult(string output, string errorMsg, int exitCode)
        {
            Output = output?.TrimEnd(Environment.NewLine.ToCharArray());
            ErrorMsg = errorMsg?.TrimEnd(Environment.NewLine.ToCharArray());
            ExitCode = exitCode;
        }
    }

    /// <summary>
    /// 执行
    /// </summary>
    public class Bash
    {
        private static bool Plinux { get; }
        private static bool Pmac { get; }
        private static bool Pwindows { get; }
        private static string PbashPath { get; }

        /// <summary>Determines whether bash is running in a native OS (Linux/MacOS).</summary>
        /// <returns>True if in *nix, else false.</returns>
        public static bool Native { get; }

        /// <summary>Determines if using Windows and if Linux subsystem is installed.</summary>
        /// <returns>True if in Windows and bash detected.</returns>
        public static bool Subsystem => Pwindows && File.Exists(@"C:\Windows\System32\bash.exe");

        /// <summary>Stores output of the previous command if redirected.</summary>
        public string Output { get; private set; }

        /// <summary>
        /// Gets an array of the command output split by newline characters if redirected. </summary>
        public string[] Lines => Output?.Split(Environment.NewLine.ToCharArray());

        /// <summary>Stores the exit code of the previous command.</summary>
        public int ExitCode { get; private set; }

        /// <summary>Stores the error message of the previous command if redirected.</summary>
        public string ErrorMsg { get; private set; }

        static Bash()
        {
            Plinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            Pmac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            Pwindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            Native = Plinux || Pmac ? true : false;
            PbashPath = Native ? "bash" : "bash.exe";
        }

        /// <summary>Execute a new Bash command.</summary>
        /// <param name="input">The command to execute.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Command(string input, bool redirect = true)
        {
            if (!Native && !Subsystem)
                throw new PlatformNotSupportedException();

            using (var bash = new Process { StartInfo = BashInfo(input, redirect) })
            {
                bash.Start();

                if (redirect)
                {
                    Output = bash.StandardOutput.ReadToEnd()
                        .TrimEnd(Environment.NewLine.ToCharArray());
                    ErrorMsg = bash.StandardError.ReadToEnd()
                        .TrimEnd(Environment.NewLine.ToCharArray());
                }
                else
                {
                    Output = null;
                    ErrorMsg = null;
                }

                bash.WaitForExit();
                ExitCode = bash.ExitCode;
                bash.Close();
            }

            if (redirect)
                return new BashResult(Output, ErrorMsg, ExitCode);
            return new BashResult(null, null, ExitCode);
        }

        private ProcessStartInfo BashInfo(string input, bool redirectOutput)
        {
            return new ProcessStartInfo
            {
                FileName = PbashPath,
                Arguments = $"-c \"{input}\"",
                RedirectStandardInput = false,
                RedirectStandardOutput = redirectOutput,
                RedirectStandardError = redirectOutput,
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false
            };
        }
    }
#endif
}