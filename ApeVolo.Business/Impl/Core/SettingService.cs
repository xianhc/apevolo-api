using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Caches.Redis.Extensions;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Core;
using AutoMapper;

namespace ApeVolo.Business.Impl.Core
{
    public class SettingService : BaseServices<Setting>, ISettingService
    {
        #region 字段

        private readonly IRedisCacheService _redisCacheService;

        #endregion

        #region 构造函数

        public SettingService(IMapper mapper, ISettingRepository settingRepository,
            IRedisCacheService redisCacheService)
        {
            _mapper = mapper;
            _baseDal = settingRepository;
            _redisCacheService = redisCacheService;
        }

        #endregion

        #region 基础方法

        public async Task<bool> CreateAsync(CreateUpdateSettingDto createUpdateSettingDto)
        {
            if (await IsExistAsync(r => r.IsDeleted == false
                                        && r.Name == createUpdateSettingDto.Name))
            {
                throw new BadRequestException($"设置键=>{createUpdateSettingDto.Name}=>已存在!");
            }

            var setting = _mapper.Map<Setting>(createUpdateSettingDto);
            return await AddEntityAsync(setting);
        }

        public async Task<bool> UpdateAsync(CreateUpdateSettingDto createUpdateSettingDto)
        {
            //取出待更新数据
            var oldSetting = await QueryFirstAsync(x => x.IsDeleted == false && x.Id == createUpdateSettingDto.Id);
            if (oldSetting.IsNull())
            {
                throw new BadRequestException("更新失败=》待更新数据不存在！");
            }

            if (oldSetting.Name != createUpdateSettingDto.Name && await IsExistAsync(x => x.IsDeleted == false
                && x.Name == createUpdateSettingDto.Name))
            {
                throw new BadRequestException($"设置键=>{createUpdateSettingDto.Name}=>已存在！");
            }

            await _redisCacheService.RemoveAsync(RedisKey.LoadSettingByName + oldSetting.Name.ToMd5String());
            var setting = _mapper.Map<Setting>(createUpdateSettingDto);
            return await UpdateEntityAsync(setting);
        }

        public async Task<bool> DeleteAsync(HashSet<string> ids)
        {
            var settings = await QueryByIdsAsync(ids);
            settings.ForEach(async x =>
            {
                await _redisCacheService.RemoveAsync(RedisKey.LoadSettingByName + x.Name.ToMd5String());
            });
            return await DeleteEntityListAsync(settings);
        }

        public async Task<List<SettingDto>> QueryAsync(SettingQueryCriteria settingQueryCriteria, Pagination pagination)
        {
            Expression<Func<Setting, bool>> whereLambda = r => (r.IsDeleted == false);
            if (!settingQueryCriteria.KeyWords.IsNullOrEmpty())
            {
                whereLambda = whereLambda.And(r =>
                    r.Name.Contains(settingQueryCriteria.KeyWords) || r.Value.Contains(settingQueryCriteria.KeyWords) ||
                    r.Description.Contains(settingQueryCriteria.KeyWords));
            }

            if (!settingQueryCriteria.Enabled.IsNullOrEmpty())
            {
                whereLambda = whereLambda.And(x => x.Enabled == settingQueryCriteria.Enabled);
            }

            if (!settingQueryCriteria.CreateTime.IsNull())
            {
                whereLambda = whereLambda.And(r =>
                    r.CreateTime >= settingQueryCriteria.CreateTime[0] &&
                    r.CreateTime <= settingQueryCriteria.CreateTime[1]);
            }

            return _mapper.Map<List<SettingDto>>(await _baseDal.QueryPageListAsync(whereLambda, pagination));
        }

        public async Task<List<ExportRowModel>> DownloadAsync(SettingQueryCriteria settingQueryCriteria)
        {
            var settingDtos = await QueryAsync(settingQueryCriteria, new Pagination() {PageSize = 9999});

            List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
            List<ExportColumnModel> exportColumnModels;
            int point;
            settingDtos.ForEach(setting =>
            {
                point = 0;
                exportColumnModels = new List<ExportColumnModel>
                {
                    new() {Key = "ID", Value = setting.Id, Point = point++},
                    new() {Key = "名称", Value = setting.Name, Point = point++},
                    new() {Key = "值", Value = setting.Value.ToString(), Point = point++},
                    new() {Key = "状态", Value = setting.Enabled ? "启用" : "停用", Point = point++},
                    new() {Key = "描述", Value = setting.Description, Point = point++},
                    new()
                    {
                        Key = "创建时间", Value = setting.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                    },
                    new()
                    {
                        Key = "更新时间", Value = setting.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                    },
                    new() {Key = "创建人", Value = setting.CreateBy, Point = point++},
                    new() {Key = "更新人", Value = setting.UpdateBy, Point = point++},
                };
                exportRowModels.Add(new ExportRowModel() {exportColumnModels = exportColumnModels});
            });
            return exportRowModels;
        }

        [RedisCaching(Expiration = 20, KeyPrefix = RedisKey.LoadSettingByName)]
        public async Task<SettingDto> FindSettingByName(string settingName)
        {
            if (settingName.IsNullOrEmpty())
            {
                throw new BadRequestException($"请输入合法的设置键名称!");
            }

            var setting =
                _mapper.Map<SettingDto>(await QueryFirstAsync(x => x.IsDeleted == false && x.Name == settingName));
            if (setting.IsNull())
            {
                throw new BadRequestException($"请输入正确的设置键名称!");
            }

            return setting;
        }

        #endregion
    }
}