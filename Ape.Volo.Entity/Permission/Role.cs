using System.Collections.Generic;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.Permission
{
    /// <summary>
    /// 角色
    /// </summary>
    [SugarTable("sys_role")]
    public class Role : BaseEntity, ISoftDeletedEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 角色等级
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int Level { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 数据权限
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false)]
        public string DataScope { get; set; }

        /// <summary>
        /// 角色代码
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = false)]
        public string Permission { get; set; }

        /// <summary>
        /// 菜单集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(RoleMenu), nameof(RoleMenu.RoleId), nameof(RoleMenu.MenuId))]
        public List<Menu> MenuList { get; set; }

        /// <summary>
        /// 部门集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(RoleDepartment), nameof(RoleDepartment.RoleId), nameof(RoleDepartment.DeptId))]
        public List<Department> DepartmentList { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 用户列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(UserRole), nameof(UserRole.RoleId), nameof(UserRole.UserId))]
        public List<User> Users { get; set; }


        /// <summary>
        /// 菜单集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(RoleApis), nameof(RoleApis.RoleId), nameof(RoleApis.ApisId))]
        public List<Apis> Apis { get; set; }
    }
}
