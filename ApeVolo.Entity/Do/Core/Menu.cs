using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core
{
    [InitTable(typeof(Menu))]
    [SugarTable("sys_menu", "系统菜单表")]
    public class Menu : BaseEntity
    {
        [SugarColumn(ColumnName = "title", ColumnDataType = "varchar", Length = 255, IsNullable = false, ColumnDescription = "菜单标题")]
        public string Title { get; set; }

        [SugarColumn(ColumnName = "link_url", ColumnDataType = "varchar", Length = 255, IsNullable = true, ColumnDescription = "菜单路由地址")]
        public string LinkUrl { get; set; }

        [SugarColumn(ColumnName = "path", ColumnDataType = "varchar", Length = 255, IsNullable = true, ColumnDescription = "组件路径")]
        public string Path { get; set; }

        [SugarColumn(ColumnName = "permission", ColumnDataType = "varchar", Length = 255, IsNullable = true, ColumnDescription = "权限标识符")]
        public string Permission { get; set; }

        [SugarColumn(ColumnName = "i_frame", IsNullable = false, ColumnDescription = "是否iframe")]
        public bool IFrame { get; set; }

        [SugarColumn(ColumnName = "component", ColumnDataType = "varchar", Length = 255, IsNullable = true, ColumnDescription = "组件")]
        public string Component { get; set; }

        [SugarColumn(ColumnName = "component_name", ColumnDataType = "varchar", Length = 255, IsNullable = true, ColumnDescription = "组件名称")]
        public string ComponentName { get; set; }

        [SugarColumn(ColumnName = "parent_id", ColumnDataType = "char", Length = 19, IsNullable = true, ColumnDescription = "父级ID")]
        public string PId { get; set; }

        [SugarColumn(ColumnName = "menu_sort", IsNullable = true, ColumnDescription = "排序标识符 越小越靠前")]
        public int MenuSort { get; set; }

        [SugarColumn(ColumnName = "icon", ColumnDataType = "varchar", Length = 255, IsNullable = true, ColumnDescription = "icon图标")]
        public string Icon { get; set; }

        [SugarColumn(ColumnName = "type", IsNullable = false, ColumnDescription = "类型：1.目录 2.菜单 3.按钮")]
        public int Type { get; set; }

        [SugarColumn(ColumnName = "cache", IsNullable = true, ColumnDescription = "是否缓存")]
        public bool Cache { get; set; }

        [SugarColumn(ColumnName = "hidden", IsNullable = true, ColumnDescription = "是否隐藏")]
        public bool Hidden { get; set; }

        /// <summary>
        /// 子节点个数
        /// </summary>
        [SugarColumn(ColumnName = "sub_count", IsNullable = true, ColumnDescription = "子节点个数")]
        public int SubCount { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<Menu> Children { get; set; }
    }
}