using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;

namespace Ape.Volo.IBusiness.Interface.Monitor;

/// <summary>
/// 在线用户接口
/// </summary>
public interface IOnlineUserService
{
    #region 基础接口

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<LoginUserInfo>> QueryAsync(Pagination pagination);

    /// <summary>
    /// 强退
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task DropOutAsync(HashSet<string> ids);

    /// <summary>
    /// 下载
    /// </summary>
    /// <returns></returns>
    Task<List<ExportBase>> DownloadAsync();

    #endregion
}
