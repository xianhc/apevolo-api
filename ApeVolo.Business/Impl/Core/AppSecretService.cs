using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Core;
using AutoMapper;

namespace ApeVolo.Business.Impl.Core;

/// <summary>
/// 应用秘钥
/// </summary>
public class AppSecretService : BaseServices<AppSecret>, IAppSecretService
{
    #region 字段

    #endregion

    #region 构造函数

    public AppSecretService(IMapper mapper, IAppSecretRepository appSecretRepository)
    {
        Mapper = mapper;
        BaseDal = appSecretRepository;
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
        var appSecret = Mapper.Map<AppSecret>(createUpdateAppSecretDto);
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

        var appSecret = Mapper.Map<AppSecret>(createUpdateAppSecretDto);
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
        Expression<Func<AppSecret, bool>> whereLambda = r => true;
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

    public async Task<List<ExportRowModel>> DownloadAsync(AppsecretQueryCriteria appsecretQueryCriteria)
    {
        var appSecretList = await QueryAsync(appsecretQueryCriteria, new Pagination { PageSize = 9999 });

        List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
        List<ExportColumnModel> exportColumnModels;
        int point;
        appSecretList.ForEach(appsecret =>
        {
            point = 0;
            exportColumnModels = new List<ExportColumnModel>
            {
                new() { Key = "ID", Value = appsecret.Id.ToString(), Point = point++ },
                new() { Key = "应用ID", Value = appsecret.AppId, Point = point++ },
                new() { Key = "应用名称", Value = appsecret.AppName, Point = point++ },
                new() { Key = "应用秘钥", Value = appsecret.AppSecretKey, Point = point++ },
                new() { Key = "备注", Value = appsecret.Remark, Point = point++ },
                new()
                {
                    Key = "创建时间", Value = appsecret.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                },
                new()
                {
                    Key = "更新时间", Value = appsecret.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                },
                new() { Key = "创建人", Value = appsecret.CreateBy, Point = point++ },
                new() { Key = "更新人", Value = appsecret.UpdateBy, Point = point++ },
            };
            exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
        });
        return exportRowModels;
    }

    #endregion
}