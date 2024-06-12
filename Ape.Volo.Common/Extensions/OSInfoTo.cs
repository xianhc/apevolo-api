using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Ape.Volo.Common.Helper;

namespace Ape.Volo.Common.Extensions;

/// <summary>
/// 系统信息
/// </summary>
public class OsInfoTo
{
    /// <summary>
    /// 确定当前操作系统是否为64位操作系统
    /// </summary>
    public bool Is64BitOperatingSystem { get; set; } = Environment.Is64BitOperatingSystem;

    /// <summary>
    /// 获取此本地计算机的NetBIOS名称
    /// </summary>
    public string MachineName { get; set; } = Environment.MachineName;

    /// <summary>
    /// 获取当前平台标识符和版本号
    /// </summary>
    public OperatingSystem OsVersion { get; set; } = Environment.OSVersion;

    /// <summary>
    /// 获取当前计算机上的物理处理器核心数量
    /// </summary>
    public int NumberOfCores { get; set; }

    /// <summary>
    /// 获取当前计算机上的物理处理器数量
    /// </summary>
    public int PhysicalProcessorCount { get; set; }

    /// <summary>
    /// 获取当前计算机上的逻辑处理器数量
    /// </summary>
    public int LogicProcessorCount { get; set; }

    public int ProcessorUtilizationRate { get; set; }

    /// <summary>
    /// 处理器名称
    /// </summary>
    public string ProcessorName { get; set; }

    /// <summary>
    /// 获取系统目录的标准路径
    /// </summary>
    public string SystemDirectory { get; set; } = Environment.SystemDirectory;

    /// <summary>
    /// 获取操作系统的内存页面中的字节数
    /// </summary>
    public int SystemPageSize { get; set; } = Environment.SystemPageSize;

    /// <summary>
    /// 获取自系统启动以来经过的毫秒数
    /// </summary>
    public long TickCount { get; set; }

    /// <summary>
    /// 获取与当前用户关联的网络域名
    /// </summary>
    public string UserDomainName { get; set; } = Environment.UserDomainName;

    /// <summary>
    /// 获取当前登录到操作系统的用户的用户名
    /// </summary>
    public string UserName { get; set; } = Environment.UserName;

    /// <summary>
    /// 获取公共语言运行时的主要，次要，内部和修订版本号
    /// </summary>
    public Version Version { get; set; } = Environment.Version;

    /// <summary>
    /// 获取运行应用程序的.NET安装的名称
    /// </summary>
    public string FrameworkDescription { get; set; } = RuntimeInformation.FrameworkDescription;

    /// <summary>
    /// 获取描述应用程序正在运行的操作系统的字符串
    /// </summary>
    public string OsDescription { get; set; } = RuntimeInformation.OSDescription;

    /// <summary>
    /// 代表操作系统平台
    /// </summary>
    public string OS { get; set; }

    /// <summary>
    /// 总物理内存 B
    /// </summary>
    public long TotalPhysicalMemory { get; set; }

    /// <summary>
    /// 可用物理内存 B
    /// </summary>
    public long FreePhysicalMemory { get; set; }

    /// <summary>
    /// 总交换空间（Linux）B
    /// </summary>
    public long SwapTotal { get; set; }

    /// <summary>
    /// 可用交换空间（Linux）B
    /// </summary>
    public long SwapFree { get; set; }

    /// <summary>
    /// 逻辑磁盘
    /// </summary>
    public List<Disk> LogicalDisk { get; set; }

    /// <summary>
    /// 本机IP
    /// </summary>
    public string LocalIp { get; set; }

    /// <summary>
    /// 构造
    /// </summary>
    public OsInfoTo()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            OS = OSPlatform.Windows.ToString();

            TotalPhysicalMemory = PlatformForWindows.TotalPhysicalMemory();
            FreePhysicalMemory = PlatformForWindows.FreePhysicalMemory();

            LogicalDisk = PlatformForWindows.LogicalDisk();

            ProcessorName = PlatformForWindows.ProcessorName();

            TickCount = PlatformForWindows.RunTime();

            PhysicalProcessorCount = PlatformForWindows.PhysicalProcessorCount();

            LogicProcessorCount = PlatformForWindows.LogicProcessorCount();

            NumberOfCores = PlatformForWindows.NumberOfCores();

            ProcessorUtilizationRate = PlatformForWindows.ProcessorUtilizationRate();

            LocalIp = PlatformForWindows.GetLocalIp();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            OS = OSPlatform.Linux.ToString();

            TotalPhysicalMemory = PlatformForLinux.MemInfo("MemTotal:");
            FreePhysicalMemory = PlatformForLinux.MemInfo("MemAvailable:");

            SwapFree = PlatformForLinux.MemInfo("SwapFree:");
            SwapTotal = PlatformForLinux.MemInfo("SwapTotal:");

            LogicalDisk = PlatformForLinux.LogicalDisk();

            ProcessorName = PlatformForLinux.CpuInfo("model name");

