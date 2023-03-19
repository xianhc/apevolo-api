using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Permission.User;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.User;

[AutoMapping(typeof(UserJobs), typeof(CreateUpdateUserJobsDto))]
public class CreateUpdateUserJobsDto : EntityDtoRoot<long>
{
    public CreateUpdateUserJobsDto(long userId, long jobId)
    {
        UserId = userId;
        JobId = jobId;
    }

    public long UserId { get; set; }

    public long JobId { get; set; }
}