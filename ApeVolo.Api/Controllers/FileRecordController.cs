using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.ActionExtension.Parameter;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers;

/// <summary>
/// 文件存储管理
/// </summary>
[Area("FileRecord")]
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
    [Description("{0}Add")]
    [CheckParamNotEmptyAttribute("description")]
    public async Task<ActionResult<object>> Upload(string description,
        [FromForm] IFormFile file)
    {
        if (file.IsNull())
        {
            return Error("请选择一个文件再尝试!");
        }

        var fileLimitSize = AppSettings.GetValue<long>("FileLimitSize") * 1024 * 1024;
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
    [Description("{0}Edit")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateFileRecordDto createUpdateAppSecretDto)
    {
        await _fileRecordService.UpdateAsync(createUpdateAppSecretDto);
        return NoContent();
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("{0}Delete")]
    [NoJsonParamter]
    public async Task<ActionResult<object>> Delete([FromBody] HashSet<long> ids)
    {
        if (ids == null || ids.Count < 1)
        {
            return Error("ids is null");
        }

        await _fileRecordService.DeleteAsync(ids);
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
    [Description("{0}List")]
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
    [Description("{0}Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(FileRecordQueryCriteria fileRecordQueryCriteria)
    {
        var exportRowModels = await _fileRecordService.DownloadAsync(fileRecordQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, Localized.Get("FileRecord"));

        FileInfo fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }

    #endregion
}