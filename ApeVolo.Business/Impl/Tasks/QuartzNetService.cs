using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Tasks;
using ApeVolo.IBusiness.Dto.Tasks;
using ApeVolo.IBusiness.EditDto.Tasks;
using ApeVolo.IBusiness.Interface.Tasks;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Tasks;
using AutoMapper;

namespace ApeVolo.Business.Impl.Tasks
{
    /// <summary>
    /// QuartzNet作业服务
    /// </summary>
    public class QuartzNetService : BaseServices<QuartzNet>, IQuartzNetService
    {
        #region 字段

        private readonly IQuartzNetLogService _quartzNetLogService;

        #endregion

        #region 构造函数

        public QuartzNetService(IQuartzNetRepository taskQuartzRepository, IMapper mapper,
            IQuartzNetLogService quartzNetLogService)
        {
            _baseDal = taskQuartzRepository;
            _mapper = mapper;
            _quartzNetLogService = quartzNetLogService;
        }

        #endregion

        #region 基础方法

        public async Task<List<QuartzNet>> QueryAllAsync()
        {
            return await _baseDal.QueryListAsync(x => x.IsDeleted == false);
        }


        public async Task<QuartzNet> CreateAsync(CreateUpdateQuartzNetDto createUpdateQuartzNetDto)
        {
            var quartzNet = _mapper.Map<QuartzNet>(createUpdateQuartzNetDto);
            return await _baseDal.AddReturnEntityAsync(quartzNet);
        }

        public async Task<bool> UpdateAsync(CreateUpdateQuartzNetDto createUpdateQuartzNetDto)
        {
            var quartzNet = _mapper.Map<QuartzNet>(createUpdateQuartzNetDto);
            return await UpdateEntityAsync(quartzNet);
        }


        [UseTran]
        public async Task<bool> UpdateJobInfoAsync(QuartzNet quartzNet, QuartzNetLog quartzNetLog)
        {
            await _baseDal.UpdateAsync(quartzNet);
            await _quartzNetLogService.CreateAsync(quartzNetLog);
            return true;
        }

        public async Task<bool> DeleteAsync(List<QuartzNet> quartzNets)
        {
            quartzNets.DelEntity();
            return await DeleteEntityListAsync(quartzNets);
        }

        public async Task<List<QuartzNetDto>> QueryAsync(QuartzNetQueryCriteria quartzNetQueryCriteria,
            Pagination pagination)
        {
            Expression<Func<QuartzNet, bool>> whereExpression = x => x.IsDeleted == false;
            if (!quartzNetQueryCriteria.TaskName.IsNullOrEmpty())
            {
                whereExpression = whereExpression.And(x => x.TaskName.Contains(quartzNetQueryCriteria.TaskName));
            }

            if (!quartzNetQueryCriteria.CreateTime.IsNullOrEmpty() && quartzNetQueryCriteria.CreateTime.Count > 1)
            {
                whereExpression = whereExpression.And(x =>
                    x.CreateTime >= quartzNetQueryCriteria.CreateTime[0] &&
                    x.CreateTime <= quartzNetQueryCriteria.CreateTime[1]);
            }

            return _mapper.Map<List<QuartzNetDto>>(await _baseDal.QueryPageListAsync(whereExpression, pagination));
        }

        public async Task<List<ExportRowModel>> DownloadAsync(QuartzNetQueryCriteria quartzNetQueryCriteria)
        {
            var quartzNets = await QueryAsync(quartzNetQueryCriteria, new Pagination { PageSize = 9999 });
            List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
            List<ExportColumnModel> exportColumnModels;
            int point = 0;
            quartzNets.ForEach(quartzNet =>
            {
                point = 0;
                exportColumnModels = new List<ExportColumnModel>();
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "ID", Value = quartzNet.Id.ToString(), Point = point++ });
                exportColumnModels.Add(
                    new ExportColumnModel { Key = "作业名称", Value = quartzNet.TaskName, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "作业分组", Value = quartzNet.TaskGroup, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "Cron表达式", Value = quartzNet.Cron, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "程序集名称", Value = quartzNet.AssemblyName, Point = point++ });
                exportColumnModels.Add(
                    new ExportColumnModel { Key = "执行类", Value = quartzNet.ClassName, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "作业描述", Value = quartzNet.Description, Point = point++ });
                exportColumnModels.Add(
                    new ExportColumnModel { Key = "负责人", Value = quartzNet.Principal, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "告警邮箱", Value = quartzNet.AlertEmail, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "失败后是否继续", Value = quartzNet.PauseAfterFailure ? "是" : "否", Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "已执行次数", Value = quartzNet.RunTimes.ToString(), Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "开始时间", Value = quartzNet.StartTime?.ToString(), Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "结束时间", Value = quartzNet.EndTime?.ToString(), Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "触发器类型", Value = quartzNet.TriggerType == 1 ? "cron" : "simple", Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "间隔时间(秒)", Value = quartzNet.IntervalSecond.ToString(), Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "循环次数", Value = quartzNet.CycleRunTimes.ToString(), Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "是否启用", Value = quartzNet.IsEnable ? "是" : "否", Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "执行参数", Value = quartzNet.RunParams, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "创建人", Value = quartzNet.CreateBy, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "创建时间", Value = quartzNet.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++ });
                exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
            });
            return exportRowModels;
        }

        #endregion
    }
}