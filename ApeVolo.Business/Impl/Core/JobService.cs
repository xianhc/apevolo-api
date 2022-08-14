using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Core;
using AutoMapper;
using SqlSugar;

namespace ApeVolo.Business.Impl.Core;

public class JobService : BaseServices<Job>, IJobService
{
    #region 字段

    #endregion

    #region 构造函数

    public JobService(IJobRepository jobRepository, IMapper mapper)
    {
        BaseDal = jobRepository;
        Mapper = mapper;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(CreateUpdateJobDto createUpdateJobDto)
    {
        if (await IsExistAsync(j => j.Name == createUpdateJobDto.Name))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Job"),
                createUpdateJobDto.Name));
        }

        var job = Mapper.Map<Job>(createUpdateJobDto);
        return await AddEntityAsync(job);
    }

    public async Task<bool> UpdateAsync(CreateUpdateJobDto createUpdateJobDto)
    {
        var oldJob =
            await QueryFirstAsync(x => x.Id == createUpdateJobDto.Id);
        if (oldJob.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldJob.Name != createUpdateJobDto.Name && await IsExistAsync(j => j.Id == createUpdateJobDto.Id))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Job"),
                createUpdateJobDto.Name));
        }

        var job = Mapper.Map<Job>(createUpdateJobDto);
        return await UpdateEntityAsync(job);
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var jobs = await QueryByIdsAsync(ids);
        if (jobs.Count < 1)
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        var userJobs = await BaseDal.QueryMuchAsync<UserJobs, User, UserJobs>(
            (uj, u) => new object[]
            {
                JoinType.Left, uj.UserId == u.Id
            },
            (uj, u) => uj,
            (uj, u) => ids.Contains(uj.JobId)
        );
        if (userJobs.Count > 0)
        {
            throw new BadRequestException(Localized.Get("DataCannotDelete"));
        }

        return await DeleteEntityListAsync(jobs);
    }


    public async Task<List<JobDto>> QueryAsync(JobQueryCriteria jobQueryCriteria, Pagination pagination)
    {
        Expression<Func<Job, bool>> whereExpression = x => true;
        if (!jobQueryCriteria.JobName.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Name.Contains(jobQueryCriteria.JobName));
        }

        if (!jobQueryCriteria.CreateTime.IsNullOrEmpty() && jobQueryCriteria.CreateTime.Count > 1)
        {
            whereExpression = whereExpression.AndAlso(x =>
                x.CreateTime >= jobQueryCriteria.CreateTime[0] && x.CreateTime <= jobQueryCriteria.CreateTime[1]);
        }

        if (!jobQueryCriteria.Enabled.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Enabled == jobQueryCriteria.Enabled);
        }

        return Mapper.Map<List<JobDto>>(await BaseDal.QueryPageListAsync(whereExpression, pagination));
    }

    public async Task<List<ExportRowModel>> DownloadAsync(JobQueryCriteria jobQueryCriteria)
    {
        var jobs = await QueryAsync(jobQueryCriteria, new Pagination { PageSize = 9999 });

        List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
        List<ExportColumnModel> exportColumnModels;
        int point;
        jobs.ForEach(job =>
        {
            point = 0;
            exportColumnModels = new List<ExportColumnModel>
            {
                new() { Key = "ID", Value = job.Id.ToString(), Point = point++ },
                new() { Key = "岗位名称", Value = job.Name, Point = point++ },
                new() { Key = "排序", Value = job.Sort.ToString(), Point = point++ },
                new() { Key = "状态", Value = job.Enabled ? "正常" : "停用", Point = point++ },
                new()
                {
                    Key = "创建时间", Value = job.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                }
            };
            exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
        });
        return exportRowModels;
    }

    #endregion

    #region 扩展方法

    public async Task<List<JobDto>> QueryAllAsync()
    {
        Expression<Func<Job, bool>> whereExpression = x => x.Enabled;


        return Mapper.Map<List<JobDto>>(await BaseDal.QueryListAsync(whereExpression, x => x.Sort,
            OrderByType.Asc));
    }

    #endregion
}