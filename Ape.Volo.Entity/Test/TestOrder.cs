using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.Test;

/// <summary>
/// 多租户测试
/// 单表ID隔离注释[MultiDbTenant]，继承ITenantEntity
/// 多库隔离取消继承ITenantEntity，增加[MultiDbTenant]
/// </summary>
[MultiDbTenant]
[SugarTable("test_order")]
public class TestOrder : BaseEntity, ISoftDeletedEntity //, ITenantEntity
{
    /// <summary>
    /// 订单号
    /// </summary>
    public string OrderNo { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string GoodsName { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Qty { get; set; }

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int TenantId { get; set; }
}
