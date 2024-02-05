using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 用户岗位Dto
/// </summary>
[AutoMapping(typeof(UserJobs), typeof(CreateUpdateUserJobsDto))]
public class CreateUpdateUserJobsDto : BaseEntityDto<long>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="jobId"></param>
    public CreateUpdateUserJobsDto(long userId, long jobId)
    {
        UserId = userId;
        JobId = jobId;
    }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 岗位ID
    /// </summary>
    public long JobId { get; set; }
}
