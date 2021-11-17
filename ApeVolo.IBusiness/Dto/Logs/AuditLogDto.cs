using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Logs;

namespace ApeVolo.IBusiness.Dto.Logs
{
    [AutoMapping(typeof(AuditLog), typeof(AuditLogDto))]
    public class AuditLogDto : BaseEntityDto
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

        public string BrowserInfo { get; set; }

        public string RequestIp { get; set; }

        public string IpAddress { get; set; }
    }
}