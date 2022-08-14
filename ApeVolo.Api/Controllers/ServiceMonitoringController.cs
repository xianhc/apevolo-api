using System;
using System.ComponentModel;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model.ServerMonitor;
using Microsoft.AspNetCore.Mvc;
using Disk = ApeVolo.Common.Model.ServerMonitor.Disk;

namespace ApeVolo.Api.Controllers;

/// <summary>
/// 服务器监控
/// </summary>
[Area("ServiceMonitoring")]
[Route("/api/")]
public class ServiceMonitoringController : BaseApiController
{
    #region 对内接口

    [HttpGet]
    [Route("service/monitor/info")]
    [Description("{0}Info")]
    [ApeVoloAuthorize(new[] { "admin" })]
    public ActionResult<object> QueryServiceInfo()
    {
        OsInfoTo os = new OsInfoTo();

        ResultsVm osInfo = new ResultsVm
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

        if (os.SwapTotal <= 0) return osInfo.ToJson();
        osInfo.Swap.Total =
            Math.Round(Convert.ToDecimal(os.SwapTotal / 1024 / 1024 / 1024), 2).ToString("0.00") + " GiB";
        osInfo.Swap.Used = Math.Round(Convert.ToDecimal((os.SwapTotal - os.SwapFree) / 1024 / 1024 / 1024), 2)
            .ToString("0.00") + " GiB";
        osInfo.Swap.Available =
            Math.Round(Convert.ToDecimal(os.SwapFree / 1024 / 1024 / 1024), 2).ToString("0.00") + " GiB";
        osInfo.Swap.UsageRate =
            Math.Round(Convert.ToDecimal(100 / os.SwapTotal * (os.SwapTotal - os.SwapFree)), 2)
                .ToString("0.00") + " GiB";

        return osInfo.ToJson();
    }

    #endregion
}