using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Logs;

namespace ApeVolo.IBusiness.Dto.Logs;

[AutoMapping(typeof(Log), typeof(LogDto))]
public class LogDto : BaseEntityDto
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

    public string BrowserInfo { get; set; }

    public string RequestIp { get; set; }

    public string IpAddress { get; set; }
}