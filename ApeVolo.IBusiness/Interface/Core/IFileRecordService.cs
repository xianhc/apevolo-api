using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.IBusiness.Interface.Core;

public interface IFileRecordService : IBaseServices<FileRecord>
{
    #region 基础接口

    Task<bool> CreateAsync(string description, IFormFile file);
    Task<bool> UpdateAsync(CreateUpdateFileRecordDto createUpdateFileRecordDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<FileRecordDto>> QueryAsync(FileRecordQueryCriteria fileRecordQueryCriteria, Pagination pagination);
    Task<List<ExportRowModel>> DownloadAsync(FileRecordQueryCriteria fileRecordQueryCriteria);

    #endregion
}