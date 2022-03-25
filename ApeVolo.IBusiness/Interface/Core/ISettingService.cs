using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.Core;

public interface ISettingService : IBaseServices<Setting>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateSettingDto createUpdateRoleDto);
    Task<bool> UpdateAsync(CreateUpdateSettingDto createUpdateRoleDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<SettingDto>> QueryAsync(SettingQueryCriteria settingQueryCriteria, Pagination pagination);
    Task<List<ExportRowModel>> DownloadAsync(SettingQueryCriteria settingQueryCriteria);

    Task<SettingDto> FindSettingByName(string settingName);

    #endregion
}