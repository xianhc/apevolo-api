using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Common.Caches;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.ExportModel.Monitor;
using Ape.Volo.IBusiness.Interface.Monitor;
using Ape.Volo.IBusiness.Interface.System;

namespace Ape.Volo.Business.Monitor;

public class OnlineUserService : IOnlineUserService
{
    private readonly ICache _cache;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    public OnlineUserService(ICache cache, ITokenBlacklistService tokenBlacklistService)
    {
        _cache = cache;
        _tokenBlacklistService = tokenBlacklistService;
    }

    public async Task<List<LoginUserInfo>> QueryAsync(Pagination pagination)
    {
        List<LoginUserInfo> loginUserInfos = new List<LoginUserInfo>();
        var arrayList = await _cache.ScriptEvaluateKeys(GlobalConstants.CacheKey.OnlineKey);
        if (arrayList.Length > 0)
        {
            foreach (var item in arrayList)
            {
                var loginUserInfo =
                    await _cache.GetAsync<LoginUserInfo>(item);
                if (loginUserInfo.IsNull()) continue;
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
        var list = new List<TokenBlacklist>();
        list.AddRange(ids.Select(x => new TokenBlacklist() { AccessToken = x }));
        if (await _tokenBlacklistService.AddEntityListAsync(list))
        {
            foreach (var item in ids)
            {
                await _cache.RemoveAsync(GlobalConstants.CacheKey.OnlineKey + item);
            }
        }
    }

    public async Task<List<ExportBase>> DownloadAsync()
    {
        List<ExportBase> onlineUserExports = new List<ExportBase>();
        var arrayList = await _cache.ScriptEvaluateKeys(GlobalConstants.CacheKey.OnlineKey);
        if (arrayList.Length > 0)
        {
            foreach (var item in arrayList)
            {
                LoginUserInfo loginUserInfo =
                    await _cache.GetAsync<LoginUserInfo>(item);
                if (loginUserInfo != null)
                {
                    onlineUserExports.Add(loginUserInfo.ChangeType<OnlineUserExport>());
                }
            }
        }

        return onlineUserExports;
    }
}
