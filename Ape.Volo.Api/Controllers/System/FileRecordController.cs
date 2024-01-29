using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.ActionExtension.Parameter;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.System;

/// <summary>
/// 文件存储管理
/// </summary>
[Area("系统管理")]
[Route("/api/storage")]
public class FileRecordController : BaseApiController
{
    #region 字段

    private readonly IFileRecordService _fileRecordService;

    #endregion

    #region 构造函数

    public FileRecordController(IFileRecordService fileRecordService)
    {
        _fileRecordService = fileRecordService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增文件
    /// </summary>
    /// <param name="file"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [HttpOptions, HttpPost]
    [Route("upload")]
    [Description("创建")]
    [CheckParamNotEmpty("description")]
    public async Task<ActionResult<object>> Upload(string description,
        [FromForm] IFormFile file)
    {
        if (file.IsNull())
        {
            return Error("请选择一个文件再尝试!");
        }

        var fileLimitSize = _fileRecordService.ApeContext.Configs.FileLimitSize * 1024 * 1024;
        if (file.Length > fileLimitSize)
        {
            return Error($"文件过大，请选择文件小于等于{fileLimitSize}MB的重新进行尝试!");
        }

        await _fileRecordService.CreateAsync(description, file);
        return Create();
    }

    /// <summary>
    /// 更新文件描述
    /// </summary>
    /// <param name="createUpdateAppSecretDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateFileRecordDto createUpdateAppSecretDto)
    {
        await _fileRecordService.UpdateAsync(createUpdateAppSecretDto);
        return NoContent();
    }

    /// <summary>
    /// 删除文件
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

        await _fileRecordService.DeleteAsync(idCollection.IdArray);
        return Success();
    }


    /// <summary>
    /// 获取文件列表
    /// </summary>
    /// <param name="fileRecordQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult<object>> Query(FileRecordQueryCriteria fileRecordQueryCriteria,
        Pagination pagination)
    {
        var fileRecordList = await _fileRecordService.QueryAsync(fileRecordQueryCriteria, pagination);

        return JsonContent(new ActionResultVm<FileRecordDto>
        {
            Content = fileRecordList,
            TotalElements = pagination.TotalElements
        });
    }


    /// <summary>
    /// 导出文件记录
    /// </summary>
    /// <param name="fileRecordQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(FileRecordQueryCriteria fileRecordQueryCriteria)
    {
        var fileRecordExports = await _fileRecordService.DownloadAsync(fileRecordQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(fileRecordExports, out var mimeType);
        return File(data, mimeType);
    }

    #endregion
}
