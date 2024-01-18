using Ape.Volo.Common.AttributeExt;
using Ape.Volo.IBusiness.Base;
using ApeVolo.Entity.Permission;

namespace Ape.Volo.IBusiness.Dto.Permission;

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
