using System.Collections.Generic;
using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

/// <summary>
/// 系统菜单
/// </summary>
[SugarTable("sys_menu", "系统菜单")]
public class Menu : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 菜单标题
    /// </summary>
    [SugarColumn(ColumnName = "title", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "菜单标题")]
    public string Title { get; set; }

    /// <summary>
    /// api地址
    /// </summary>
    [SugarColumn(ColumnName = "link_url", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "api地址")]
    public string LinkUrl { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [SugarColumn(ColumnName = "path", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "组件路径")]
    public string Path { get; set; }

    /// <summary>
    /// 权限标识符
    /// </summary>
    [SugarColumn(ColumnName = "permission", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "权限标识符")]
    public string Permission { get; set; }

    /// <summary>
    /// 是否iframe
    /// </summary>
    [SugarColumn(ColumnName = "i_frame", IsNullable = false, ColumnDescription = "是否iframe")]
    public bool IFrame { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    [SugarColumn(ColumnName = "component", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "组件")]
    public string Component { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    [SugarColumn(ColumnName = "component_name", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "组件名称")]
    public string ComponentName { get; set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    [SugarColumn(ColumnName = "parent_id", ColumnDataType = "bigint", Length = 19, IsNullable = true,
        ColumnDescription = "父级ID")]
    public long? PId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnName = "menu_sort", IsNullable = true, ColumnDescription = "排序标识符 越小越靠前")]
    public int MenuSort { get; set; }

    /// <summary>
    /// icon图标
    /// </summary>
    [SugarColumn(ColumnName = "icon", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "icon图标")]
    public string Icon { get; set; }

    /// <summary>
    /// 类型
    /// 1.目录 2.菜单 3.按钮
    /// </summary>
    [SugarColumn(ColumnName = "type", IsNullable = false, ColumnDescription = "类型：1.目录 2.菜单 3.按钮")]
    public int Type { get; set; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    [SugarColumn(ColumnName = "cache", IsNullable = true, ColumnDescription = "是否缓存")]
    public bool Cache { get; set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    [SugarColumn(ColumnName = "hidden", IsNullable = true, ColumnDescription = "是否隐藏")]
    public bool Hidden { get; set; }

    /// <summary>
    /// 子节点个数
    /// </summary>
    [SugarColumn(ColumnName = "sub_count", IsNullable = true, ColumnDescription = "子节点个数")]
    public int SubCount { get; set; }

    /// <summary>
    /// 子菜单集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Menu> Children { get; set; }
}