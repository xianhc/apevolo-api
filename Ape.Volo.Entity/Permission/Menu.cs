using System.Collections.Generic;
using Ape.Volo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 系统菜单
/// </summary>
[SugarTable("sys_menu")]
public class Menu : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 菜单标题
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Title { get; set; }

    /// <summary>
    /// api地址
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string LinkUrl { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Path { get; set; }

    /// <summary>
    /// 权限标识符
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Permission { get; set; }

    /// <summary>
    /// 是否iframe
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool IFrame { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Component { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string ComponentName { get; set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public long ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int Sort { get; set; }

    /// <summary>
    /// icon图标
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Icon { get; set; }

    /// <summary>
    /// 类型
    /// 1.目录 2.菜单 3.按钮
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Type { get; set; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public bool Cache { get; set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public bool Hidden { get; set; }

    /// <summary>
    /// 子节点个数
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int SubCount { get; set; }

    /// <summary>
    /// 子菜单集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Menu> Children { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
