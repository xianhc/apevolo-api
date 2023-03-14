using System;
using System.Threading.Tasks;
using ApeVolo.Common.DI;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.IBusiness.Interface.Monitor.Server;
using ApeVolo.IBusiness.Vo.ServerResources;
using Disk = ApeVolo.IBusiness.Vo.ServerResources.Disk;

namespace ApeVolo.Business.Monitor.Server;

public class ServerResourcesService : IServerResourcesService, IDependencyService
{
    public async Task<ServerResourcesInfo> Query()
    {
        var os = new OsInfoTo();

        ServerResourcesInfo osInfo = new ServerResourcesInfo
        {
            Time = DateTime.Now.ToString("HH:mm;ss"),
            Sys = new Sys
            {
                Os = os.OsDescription,
                Day = DateTimeHelper.FormatLongToTime(os.TickCount),
                Ip = os.LocalIp,
            },
            Cpu = new Cpu
            {
                Name = os.ProcessorName,
                Package = os.PhysicalProcessorCount + "个物理CPU",
                Core = os.NumberOfCores + "个物理核心",
                CoreNumber = os.NumberOfCores,
                Logic = os.LogicProcessorCount + "个逻辑CPU",
                Used = os.ProcessorUtilizationRate.ToString(),
                Idle = (100 - os.ProcessorUtilizationRate).ToString()
            },
            Memory = new Memory
            {
                Total = Math.Round(Convert.ToDecimal(os.TotalPhysicalMemory) / 1024 / 1024 / 1024, 2)
                    .ToString("0.00") + " GiB",
                Used = Math.Round(Convert.ToDecimal(os.TotalPhysicalMemory - os.FreePhysicalMemory) / 1073741824, 2)
                    .ToString("0.00") + " GiB",
                Available = Math.Round(Convert.ToDecimal(os.FreePhysicalMemory) / 1024 / 1024 / 1024, 1)
                    .ToString("0.00") + " GiB",
                UsageRate = Math.Round(100 / Math.Round(Convert.ToDecimal(os.TotalPhysicalMemory) / 1073741824, 2) *
                                       Math.Round(
                                           Convert.ToDecimal(os.TotalPhysicalMemory - os.FreePhysicalMemory) /
                                           1024 / 1024 / 1024, 2), 2).ToString("0.00") + " GiB"
            },
            Swap = new Swap(),
            Disk = new Disk()
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

            osInfo.Disk.Total = Math.Round(Convert.ToDecimal(size) / 1024 / 1024 / 1024, 2).ToString("0.00") +
                                " GiB";
            osInfo.Disk.Used =
                Math.Round(Convert.ToDecimal(size - freeSpace) / 1024 / 1024 / 1024, 2).ToString("0.00") + " GiB";
            osInfo.Disk.Available =
                Math.Round(Convert.ToDecimal(freeSpace) / 1024 / 1024 / 1024, 2).ToString("0.00") + " GiB";
            osInfo.Disk.UsageRate = Math.Round(100 / Math.Round(Convert.ToDecimal(size) / 1024 / 1024 / 1024, 2) *
                                               Math.Round(Convert.ToDecimal(size - freeSpace) / 1024 / 1024 / 1024,
                                                   2), 2)
                .ToString("0.00") + " GiB";
        }

        if (os.SwapTotal > 0)
        {
            osInfo.Swap.Total =
                Math.Round(Convert.ToDecimal(os.SwapTotal / 1024 / 1024 / 1024), 2).ToString("0.00") + " GiB";
            osInfo.Swap.Used = Math.Round(Convert.ToDecimal((os.SwapTotal - os.SwapFree) / 1024 / 1024 / 1024), 2)
                .ToString("0.00") + " GiB";
            osInfo.Swap.Available =
                Math.Round(Convert.ToDecimal(os.SwapFree / 1024 / 1024 / 1024), 2).ToString("0.00") + " GiB";
            osInfo.Swap.UsageRate =
                Math.Round(Convert.ToDecimal(100 / os.SwapTotal * (os.SwapTotal - os.SwapFree)), 2)
                    .ToString("0.00") + " GiB";
        }

        await Task.CompletedTask;
        return osInfo;
    }
}