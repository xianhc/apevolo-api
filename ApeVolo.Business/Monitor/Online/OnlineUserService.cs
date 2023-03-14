using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.DI;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
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

    public async Task<List<ExportRowModel>> DownloadAsync()
    {
        List<OnlineUser> onlineUsers = new List<OnlineUser>();
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
                    onlineUsers.Add(onlineUser);
                }
            }
        }

        var exportRowModels = new List<ExportRowModel>();
        List<ExportColumnModel> exportColumnModels;
        int point;
        onlineUsers.ForEach(onlineUser =>
        {
            point = 0;
            exportColumnModels = new List<ExportColumnModel>();
            exportColumnModels.Add(new ExportColumnModel
                { Key = "登录账号", Value = onlineUser.UserName, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "用户昵称", Value = onlineUser.NickName, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel { Key = "所属部门", Value = onlineUser.Dept, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel { Key = "登录IP", Value = onlineUser.Ip, Point = point++ });
            exportColumnModels.Add(
                new ExportColumnModel { Key = "IP地址", Value = onlineUser.Address, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "操作系统", Value = onlineUser.OperatingSystem, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "设备类型", Value = onlineUser.DeviceType, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "浏览器", Value = onlineUser.BrowserName, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "版本号", Value = onlineUser.Version, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
            {
                Key = "登录时间", Value = onlineUser.LoginTime.ToString(CultureInfo.InvariantCulture), Point = point++
            });
            exportColumnModels.Add(new ExportColumnModel { Key = "在线Key", Value = onlineUser.Key, Point = point++ });
            exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
        });
        return exportRowModels;
    }
}