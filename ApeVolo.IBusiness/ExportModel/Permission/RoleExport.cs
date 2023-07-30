using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Permission;

public class RoleExport : ExportBase
{
    [Display(Name = "角色名称")]
    public string Name { get; set; }

    [Display(Name = "角色等级")]
    public int Level { get; set; }

    [Display(Name = "角色描述")]
    public string Description { get; set; }

    [Display(Name = "数据范围")]
    public string DataScope { get; set; }

    [Display(Name = "数据部门")]
    public string DataDept { get; set; }

    [Display(Name = "角色代码")]
    public string Permission { get; set; }
}
