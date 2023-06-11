using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Dto.System.AppSecret;
using ApeVolo.IBusiness.ExportModel.System;
using ApeVolo.IBusiness.Interface.System.AppSecret;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.System.AppSecret;
using AutoMapper;

namespace ApeVolo.Business.System.AppSecret;

/// <summary>
/// 应用秘钥
/// </summary>
public class AppSecretService : BaseServices<Entity.System.AppSecret>, IAppSecretService
{
    #region 字段

    #endregion

    #region 构造函数

    public AppSecretService(IMapper mapper, IAppSecretRepository appSecretRepository, ICurrentUser currentUser)
    {
        Mapper = mapper;
        BaseDal = appSecretRepository;
        CurrentUser = currentUser;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(CreateUpdateAppSecretDto createUpdateAppSecretDto)
    {
        if (await IsExistAsync(r => r.AppName == createUpdateAppSecretDto.AppName))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("AppSecret"),
                createUpdateAppSecretDto.AppName));
        }

        var id = IdHelper.GetId();
        createUpdateAppSecretDto.AppId = DateTime.Now.ToString("yyyyMMdd") + id[..8];
        createUpdateAppSecretDto.AppSecretKey =
            (createUpdateAppSecretDto.AppId + id).ToHmacsha256String(AppSettings.GetValue("HmacSecret"));
        var appSecret = Mapper.Map<Entity.System.AppSecret>(createUpdateAppSecretDto);
        return await AddEntityAsync(appSecret);
    }

    public async Task<bool> UpdateAsync(CreateUpdateAppSecretDto createUpdateAppSecretDto)
    {
        //取出待更新数据
        var oldAppSecret = await QueryFirstAsync(x => x.Id == createUpdateAppSecretDto.Id);
        if (oldAppSecret.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldAppSecret.AppName != createUpdateAppSecretDto.AppName &&
            await IsExistAsync(x => x.AppName == createUpdateAppSecretDto.AppName))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("AppSecret"),
                createUpdateAppSecretDto.AppName));
        }

        var appSecret = Mapper.Map<Entity.System.AppSecret>(createUpdateAppSecretDto);
        return await UpdateEntityAsync(appSecret);
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var appSecretList = await QueryByIdsAsync(ids);
        return await DeleteEntityListAsync(appSecretList);
    }

    public async Task<List<AppSecretDto>> QueryAsync(AppsecretQueryCriteria appsecretQueryCriteria,
        Pagination pagination)
    {
        Expression<Func<Entity.System.AppSecret, bool>> whereLambda = r => true;
        if (!appsecretQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereLambda = whereLambda.AndAlso(r =>
                r.AppId.Contains(appsecretQueryCriteria.KeyWords) ||
                r.AppName.Contains(appsecretQueryCriteria.KeyWords) ||
                r.Remark.Contains(appsecretQueryCriteria.KeyWords));
        }

        if (!appsecretQueryCriteria.CreateTime.IsNull())
        {
            whereLambda = whereLambda.AndAlso(r =>
                r.CreateTime >= appsecretQueryCriteria.CreateTime[0] &&
                r.CreateTime <= appsecretQueryCriteria.CreateTime[1]);
        }

        return Mapper.Map<List<AppSecretDto>>(await BaseDal.QueryPageListAsync(whereLambda, pagination));
    }

    public async Task<List<ExportBase>> DownloadAsync(AppsecretQueryCriteria appsecretQueryCriteria)
    {
        var appSecrets = await QueryAsync(appsecretQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportBase> appSecretExports = new List<ExportBase>();
        appSecretExports.AddRange(appSecrets.Select(x => new AppSecretExport()
        {
            AppId = x.AppId,
            AppSecretKey = x.AppSecretKey,
            AppName = x.AppName,
            Remark = x.Remark,
            CreateTime = x.CreateTime
        }));
        return appSecretExports;
    }

    #endregion
}