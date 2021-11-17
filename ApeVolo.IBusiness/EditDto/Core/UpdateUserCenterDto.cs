namespace ApeVolo.IBusiness.EditDto.Core
{
    public class UpdateUserCenterDto
    {
        [Common.AttributeExt.ApeVoloRequiredAttribute(Message = "Id cannot be empty")]
        public string Id { get; set; }
        public string NickName { get;set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
    }
}