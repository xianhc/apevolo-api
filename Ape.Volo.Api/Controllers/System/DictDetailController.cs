using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.Interface.System;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.System;

/// <summary>
/// 字典详情管理
/// </summary>
[Area("字典详情管理")]
[Route("/api/dictDetail", Order = 8)]
public class DictDetailController : BaseApiController
{
    #region 字段

    private readonly IDictDetailService _dictDetailService;
    private readonly IDictService _dictService;

    #endregion

    #region 构造函数

    public DictDetailController(IDictDetailService dictDetailService, IDictService dictService)
    {
        _dictDetailService = dictDetailService;
        _dictService = dictService;
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
    public async Task<ActionResult> Create(
        [FromBody] CreateUpdateDictDetailDto createUpdateDictDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

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
    public async Task<ActionResult> Update(
        [FromBody] CreateUpdateDictDetailDto createUpdateDictDetailDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

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
    public async Task<ActionResult> Delete(long id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id cannot be empty");
        }

        await _dictDetailService.DeleteAsync(id);
        return Success();
    }

    /// <summary>
    /// 查看字典详情列表
    /// </summary>
    /// <param name="dictName"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult> Query(string dictName)
    {
        var list = await _dictDetailService.QueryAsync(dictName);
        return JsonContent(new ActionResultVm<DictDetailDto>
        {
            Content = list,
            TotalElements = list.Count
        });
    }

    #endregion
}
