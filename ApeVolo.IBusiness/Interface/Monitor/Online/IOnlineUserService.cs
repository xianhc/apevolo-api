using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;

namespace ApeVolo.IBusiness.Interface.Monitor.Online;

public interface IOnlineUserService
{
    #region 基础接口

    Task<List<OnlineUser>> QueryAsync(Pagination pagination);

    Task DropOutAsync(HashSet<string> ids);

    Task<List<ExportRowModel>> DownloadAsync();

    #endregion
}