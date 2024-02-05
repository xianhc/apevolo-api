using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.Permission;

/// <summary>
/// 角色导出模板
/// </summary>
public class RoleExport : ExportBase
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Display(Name = "角色名称")]
    public string Name { get; set; }

    /// <summary>
    /// 角色等级
    /// </summary>
    [Display(Name = "角色等级")]
    public int Level { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    [Display(Name = "角色描述")]
    public string Description { get; set; }

    /// <summary>
    /// 数据范围
    /// </summary>
    [Display(Name = "数据范围")]
    public string DataScope { get; set; }

    /// <summary>
    /// 数据部门
    /// </summary>
    [Display(Name = "数据部门")]
    public string DataDept { get; set; }

    /// <summary>
    /// 角色代码
    /// </summary>
    [Display(Name = "角色代码")]
    public string Permission { get; set; }
}
