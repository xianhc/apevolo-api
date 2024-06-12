using System.Collections.Generic;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.Permission
{
    /// <summary>
    /// 部门
    /// </summary>
    [SugarTable("sys_department")]
    public class Department : BaseEntity, ISoftDeletedEntity
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 父级部门ID
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// 子节点个数
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int SubCount { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 用户列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(User.DeptId), nameof(Id))]
        public List<User> Users { get; set; }

        /// <summary>
        /// 用户集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(RoleDepartment), nameof(RoleDepartment.DeptId), nameof(RoleDepartment.RoleId))]
        public List<Role> Roles { get; set; }
    }
}
