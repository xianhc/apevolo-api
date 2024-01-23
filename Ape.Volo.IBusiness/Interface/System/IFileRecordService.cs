using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Http;

namespace Ape.Volo.IBusiness.Interface.System;

public interface IFileRecordService : IBaseServices<FileRecord>
{
    #region 基础接口

    Task<bool> CreateAsync(string description, IFormFile file);
    Task<bool> UpdateAsync(CreateUpdateFileRecordDto createUpdateFileRecordDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<FileRecordDto>> QueryAsync(FileRecordQueryCriteria fileRecordQueryCriteria, Pagination pagination);
    Task<List<ExportBase>> DownloadAsync(FileRecordQueryCriteria fileRecordQueryCriteria);

    #endregion
}
