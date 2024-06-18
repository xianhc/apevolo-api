using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.ExportModel.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using SqlSugar;

namespace Ape.Volo.Business.Permission;

public class JobService : BaseServices<Job>, IJobService
{
    #region 构造函数

    public JobService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(CreateUpdateJobDto createUpdateJobDto)
    {
        if (await TableWhere(j => j.Name == createUpdateJobDto.Name).AnyAsync())
        {
            throw new BadRequestException($"岗位名称=>{createUpdateJobDto.Name}=>已存在!");
        }

        var job = ApeContext.Mapper.Map<Job>(createUpdateJobDto);
        return await AddEntityAsync(job);
    }

    public async Task<bool> UpdateAsync(CreateUpdateJobDto createUpdateJobDto)
    {
        var oldJob =
            await TableWhere(x => x.Id == createUpdateJobDto.Id).FirstAsync();
        if (oldJob.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (oldJob.Name != createUpdateJobDto.Name &&
            await TableWhere(j => j.Name == createUpdateJobDto.Name).AnyAsync())
        {
            throw new BadRequestException($"岗位名称=>{createUpdateJobDto.Name}=>已存在!");
        }

        var job = ApeContext.Mapper.Map<Job>(createUpdateJobDto);
        return await UpdateEntityAsync(job);
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var jobs = await TableWhere(x => ids.Contains(x.Id)).Includes(x => x.Users).ToListAsync();
        if (jobs.Count < 1)
        {
            throw new BadRequestException("数据不存在！");
        }

        if (jobs.Any(job => job.Users != null && job.Users.Count != 0))
        {
            throw new BadRequestException("存在用户关联，请解除后再试！");
        }

        return await LogicDelete<Job>(x => ids.Contains(x.Id)) > 0;
    }


    public async Task<List<JobDto>> QueryAsync(JobQueryCriteria jobQueryCriteria, Pagination pagination)
    {
        var whereExpression = GetWhereExpression(jobQueryCriteria);
        var queryOptions = new QueryOptions<Job>
        {
            Pagination = pagination,
            WhereLambda = whereExpression,
        };
        return ApeContext.Mapper.Map<List<JobDto>>(
            await SugarRepository.QueryPageListAsync(queryOptions));
    }

    public async Task<List<ExportBase>> DownloadAsync(JobQueryCriteria jobQueryCriteria)
    {
        var whereExpression = GetWhereExpression(jobQueryCriteria);
        var jbos = await TableWhere(whereExpression).ToListAsync();
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
        Expression<Func<Job, bool>> whereExpression = x => x.Enabled;


        return ApeContext.Mapper.Map<List<JobDto>>(await SugarRepository.QueryListAsync(whereExpression, x => x.Sort,
            OrderByType.Asc));
    }

    #endregion

    #region 条件表达式

    private static Expression<Func<Job, bool>> GetWhereExpression(JobQueryCriteria jobQueryCriteria)
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

        return whereExpression;
    }

    #endregion
}
