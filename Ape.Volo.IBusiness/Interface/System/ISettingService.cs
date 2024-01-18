using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.QueryModel;
using ApeVolo.Entity.System;

namespace Ape.Volo.IBusiness.Interface.System;

public interface ISettingService : IBaseServices<Setting>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateSettingDto createUpdateSettingDto);
    Task<bool> UpdateAsync(CreateUpdateSettingDto createUpdateSettingDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<SettingDto>> QueryAsync(SettingQueryCriteria settingQueryCriteria, Pagination pagination);
    Task<List<ExportBase>> DownloadAsync(SettingQueryCriteria settingQueryCriteria);

    Task<SettingDto> FindSettingByName(string settingName);

    #endregion
}
