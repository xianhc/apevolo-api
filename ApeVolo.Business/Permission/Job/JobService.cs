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
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Permission.User;
using ApeVolo.IBusiness.Dto.Permission.Job;
using ApeVolo.IBusiness.ExportModel.Permission;
using ApeVolo.IBusiness.Interface.Permission.Job;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Permission.Job;
using AutoMapper;
using SqlSugar;

namespace ApeVolo.Business.Permission.Job;

public class JobService : BaseServices<Entity.Permission.Job>, IJobService
{
    #region 字段

    #endregion

    #region 构造函数

    public JobService(IJobRepository jobRepository, IMapper mapper, ICurrentUser currentUser)
    {
        BaseDal = jobRepository;
        Mapper = mapper;
        CurrentUser = currentUser;
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

        var job = Mapper.Map<Entity.Permission.Job>(createUpdateJobDto);
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

        if (oldJob.Name != createUpdateJobDto.Name && await IsExistAsync(j => j.Name == createUpdateJobDto.Name))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Job"),
                createUpdateJobDto.Name));
        }

        var job = Mapper.Map<Entity.Permission.Job>(createUpdateJobDto);
        return await UpdateEntityAsync(job);
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var jobs = await QueryByIdsAsync(ids);
        if (jobs.Count < 1)
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        var userJobs = await BaseDal.QueryMuchAsync<UserJobs, Entity.Permission.User.User, UserJobs>(
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
        Expression<Func<Entity.Permission.Job, bool>> whereExpression = x => true;
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

    public async Task<List<ExportBase>> DownloadAsync(JobQueryCriteria jobQueryCriteria)
    {
        var jbos = await QueryAsync(jobQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportBase> roleExports = new List<ExportBase>();
        roleExports.AddRange(jbos.Select(x => new JobExport()
        {
            Id = x.Id,
            Name = x.Name,
            Sort = x.Sort,
            EnabledState = x.Enabled ? EnabledState.Enabled : EnabledState.Disabled,
            CreateTime = x.CreateTime
        }));
        return roleExports;
    }

    #endregion

    #region 扩展方法

    public async Task<List<JobDto>> QueryAllAsync()
    {
        Expression<Func<Entity.Permission.Job, bool>> whereExpression = x => x.Enabled;


        return Mapper.Map<List<JobDto>>(await BaseDal.QueryListAsync(whereExpression, x => x.Sort,
            OrderByType.Asc));
    }

    #endregion
}