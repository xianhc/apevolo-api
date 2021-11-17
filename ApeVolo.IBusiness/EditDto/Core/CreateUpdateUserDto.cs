using ApeVolo.Common.AttributeExt;
using System.Collections.Generic;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core
{
    [AutoMapping(typeof(User), typeof(CreateUpdateUserDto))]
    public class CreateUpdateUserDto : BaseCreateUpdateEntityDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [ApeVoloRequired(Message = "用户名不能为空！")]
        public string Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [ApeVoloRequired(Message = "昵称不能为空！")]
        public string NickName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [ApeVoloRequired(Message = "邮箱不能为空！")]
        public string Email { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// 电话
        /// </summary>
        [ApeVoloRequired(Message = "电话不能为空！")]
        public string Phone { get; set; }

        [ApeVoloRequired(Message = "性别不能为空！")] public string Gender { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [ApeVoloRequired(Message = "请选择一个部门！")]
        public CreateUpdateDepartmentDto Dept { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [ApeVoloRequired(Message = "角色至少选择一个！")]
        public List<CreateUpdateRoleDto> Roles { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        [ApeVoloRequired(Message = "岗位至少选择一个！")]
        public List<CreateUpdateJobDto> Jobs { get; set; }
    }
}