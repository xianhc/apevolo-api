using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.ExportModel.Monitor;
using ApeVolo.IBusiness.Interface.Monitor;

namespace ApeVolo.Business.Monitor;

public class OnlineUserService : IOnlineUserService
{
    private readonly IRedisCacheService _redisCacheService;

    public OnlineUserService(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public async Task<List<LoginUserInfo>> QueryAsync(Pagination pagination)
    {
        List<LoginUserInfo> loginUserInfos = new List<LoginUserInfo>();
        var arrayList = await _redisCacheService.ScriptEvaluateKeys(GlobalConstants.CacheKey.OnlineKey);
        if (arrayList.Length > 0)
        {
            foreach (var item in arrayList)
            {
                var loginUserInfo =
                    await _redisCacheService.GetAsync<LoginUserInfo>(item);
                if (loginUserInfo.IsNull()) continue;
                loginUserInfo.CurrentPermission = null;
                loginUserInfo.AccessToken = loginUserInfo.AccessToken.ToMd5String16();
                loginUserInfos.Add(loginUserInfo);
            }
        }

        List<LoginUserInfo> newOnlineUsers = new List<LoginUserInfo>();
        if (loginUserInfos.Count > 0)
        {
            newOnlineUsers = loginUserInfos.Skip((pagination.PageIndex - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();
        }

        return newOnlineUsers;
    }

    public async Task DropOutAsync(HashSet<string> ids)
    {
        foreach (var item in ids)
        {
            await _redisCacheService.RemoveAsync(GlobalConstants.CacheKey.OnlineKey + item);
        }
    }

    public async Task<List<ExportBase>> DownloadAsync()
    {
        List<ExportBase> onlineUserExports = new List<ExportBase>();
        var arrayList = await _redisCacheService.ScriptEvaluateKeys(GlobalConstants.CacheKey.OnlineKey);
        if (arrayList.Length > 0)
        {
            foreach (var item in arrayList)
            {
                LoginUserInfo loginUserInfo =
                    await _redisCacheService.GetAsync<LoginUserInfo>(item);
                if (loginUserInfo != null)
                {
                    loginUserInfo.CurrentPermission = null;
                    onlineUserExports.Add(loginUserInfo.ChangeType<OnlineUserExport>());
                }
            }
        }

        return onlineUserExports;
    }
}
