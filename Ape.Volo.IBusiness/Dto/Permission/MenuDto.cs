using System.Collections.Generic;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 菜单Dto
/// </summary>
[AutoMapping(typeof(Menu), typeof(MenuDto))]
public class MenuDto : BaseEntityDto<long>
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 路由地址
    /// </summary>
    public string LinkUrl { get; set; }

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
    public int Sort { get; set; }

    /// <summary>
    /// Icon
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 缓存
    /// </summary>
    public bool Cache { get; set; }

    /// <summary>
    /// 隐藏
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// 子节点个数
    /// </summary>
    public int SubCount { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<MenuDto> Children { get; set; }

    /// <summary>
    /// 页
    /// </summary>
    public bool Leaf => SubCount == 0;

    /// <summary>
    /// 是否有子节点
    /// </summary>
    public bool HasChildren => SubCount > 0;

    /// <summary>
    /// 标签
    /// </summary>
    public string Label
    {
        get { return Title; }
    }
}
