using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;

namespace ApeVolo.IBusiness.Interface.Monitor;

public interface IOnlineUserService
{
    #region 基础接口

    Task<List<LoginUserInfo>> QueryAsync(Pagination pagination);

    Task DropOutAsync(HashSet<string> ids);

    Task<List<ExportBase>> DownloadAsync();

    #endregion
}
