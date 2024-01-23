using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.ExportModel.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.Business.System;

public class SettingService : BaseServices<Setting>, ISettingService
{
    #region 构造函数

    public SettingService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(CreateUpdateSettingDto createUpdateSettingDto)
    {
        if (await TableWhere(r => r.Name == createUpdateSettingDto.Name).AnyAsync())
        {
            throw new BadRequestException($"设置键=>{createUpdateSettingDto.Name}=>已存在!");
        }

        var setting = ApeContext.Mapper.Map<Setting>(createUpdateSettingDto);
        return await AddEntityAsync(setting);
    }

    public async Task<bool> UpdateAsync(CreateUpdateSettingDto createUpdateSettingDto)
    {
        //取出待更新数据
        var oldSetting = await TableWhere(x => x.Id == createUpdateSettingDto.Id).FirstAsync();
        if (oldSetting.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (oldSetting.Name != createUpdateSettingDto.Name &&
            await TableWhere(x => x.Name == createUpdateSettingDto.Name).AnyAsync())
        {
            throw new BadRequestException($"设置键=>{createUpdateSettingDto.Name}=>已存在!");
        }

        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.LoadSettingByName +
                                           oldSetting.Name.ToMd5String16());
        var setting = ApeContext.Mapper.Map<Setting>(createUpdateSettingDto);
        return await UpdateEntityAsync(setting);
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var settings = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        foreach (var setting in settings)
        {
            await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.LoadSettingByName +
                                               setting.Name.ToMd5String16());
        }

        return await LogicDelete<Setting>(x => ids.Contains(x.Id)) > 0;
    }

    public async Task<List<SettingDto>> QueryAsync(SettingQueryCriteria settingQueryCriteria, Pagination pagination)
    {
        var whereExpression = GetWhereExpression(settingQueryCriteria);
        return ApeContext.Mapper.Map<List<SettingDto>>(
            await SugarRepository.QueryPageListAsync(whereExpression, pagination));
    }

    public async Task<List<ExportBase>> DownloadAsync(SettingQueryCriteria settingQueryCriteria)
    {
        var whereExpression = GetWhereExpression(settingQueryCriteria);
        var settings = await TableWhere(whereExpression).ToListAsync();
        List<ExportBase> settingExports = new List<ExportBase>();
        settingExports.AddRange(settings.Select(x => new SettingExport()
        {
            Name = x.Name,
            Value = x.Value,
            EnabledState = x.Enabled ? EnabledState.Enabled : EnabledState.Disabled,
            Description = x.Description,
            CreateTime = x.CreateTime
        }));
        return settingExports;
    }

    [UseCache(Expiration = 20, KeyPrefix = GlobalConstants.CacheKey.LoadSettingByName)]
    public async Task<SettingDto> FindSettingByName(string settingName)
    {
        if (settingName.IsNullOrEmpty())
        {
            throw new BadRequestException("设置键不能为空");
        }

        SettingDto settingDto = null;
        var setting = await TableWhere(x => x.Name == settingName).FirstAsync();
        if (setting != null)
        {
            settingDto = ApeContext.Mapper.Map<SettingDto>(setting);
        }

        return settingDto;
    }

    #endregion


    #region 条件表达式

    private static Expression<Func<Setting, bool>> GetWhereExpression(SettingQueryCriteria settingQueryCriteria)
    {
        Expression<Func<Setting, bool>> whereExpression = r => true;
        if (!settingQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(r =>
                r.Name.Contains(settingQueryCriteria.KeyWords) || r.Value.Contains(settingQueryCriteria.KeyWords) ||
                r.Description.Contains(settingQueryCriteria.KeyWords));
        }

        if (!settingQueryCriteria.Enabled.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Enabled == settingQueryCriteria.Enabled);
        }

        if (!settingQueryCriteria.CreateTime.IsNull())
        {
            whereExpression = whereExpression.AndAlso(r =>
                r.CreateTime >= settingQueryCriteria.CreateTime[0] &&
                r.CreateTime <= settingQueryCriteria.CreateTime[1]);
        }

        return whereExpression;
    }

    #endregion
}
