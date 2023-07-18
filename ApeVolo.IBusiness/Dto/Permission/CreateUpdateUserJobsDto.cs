using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(UserJobs), typeof(CreateUpdateUserJobsDto))]
public class CreateUpdateUserJobsDto : BaseEntityDto<long>
{
    public CreateUpdateUserJobsDto(long userId, long jobId)
    {
        UserId = userId;
        JobId = jobId;
    }

    public long UserId { get; set; }

    public long JobId { get; set; }
}
