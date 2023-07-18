using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Parameter;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.System;
using ApeVolo.IBusiness.Interface.System;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.System;

/// <summary>
/// 文件存储管理
/// </summary>
[Area("System")]
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
    [Description("Add")]
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
    [Description("Edit")]
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
    [Description("Delete")]
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
    [Description("List")]
    public async Task<ActionResult<object>> Query(FileRecordQueryCriteria fileRecordQueryCriteria,
        Pagination pagination)
    {
        var fileRecordList = await _fileRecordService.QueryAsync(fileRecordQueryCriteria, pagination);

        return new ActionResultVm<FileRecordDto>
        {
            Content = fileRecordList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }


    /// <summary>
    /// 导出文件记录
    /// </summary>
    /// <param name="fileRecordQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(FileRecordQueryCriteria fileRecordQueryCriteria)
    {
        var fileRecordExports = await _fileRecordService.DownloadAsync(fileRecordQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(fileRecordExports, out var mimeType);
        return File(data, mimeType);
    }

    #endregion
}
