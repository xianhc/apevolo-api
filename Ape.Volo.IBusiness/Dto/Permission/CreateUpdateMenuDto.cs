using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Enums;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 菜单Dto
/// </summary>
[AutoMapping(typeof(Menu), typeof(CreateUpdateMenuDto))]
public class CreateUpdateMenuDto : BaseEntityDto<long>
{
    /// <summary>
    /// 标题
    /// </summary>
    [Required]
    public string Title { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string Permission { get; set; }

    /// <summary>
    /// 是否IFrame
    /// </summary>
    public bool IFrame { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string ComponentName { get; set; }

    /// <summary>
    /// 父级ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(1, 999)]
    public int Sort { get; set; }

    /// <summary>
    /// Icon图标
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [Range(1, 3)]
    public MenuType Type { get; set; }

    /// <summary>
    /// 缓存
    /// </summary>
    public bool Cache { get; set; }

    /// <summary>
    /// 隐藏
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// 子菜单个数
    /// </summary>
    public int SubCount { get; set; }
}