            TickCount = PlatformForLinux.RunTime();

            PhysicalProcessorCount = PlatformForLinux.PhysicalProcessorCount();

            LogicProcessorCount = PlatformForLinux.LogicProcessorCount();

            NumberOfCores = PlatformForLinux.NumberOfCores();

            ProcessorUtilizationRate = Convert.ToInt32(PlatformForLinux.CPULoad());

            LocalIp = PlatformForWindows.GetLocalIp();
        }
        //if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //{
        //    OS = OSPlatform.OSX.ToString();

        //    TotalPhysicalMemory = PlatformForLinux.MemInfo("MemTotal:");
        //    FreePhysicalMemory = PlatformForLinux.MemInfo("MemAvailable:");

        //    LogicalDisk = PlatformForLinux.LogicalDisk();

        //    ProcessorName = PlatformForLinux.CpuInfo("model name");

        //    TickCount = PlatformForLinux.RunTime();
        //}
    }

    /// <summary>
    /// WINDOWS
    /// </summary>
    public class PlatformForWindows
    {
        /// <summary>
        /// 获取物理内存 B
        /// </summary>
        /// <returns></returns>
        public static long TotalPhysicalMemory()
        {
            long totalPhysicalMemory = 0;

            var mc = new ManagementClass("Win32_ComputerSystem");
            var moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["TotalPhysicalMemory"] != null)
                {
                    totalPhysicalMemory = long.Parse(mo["TotalPhysicalMemory"].ToString());

                    break;
                }
            }

            return totalPhysicalMemory;
        }

        /// <summary>
        /// 获取可用内存 B
        /// </summary>
        public static long FreePhysicalMemory()
        {
            long freePhysicalMemory = 0;

            var mos = new ManagementClass("Win32_OperatingSystem");
            foreach (ManagementObject mo in mos.GetInstances())
            {
                if (mo["FreePhysicalMemory"] != null)
                {
                    freePhysicalMemory = 1024 * long.Parse(mo["FreePhysicalMemory"].ToString());

                    break;
                }
            }

            return freePhysicalMemory;
        }

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public static List<Disk> LogicalDisk()
        {
            var list = new List<Disk>();

            var diskClass = new ManagementClass("Win32_LogicalDisk");
            var disks = diskClass.GetInstances();
            foreach (var o in disks)
            {
                var disk = (ManagementObject)o;
                // DriveType.Fixed 为固定磁盘(硬盘) 
                if (int.Parse(disk["DriveType"].ToString()) == (int)DriveType.Fixed)
                {
                    list.Add(new Disk
                    {
                        Name = disk["Name"].ToString(),
                        Size = Convert.ToInt64(disk["Size"]),
                        FreeSpace = Convert.ToInt64(disk["FreeSpace"])
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 获取处理器名称
        /// </summary>
        /// <returns></returns>
        public static string ProcessorName()
        {
            var cmd = "wmic cpu get name";
            var cr = CmdHelper.Run(cmd).TrimEnd(Environment.NewLine.ToCharArray());
            var pvalue = cr.Split(Environment.NewLine.ToCharArray()).LastOrDefault();
            return pvalue;
        }

        /// <summary>
        /// 运行时长
        /// </summary>
        /// <returns></returns>
        public static long RunTime()
        {
            var cmd = "net statistics WORKSTATION";
            var cr = CmdHelper.Run(cmd).Split(Environment.NewLine.ToCharArray());
            DateTime.TryParse(cr[14].Split(' ').LastOrDefault(), out DateTime startTime);
            var pvalue = Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds);

            return pvalue;
        }

        /// <summary>
        /// 物理CPU数量
        /// </summary>
        /// <returns></returns>
        public static int PhysicalProcessorCount()
        {
            int count = 0;

            var mc = new ManagementClass("Win32_ComputerSystem");
            var moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["TotalPhysicalMemory"] != null)
                {
                    count = int.Parse(mo["NumberOfProcessors"].ToString());

                    break;
                }
            }

            return count;
        }


        /// <summary>
        /// 逻辑CPU数量
        /// </summary>
        /// <returns></returns>
        public static int LogicProcessorCount()
        {
            int count = 0;

            var mc = new ManagementClass("Win32_ComputerSystem");
            var moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["NumberOfLogicalProcessors"] != null)
                {
                    count = int.Parse(mo["NumberOfLogicalProcessors"].ToString());

                    break;
                }
            }

            return count;
        }

        /// <summary>
        /// 物理处理器核心数量
        /// </summary>
        /// <returns></returns>
        public static int NumberOfCores()
        {
            int count = 0;

            var mc = new ManagementClass("Win32_Processor");
            var moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["NumberOfCores"] != null)
                {
                    count = int.Parse(mo["NumberOfCores"].ToString());

                    break;
                }
            }

            return count;
        }

        public static int ProcessorUtilizationRate()
        {
            int rate = 0;

            var mc = new ManagementClass("Win32_PerfFormattedData_PerfOS_Processor");
            var moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                if (mo["Name"].ToString() == "_Total")
                {
                    if (mo["PercentProcessorTime"] != null)
                    {
                        rate = int.Parse(mo["PercentProcessorTime"].ToString());
                    }
                }
            }

            return rate;
        }

        public static string GetLocalIp()
        {
            string localIp = "";
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIp = endPoint.Address.ToString();
            return localIp;
        }
    }

    /// <summary>
    /// Linux系统
    /// </summary>
    public class PlatformForLinux
    {
        /// <summary>
        /// 获取 /proc/meminfo
        /// </summary>
        /// <param name="pkey"></param>
        /// <returns></returns>
        public static long MemInfo(string pkey)
        {
            var meminfo = FileHelper.ReadText("/proc/meminfo");
            var pitem = meminfo.Split(Environment.NewLine.ToCharArray()).FirstOrDefault(x => x.StartsWith(pkey));

            var pvalue = 1024 * long.Parse(pitem.Replace(pkey, "").ToLower().Replace("kb", "").Trim());

            return pvalue;
        }

        /// <summary>
        /// 获取 /proc/cpuinfo
        /// </summary>
        /// <param name="pkey"></param>
        /// <returns></returns>
        public static string CpuInfo(string pkey)
        {
            var meminfo = FileHelper.ReadText("/proc/cpuinfo");
            pkey = "cpu cores";
            //var pitem = meminfo.Split(Environment.NewLine.ToCharArray()).FirstOrDefault(x => x.StartsWith(pkey));
            var pitem = meminfo.Split(Environment.NewLine.ToCharArray()).Where(x => x.StartsWith(pkey)).ToList();
            //var pvalue = pitem.Split(':')[1].Trim();
            return "";
            // return pvalue;
        }

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public static List<Disk> LogicalDisk()
        {
            var list = new List<Disk>();

            var dfresult = CmdHelper.Shell("df");
            var listdev = dfresult.Output.Split(Environment.NewLine.ToCharArray())
                .Where(x => x.StartsWith("/dev/"));
            foreach (var devitem in listdev)
            {
                var dis = devitem.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                list.Add(new Disk
                {
                    Name = dis[0],
                    Size = long.Parse(dis[1]) * 1024,
                    FreeSpace = long.Parse(dis[3]) * 1024
                });
            }

            return list;
        }

        /// <summary>
        /// 获取CPU使用率 %
        /// </summary>
        /// <returns></returns>
        public static float CPULoad()
        {
            var br = CmdHelper.Shell("vmstat 1 2");
            var cpuitems = br.Output.Split(Environment.NewLine.ToCharArray()).LastOrDefault().Split(' ')
                .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var us = cpuitems[cpuitems.Count - 5];
            var rate = float.Parse(us);
            return float.Parse(us);
        }

        /// <summary>
        /// 运行时长
        /// </summary>
        /// <returns></returns>
        public static long RunTime()
        {
            var uptime = FileHelper.ReadText("/proc/uptime");
            var pitem = Convert.ToDouble(uptime.Split(' ')[0]);

            var pvalue = Convert.ToInt64(pitem * 1000);
            return pvalue;
        }

        /// <summary>
        /// 物理CPU数量
        /// </summary>
        /// <returns></returns>
        public static int PhysicalProcessorCount()
        {
            HashSet<string> Maps = new HashSet<string>();
            var meminfo = FileHelper.ReadText("/proc/cpuinfo");
            string pkey = "physical id";
            var pitem = meminfo.Split(Environment.NewLine.ToCharArray()).Where(x => x.StartsWith(pkey)).ToList();
            foreach (var item in pitem)
            {
                Maps.Add(item);
            }

            return Maps.Count;
        }


        /// <summary>
        /// 逻辑CPU数量
        /// </summary>
        /// <returns></returns>
        public static int LogicProcessorCount()
        {
            HashSet<string> Maps = new HashSet<string>();
            var meminfo = FileHelper.ReadText("/proc/cpuinfo");
            string pkey = "processor";
            var pitem = meminfo.Split(Environment.NewLine.ToCharArray()).Where(x => x.StartsWith(pkey)).ToList();
            foreach (var item in pitem)
            {
                Maps.Add(item);
            }

            return Maps.Count;
        }

        /// <summary>
        /// 物理处理器核心数量
        /// </summary>
        /// <returns></returns>
        public static int NumberOfCores()
        {
            HashSet<string> Maps = new HashSet<string>();
            var meminfo = FileHelper.ReadText("/proc/cpuinfo");
            string pkey = "core id";
            var pitem = meminfo.Split(Environment.NewLine.ToCharArray()).Where(x => x.StartsWith(pkey)).ToList();
            foreach (var item in pitem)
            {
                Maps.Add(item);
            }

            return Maps.Count;
        }
    }
}

public class Disk
{
    public string Name { get; set; }

    public long Size { get; set; }

    public long FreeSpace { get; set; }
}
