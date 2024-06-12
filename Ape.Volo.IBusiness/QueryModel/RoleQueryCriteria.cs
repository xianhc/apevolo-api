namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 角色查询参数
/// </summary>
public class RoleQueryCriteria : DateRange
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; }
}
