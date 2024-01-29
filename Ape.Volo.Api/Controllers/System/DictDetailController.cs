using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.System;

/// <summary>
/// 字典详情管理
/// </summary>
[Area("系统管理")]
[Route("/api/dictDetail")]
public class DictDetailController : BaseApiController
{
    #region 字段

    private readonly IDictDetailService _dictDetailService;

    #endregion

    #region 构造函数

    public DictDetailController(IDictDetailService dictDetailService)
    {
        _dictDetailService = dictDetailService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增字典详情
    /// </summary>
    /// <param name="createUpdateDictDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("创建")]
    [ApeVoloAuthorize(new[] { "admin", "dict_add" })]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateDictDetailDto createUpdateDictDto)
    {
        await _dictDetailService.CreateAsync(createUpdateDictDto);
        return Success();
    }


    /// <summary>
    /// 更新字典详情
    /// </summary>
    /// <param name="createUpdateDictDetailDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    [ApeVoloAuthorize(new[] { "admin", "dict_edit" })]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateDictDetailDto createUpdateDictDetailDto)
    {
        await _dictDetailService.UpdateAsync(createUpdateDictDetailDto);
        return NoContent();
    }

    /// <summary>
    /// 删除字典详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("删除")]
    [ApeVoloAuthorize(new[] { "admin", "dict_del" })]
    public async Task<ActionResult<object>> Delete(string id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id is null");
        }

        await _dictDetailService.DeleteAsync(id);
        return Success();
    }

    /// <summary>
    /// 查看字典详情列表
    /// </summary>
    /// <param name="dictDetailQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    [ApeVoloAuthorize(new[] { "admin", "dict_list" })]
    public async Task<ActionResult<object>> Query(DictDetailQueryCriteria dictDetailQueryCriteria,
        Pagination pagination)
    {
        var list = await _dictDetailService.QueryAsync(dictDetailQueryCriteria, pagination);
        return JsonContent(new ActionResultVm<DictDetailDto>
        {
            Content = list,
            TotalElements = pagination.TotalElements
        });
    }

    #endregion
}
