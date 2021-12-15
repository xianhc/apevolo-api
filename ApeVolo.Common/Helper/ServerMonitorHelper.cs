using System;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model.ServerMonitor;

namespace ApeVolo.Common.Helper
{
    public static class ServerMonitorHelper
    {
        public static ResultsVm GetServerMonitorInfo()
        {
            OsInfoTo os = new OsInfoTo();

            ResultsVm vm = new ResultsVm
            {

                Time = DateTime.Now.ToString("HH:mm;ss"),
                Sys = new Sys
                {
                    Os = os.OsDescription,
                    Day = FormatLongToTimeStr(os.TickCount),
                    Ip = "127.0.0.1",
                },
                Cpu = new Cpu
                {
                    Name  = os.ProcessorName,
                    Package = os.PhysicalProcessorCount.ToString(),
                    Core = os.NumberOfCores + "个物理核心",
                    CoreNumber = os.NumberOfCores,
                    Logic = os.LogicProcessorCount.ToString(),
                    Used = os.ProcessorUtilizationRate.ToString(),
                    Idle = (100 - os.ProcessorUtilizationRate).ToString()
                },
                Memory = new Memory
                {
                    Total = Math.Round(Convert.ToDecimal(os.TotalPhysicalMemory) / 1073741824, 1).ToString("0.0"),
                    Used = Math.Round(Convert.ToDecimal(os.TotalPhysicalMemory - os.FreePhysicalMemory) / 1073741824, 1)
                        .ToString("0.0"),
                    Available = Math.Round(Convert.ToDecimal(os.FreePhysicalMemory) / 1073741824, 1).ToString("0.0"),
                    UsageRate = Math.Round(100 / Math.Round(Convert.ToDecimal(os.TotalPhysicalMemory) / 1073741824, 1) *
                                           Math.Round(
                                               Convert.ToDecimal(os.TotalPhysicalMemory - os.FreePhysicalMemory) /
                                               1073741824, 1), 2).ToString("0.00")
                },
                Swap = new Swap()
            };
            if (os.LogicalDisk != null && os.LogicalDisk.Count > 0)
            {
                long size = 0;
                long freeSpace = 0;
                foreach (var item in os.LogicalDisk)
                {
                    size += item.Size;
                    freeSpace += item.FreeSpace;
                }
                vm.Disk.Total = Math.Round(Convert.ToDecimal(size) / 1073741824, 2).ToString("0.00");
                vm.Disk.Used = Math.Round(Convert.ToDecimal(size - freeSpace) / 1073741824, 2).ToString("0.00");
                vm.Disk.Available = Math.Round(Convert.ToDecimal(freeSpace) / 1073741824, 2).ToString("0.00");
                vm.Disk.UsageRate = Math.Round(100 / Math.Round(Convert.ToDecimal(size) / 1073741824, 2) *
                                               Math.Round(Convert.ToDecimal(size - freeSpace) / 1073741824, 2), 2)
                    .ToString("0.00");
            }
            return vm;
        }
        
        private static string FormatLongToTimeStr(long time)
        {
            int hour = 0;
            int minute = 0;
            int second = 0;
            second = (int)(time / 1000);

            if (second > 60)
            {
                minute = second / 60;
                second = second % 60;
            }
            if (minute > 60)
            {
                hour = minute / 60;
                minute = minute % 60;
            }
            return hour + "小时" + minute + "分钟" + second + "秒";
        }
    }
}