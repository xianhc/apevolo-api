using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Http;

namespace Ape.Volo.IBusiness.Interface.System;

/// <summary>
/// 文件记录接口
/// </summary>
public interface IFileRecordService : IBaseServices<FileRecord>
{
    #region 基础接口

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="description"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(string description, IFormFile file);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateFileRecordDto"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(CreateUpdateFileRecordDto createUpdateFileRecordDto);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(HashSet<long> ids);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="fileRecordQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<FileRecordDto>> QueryAsync(FileRecordQueryCriteria fileRecordQueryCriteria, Pagination pagination);

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="fileRecordQueryCriteria"></param>
    /// <returns></returns>
    Task<List<ExportBase>> DownloadAsync(FileRecordQueryCriteria fileRecordQueryCriteria);

    #endregion
}
