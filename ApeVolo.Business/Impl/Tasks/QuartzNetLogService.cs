using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Tasks;
using ApeVolo.IBusiness.Dto.Tasks;
using ApeVolo.IBusiness.Interface.Tasks;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Tasks;
using AutoMapper;

namespace ApeVolo.Business.Impl.Tasks
{
    /// <summary>
    /// QuartzNet作业日志服务
    /// </summary>
    public class QuartzNetLogService : BaseServices<QuartzNetLog>, IQuartzNetLogService
    {
        #region 构造函数

        public QuartzNetLogService(IQuartzNetLogRepository taskQuartzLogRepository, IMapper mapper)
        {
            _baseDal = taskQuartzLogRepository;
            _mapper = mapper;
        }

        #endregion

        #region 基础方法

        public async Task<bool> CreateAsync(QuartzNetLog quartzNetLog)
        {
            return await _baseDal.AddReturnBoolAsync(quartzNetLog);
        }

        public async Task<List<QuartzNetLogDto>> QueryAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria,
            Pagination pagination)
        {
            Expression<Func<QuartzNetLog, bool>> whereExpression = x => x.IsDeleted == false;

            if (!quartzNetLogQueryCriteria.Id.IsNullOrEmpty())
            {
                whereExpression = whereExpression.And(x => x.TaskId == quartzNetLogQueryCriteria.Id);
            }

            if (quartzNetLogQueryCriteria.IsSuccess.HasValue)
            {
                whereExpression = whereExpression.And(x => x.IsSuccess == quartzNetLogQueryCriteria.IsSuccess);
            }

            if (!quartzNetLogQueryCriteria.CreateTime.IsNullOrEmpty() && quartzNetLogQueryCriteria.CreateTime.Count > 1)
            {
                whereExpression = whereExpression.And(x =>
                    x.CreateTime >= quartzNetLogQueryCriteria.CreateTime[0] &&
                    x.CreateTime <= quartzNetLogQueryCriteria.CreateTime[1]);
            }

            return _mapper.Map<List<QuartzNetLogDto>>(await _baseDal.QueryPageListAsync(whereExpression, pagination));
        }

        public async Task<List<ExportRowModel>> DownloadAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria)
        {
            var quartzNetLogList =
                await QueryAsync(quartzNetLogQueryCriteria, new Pagination { PageSize = 9999 });
            List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
            List<ExportColumnModel> exportColumnModels;
            int point = 0;
            quartzNetLogList.ForEach(quartzNetLogDto =>
            {
                point = 0;
                exportColumnModels = new List<ExportColumnModel>();
                exportColumnModels.Add(
                    new ExportColumnModel { Key = "任务ID", Value = quartzNetLogDto.TaskId, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "任务名称", Value = quartzNetLogDto.TaskName, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "任务分组", Value = quartzNetLogDto.TaskGroup, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "程序集", Value = quartzNetLogDto.Cron, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "执行类", Value = quartzNetLogDto.AssemblyName, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "cron表达式", Value = quartzNetLogDto.Cron, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "执行参数", Value = quartzNetLogDto.RunParams, Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                {
                    Key = "异常详情",
                    Value = quartzNetLogDto.ExceptionDetail.Length > 100
                        ? quartzNetLogDto.ExceptionDetail.Substring(1, 100)
                        : quartzNetLogDto.ExceptionDetail,
                    Point = point++
                });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "耗时(毫秒)", Value = quartzNetLogDto.ExecutionDuration.ToString(), Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                    { Key = "状态", Value = quartzNetLogDto.IsSuccess ? "成功" : "失败", Point = point++ });
                exportColumnModels.Add(new ExportColumnModel
                {
                    Key = "执行时间", Value = quartzNetLogDto.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                });
                exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
            });
            return exportRowModels;
        }

        #endregion
    }
}