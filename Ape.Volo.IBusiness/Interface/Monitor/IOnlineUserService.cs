using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;

namespace Ape.Volo.IBusiness.Interface.Monitor;

public interface IOnlineUserService
{
    #region 基础接口

    Task<List<LoginUserInfo>> QueryAsync(Pagination pagination);

    Task DropOutAsync(HashSet<string> ids);

    Task<List<ExportBase>> DownloadAsync();

    #endregion
}
