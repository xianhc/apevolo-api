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
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Core;
using AutoMapper;

namespace ApeVolo.Business.Impl.Core
{
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
            _mapper = mapper;
            _baseDal = appSecretRepository;
        }

        #endregion

        #region 基础方法
        public async Task<bool> CreateAsync(CreateUpdateAppSecretDto createUpdateAppSecretDto)
        {
            if (await IsExistAsync(r => r.IsDeleted == false
                                        && r.AppName == createUpdateAppSecretDto.AppName))
            {
                throw new BadRequestException($"应用名称=>{createUpdateAppSecretDto.AppName}=>已存在!");
            }

            var id = IdHelper.GetId();
            createUpdateAppSecretDto.AppId = DateTime.Now.ToString("yyyyMMdd") + id[..8];
            createUpdateAppSecretDto.AppSecretKey =
                (createUpdateAppSecretDto.AppId + id).ToHmacsha256String(AppSettings.GetValue("HmacSecret"));
            var appSecret = _mapper.Map<AppSecret>(createUpdateAppSecretDto);
            return await AddEntityAsync(appSecret);
        }

        public async Task<bool> UpdateAsync(CreateUpdateAppSecretDto createUpdateAppSecretDto)
        {
            //取出待更新数据
            var oldAppSecret = await QueryFirstAsync(x => x.IsDeleted == false && x.Id == createUpdateAppSecretDto.Id);
            if (oldAppSecret.IsNull())
            {
                throw new BadRequestException("更新失败=》待更新数据不存在！");
            }

            if (oldAppSecret.AppName != createUpdateAppSecretDto.AppName && await IsExistAsync(x => x.IsDeleted == false
                && x.AppName == createUpdateAppSecretDto.AppName))
            {
                throw new BadRequestException($"应用名称=>{createUpdateAppSecretDto.AppName}=>已存在！");
            }

            var appSecret = _mapper.Map<AppSecret>(createUpdateAppSecretDto);
            return await UpdateEntityAsync(appSecret);
        }

        public async Task<bool> DeleteAsync(HashSet<string> ids)
        {
            var appSecretList = await QueryByIdsAsync(ids);
            return await DeleteEntityListAsync(appSecretList);
        }

        public async Task<List<AppSecretDto>> QueryAsync(AppsecretQueryCriteria appsecretQueryCriteria,
            Pagination pagination)
        {
            Expression<Func<AppSecret, bool>> whereLambda = r => (r.IsDeleted == false);
            if (!appsecretQueryCriteria.KeyWords.IsNullOrEmpty())
            {
                whereLambda = whereLambda.And(r =>
                    r.AppId.Contains(appsecretQueryCriteria.KeyWords) ||
                    r.AppName.Contains(appsecretQueryCriteria.KeyWords) ||
                    r.Remark.Contains(appsecretQueryCriteria.KeyWords));
            }

            if (!appsecretQueryCriteria.CreateTime.IsNull())
            {
                whereLambda = whereLambda.And(r =>
                    r.CreateTime >= appsecretQueryCriteria.CreateTime[0] &&
                    r.CreateTime <= appsecretQueryCriteria.CreateTime[1]);
            }

            return _mapper.Map<List<AppSecretDto>>(await _baseDal.QueryPageListAsync(whereLambda, pagination));
        }

        public async Task<List<ExportRowModel>> DownloadAsync(AppsecretQueryCriteria appsecretQueryCriteria)
        {
            var appSecretList = await QueryAsync(appsecretQueryCriteria, new Pagination() {PageSize = 9999});

            List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
            List<ExportColumnModel> exportColumnModels;
            int point;
            appSecretList.ForEach(appsecret =>
            {
                point = 0;
                exportColumnModels = new List<ExportColumnModel>
                {
                    new() {Key = "ID", Value = appsecret.Id, Point = point++},
                    new() {Key = "应用ID", Value = appsecret.AppId, Point = point++},
                    new() {Key = "应用名称", Value = appsecret.AppName, Point = point++},
                    new() {Key = "应用秘钥", Value = appsecret.AppSecretKey, Point = point++},
                    new() {Key = "备注", Value = appsecret.Remark, Point = point++},
                    new()
                    {
                        Key = "创建时间", Value = appsecret.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                    },
                    new()
                    {
                        Key = "更新时间", Value = appsecret.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                    },
                    new() {Key = "创建人", Value = appsecret.CreateBy, Point = point++},
                    new() {Key = "更新人", Value = appsecret.UpdateBy, Point = point++},
                };
                exportRowModels.Add(new ExportRowModel() {exportColumnModels = exportColumnModels});
            });
            return exportRowModels;
        }
        #endregion
    }
}