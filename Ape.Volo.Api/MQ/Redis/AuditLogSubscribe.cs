using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Common.Attributes.Redis;
using Ape.Volo.Common.Caches.Redis.MessageQueue;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Entity.Monitor;
using Ape.Volo.IBusiness.Interface.Monitor;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Ape.Volo.Api.MQ.Redis;

public class AuditLogSubscribe : IRedisSubscribe
{
    #region Fields

    private readonly ILogger<AuditLogSubscribe> _logger;
    private readonly IAuditLogService _auditInfoService;

    #endregion

    #region Ctor

    public AuditLogSubscribe(IAuditLogService auditLogService, ILogger<AuditLogSubscribe> logger)
    {
        _auditInfoService = auditLogService;
        _logger = logger;
    }

    #endregion

    [SubscribeDelay(MqTopicNameKey.AuditLogQueue, true)]
    private async Task DoSub(List<RedisValue> redisValues)
    {
        try
        {
            if (redisValues.Any())
            {
                List<AuditLog> auditLogs = new List<AuditLog>();
                redisValues.ForEach(x => { auditLogs.Add(x.ToString().ToObject<AuditLog>()); });
                await _auditInfoService.CreateListAsync(auditLogs);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(ExceptionHelper.GetExceptionAllMsg(e));
        }
    }
}
