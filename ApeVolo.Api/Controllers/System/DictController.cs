using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.System;
using ApeVolo.IBusiness.Interface.System;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.System;

/// <summary>
/// 字典管理
/// </summary>
[Area("系统管理")]
[Route("/api/dict")]
public class DictController : BaseApiController
{
    #region 字段

    private readonly IDictService _dictService;

    #endregion

    #region 构造函数

    public DictController(IDictService dictService)
    {
        _dictService = dictService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增字典
    /// </summary>
    /// <param name="createUpdateDictDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("创建")]
    public async Task<ActionResult<object>> Create([FromBody] CreateUpdateDictDto createUpdateDictDto)
    {
        await _dictService.CreateAsync(createUpdateDictDto);
        return Success();
    }


    /// <summary>
    /// 更新字典
    /// </summary>
    /// <param name="createUpdateDictDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    public async Task<ActionResult<object>> Update([FromBody] CreateUpdateDictDto createUpdateDictDto)
    {
        await _dictService.UpdateAsync(createUpdateDictDto);
        return NoContent();
    }

    /// <summary>
    /// 删除字典
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("删除")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _dictService.DeleteAsync(idCollection.IdArray);
        return Success();
    }

    /// <summary>
    /// 查看字典列表
    /// </summary>
    /// <param name="dictQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult<object>> Query(DictQueryCriteria dictQueryCriteria,
        Pagination pagination)
    {
        var list = await _dictService.QueryAsync(dictQueryCriteria, pagination);

        return new ActionResultVm<DictDto>
        {
            Content = list,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    /// <summary>
    /// 导出字典
    /// </summary>
    /// <param name="dictQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(DictQueryCriteria dictQueryCriteria)
    {
        var dictExports = await _dictService.DownloadAsync(dictQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(dictExports, out var mimeType);
        return File(data, mimeType);
    }

    #endregion
}
