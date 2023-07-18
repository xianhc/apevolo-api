using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Monitor;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Monitor.Logs.Exception;

[AutoMapping(typeof(ExceptionLog), typeof(ExceptionLogDto))]
public class ExceptionLogDto : BaseEntityDto<long>
{
    public string Area { get; set; }

    public string Controller { get; set; }

    public string Action { get; set; }

    public string Method { get; set; }

    public string Description { get; set; }

    public string RequestUrl { get; set; }

    public string RequestParameters { get; set; }

    public string ExceptionMessage { get; set; }

    public string ExceptionMessageFull { get; set; }

    public string ExceptionStack { get; set; }

    public int LogLevel { get; set; }

    public string RequestIp { get; set; }

    public string IpAddress { get; set; }

    public string OperatingSystem { get; set; }

    public string DeviceType { get; set; }

    public string BrowserName { get; set; }

    public string Version { get; set; }
}
