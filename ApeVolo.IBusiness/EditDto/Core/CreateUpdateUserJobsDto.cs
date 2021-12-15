using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core
{
    [AutoMapping(typeof(UserJobs), typeof(CreateUpdateUserJobsDto))]
    public class CreateUpdateUserJobsDto
    {
        public CreateUpdateUserJobsDto(long userId, long jobId)
        {
            UserId = userId;
            JobId = jobId;
        }
        public long UserId { get; set; }

        public long JobId { get; set; }
    }
}
