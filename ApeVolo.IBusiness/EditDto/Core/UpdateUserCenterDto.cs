using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.EditDto.Core
{
    public class UpdateUserCenterDto
    {
        [ApeVoloRequired(Message = "Id cannot be empty")]
        public long Id { get; set; }
        public string NickName { get;set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
    }
}