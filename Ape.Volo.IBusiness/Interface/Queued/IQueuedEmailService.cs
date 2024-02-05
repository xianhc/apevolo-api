using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Queued;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Queued;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.Queued;

/// <summary>
/// 邮件队列接口
/// </summary>
public interface IQueuedEmailService : IBaseServices<QueuedEmail>
{
    #region 基础接口

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="createUpdateQueuedEmailDto"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto);

    /// <summary>
    /// 更新发送次数
    /// </summary>
    /// <param name="queuedEmailDto"></param>
    /// <returns></returns>
    Task<bool> UpdateTriesAsync(QueuedEmailDto queuedEmailDto);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateQueuedEmailDto"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(HashSet<long> ids);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="queuedEmailQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<QueuedEmailDto>> QueryAsync(QueuedEmailQueryCriteria queuedEmailQueryCriteria, Pagination pagination);

    #endregion

    #region 扩展接口

    /// <summary>
    /// 重置邮箱
    /// </summary>
    /// <param name="emailAddres"></param>
    /// <param name="messageTemplateName"></param>
    /// <returns></returns>
    Task<bool> ResetEmail(string emailAddres, string messageTemplateName);

    #endregion
}
