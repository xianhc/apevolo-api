using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.DI;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.ExportModel.Monitor;
using ApeVolo.IBusiness.Interface.Monitor.Online;

namespace ApeVolo.Business.Monitor.Online;

public class OnlineUserService : IOnlineUserService, IDependencyService
{
    private readonly IRedisCacheService _redisCacheService;

    public OnlineUserService(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public async Task<List<OnlineUser>> QueryAsync(Pagination pagination)
    {
        List<OnlineUser> onlineUsers = new List<OnlineUser>();
        var arrayList = await _redisCacheService.ScriptEvaluateKeys(RedisKey.OnlineKey);
        if (arrayList.Length > 0)
        {
            foreach (var item in arrayList)
            {
                var onlineUser =
                    await _redisCacheService.GetCacheAsync<OnlineUser>(item);
                if (onlineUser.IsNull()) continue;
                onlineUser.CurrentPermission = null;
                onlineUsers.Add(onlineUser);
            }
        }

        List<OnlineUser> newOnlineUsers = new List<OnlineUser>();
        if (onlineUsers.Count > 0)
        {
            newOnlineUsers = onlineUsers.Skip((pagination.PageIndex - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();
        }

        return newOnlineUsers;
    }

    public async Task DropOutAsync(HashSet<string> ids)
    {
        foreach (var item in ids)
        {
            await _redisCacheService.RemoveAsync(RedisKey.OnlineKey + item);
        }
    }

    public async Task<List<ExportBase>> DownloadAsync()
    {
        List<ExportBase> onlineUserExports = new List<ExportBase>();
        var arrayList = await _redisCacheService.ScriptEvaluateKeys(RedisKey.OnlineKey);
        if (arrayList.Length > 0)
        {
            foreach (var item in arrayList)
            {
                OnlineUser onlineUser =
                    await _redisCacheService.GetCacheAsync<OnlineUser>(item);
                if (onlineUser != null)
                {
                    onlineUser.CurrentPermission = null;
                    onlineUserExports.Add(onlineUser.ChangeType<OnlineUserExport>());
                }
            }
        }

        return onlineUserExports;
    }
}