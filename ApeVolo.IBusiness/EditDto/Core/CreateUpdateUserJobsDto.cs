using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core
{
    [AutoMapping(typeof(UserJobs), typeof(CreateUpdateUserJobsDto))]
    public class CreateUpdateUserJobsDto
    {
        public CreateUpdateUserJobsDto(string userId, string jobId)
        {
            UserId = userId;
            JobId = jobId;
        }
        public string UserId { get; set; }

        public string JobId { get; set; }
    }
}
