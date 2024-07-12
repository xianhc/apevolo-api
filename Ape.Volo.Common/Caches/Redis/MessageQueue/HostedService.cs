using System;
using System.Threading;
using System.Threading.Tasks;
using Ape.Volo.Common.Caches.Redis.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ape.Volo.Common.Caches.Redis.MessageQueue;

public class HostedService : IHostedService, IDisposable
{
    private readonly ILogger<HostedService> _logger;
    private readonly IOptions<RedisQueueOptions> _options;

    public HostedService(ILogger<HostedService> logger, IOptions<RedisQueueOptions> options)
    {
        _options = options;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("redis消息队列启动");
        var init = new InitCore();
        Task.Run(async () => { await init.FindInterfaceTypes(_options.Value); }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("redis消息队列结束");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    }
}
