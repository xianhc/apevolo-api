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

    Task<bool> CreateAsync(CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto);

    Task<bool> UpdateTriesAsync(QueuedEmailDto queuedEmailDto);
    Task<bool> UpdateAsync(CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto);

    Task<bool> DeleteAsync(HashSet<long> ids);

    Task<List<QueuedEmailDto>> QueryAsync(QueuedEmailQueryCriteria queuedEmailQueryCriteria, Pagination pagination);

    #endregion

    #region 扩展接口

    Task<bool> ResetEmail(string emailAddres, string messageTemplateName);

    #endregion
}
