using System.Collections.Generic;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 系统菜单
/// </summary>
[SugarTable("sys_menu", "系统菜单")]
public class Menu : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 菜单标题
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "菜单标题")]
    public string Title { get; set; }

    /// <summary>
    /// api地址
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "api地址")]
    public string LinkUrl { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "组件路径")]
    public string Path { get; set; }

    /// <summary>
    /// 权限标识符
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "权限标识符")]
    public string Permission { get; set; }

    /// <summary>
    /// 是否iframe
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否iframe")]
    public bool IFrame { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "组件")]
    public string Component { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "组件名称")]
    public string ComponentName { get; set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "父级ID")]
    public long? ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "排序标识符 越小越靠前")]
    public int Sort { get; set; }

    /// <summary>
    /// icon图标
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "icon图标")]
    public string Icon { get; set; }

    /// <summary>
    /// 类型
    /// 1.目录 2.菜单 3.按钮
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "类型：1.目录 2.菜单 3.按钮")]
    public int Type { get; set; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "是否缓存")]
    public bool Cache { get; set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "是否隐藏")]
    public bool Hidden { get; set; }

    /// <summary>
    /// 子节点个数
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "子节点个数")]
    public int SubCount { get; set; }

    /// <summary>
    /// 子菜单集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Menu> Children { get; set; }

    public bool IsDeleted { get; set; }
}
