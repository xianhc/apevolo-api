using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Monitor.Logs.Auditing;

[AutoMapping(typeof(AuditLog), typeof(AuditLogDto))]
public class AuditLogDto : EntityDtoRoot<long>
{
    public string Area { get; set; }

    public string Controller { get; set; }

    public string Action { get; set; }

    public string Method { get; set; }

    public string Description { get; set; }

    public string RequestUrl { get; set; }

    public string RequestParameters { get; set; }

    public string ResponseData { get; set; }

    public int ExecutionDuration { get; set; }

    public string RequestIp { get; set; }

    public string IpAddress { get; set; }

    public string OperatingSystem { get; set; }

    public string DeviceType { get; set; }

    public string BrowserName { get; set; }

    public string Version { get; set; }
}