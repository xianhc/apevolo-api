using System;
using System.Threading.Tasks;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Helper;
using Ape.Volo.IBusiness.Interface.Monitor;
using Ape.Volo.IBusiness.Vo.ServerResources;
using Disk = Ape.Volo.IBusiness.Vo.ServerResources.Disk;

namespace Ape.Volo.Business.Monitor;

public class ServerResourcesService : IServerResourcesService
{
    private const int B = 1;
    private const int Kb = 1024 * B;
    private const int Mb = 1024 * Kb;
    private const int Gb = 1024 * Mb;

    public async Task<ServerResourcesInfo> Query()
    {
        var os = new OsInfoTo();

        ServerResourcesInfo osInfo = new ServerResourcesInfo
        {
            Time = DateTime.Now.ToString("HH:mm:ss"),
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
                Total = Math.Round(Convert.ToDecimal(os.TotalPhysicalMemory) / Gb, 2)
                    .ToString("0.00") + " GiB",
                Used = Math.Round(Convert.ToDecimal(os.TotalPhysicalMemory - os.FreePhysicalMemory) / 1073741824, 2)
                    .ToString("0.00") + " GiB",
                Available = Math.Round(Convert.ToDecimal(os.FreePhysicalMemory) / Gb, 1)
                    .ToString("0.00") + " GiB",
                UsageRate = Math.Round(100 / Math.Round(Convert.ToDecimal(os.TotalPhysicalMemory) / 1073741824, 2) *
                                       Math.Round(
                                           Convert.ToDecimal(os.TotalPhysicalMemory - os.FreePhysicalMemory) /
                                           Gb, 2), 2).ToString("0.00")
            },
            Swap = new Swap(),
            Disk = new Disk()
        };
        if (os.LogicalDisk is { Count: > 0 })
        {
            long size = 0;
            long freeSpace = 0;
            foreach (var item in os.LogicalDisk)
            {
                size += item.Size;
                freeSpace += item.FreeSpace;
            }

            osInfo.Disk.Total = Math.Round(Convert.ToDecimal(size) / Gb, 2).ToString("0.00") +
                                " GiB";
            osInfo.Disk.Used =
                Math.Round(Convert.ToDecimal(size - freeSpace) / Gb, 2).ToString("0.00") + " GiB";
            osInfo.Disk.Available =
                Math.Round(Convert.ToDecimal(freeSpace) / Gb, 2).ToString("0.00") + " GiB";
            osInfo.Disk.UsageRate = Math.Round(100 / Math.Round(Convert.ToDecimal(size) / Gb, 2) *
                                               Math.Round(Convert.ToDecimal(size - freeSpace) / Gb,
                                                   2), 2)
                .ToString("0.00");
        }

        if (os.SwapTotal > 0)
        {
            osInfo.Swap.Total =
                Math.Round(Convert.ToDecimal(os.SwapTotal / Gb), 2).ToString("0.00") + " GiB";
            osInfo.Swap.Used = Math.Round(Convert.ToDecimal((os.SwapTotal - os.SwapFree) / Gb), 2)
                .ToString("0.00") + " GiB";
            osInfo.Swap.Available =
                Math.Round(Convert.ToDecimal(os.SwapFree / Gb), 2).ToString("0.00") + " GiB";
            osInfo.Swap.UsageRate =
                Math.Round(Convert.ToDecimal(100 / os.SwapTotal * (os.SwapTotal - os.SwapFree)), 2)
                    .ToString("0.00");
        }

        await Task.CompletedTask;
        return osInfo;
    }
}
