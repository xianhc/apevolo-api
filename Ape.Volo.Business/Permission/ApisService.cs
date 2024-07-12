using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.Business.Permission;

public class ApisService : BaseServices<Apis>, IApisService
{
    public ApisService()
    {
    }

    public async Task<bool> CreateAsync(CreateUpdateApisDto createUpdateApisDto)
    {
        if (await TableWhere(a => a.Url == createUpdateApisDto.Url).AnyAsync())
        {
            throw new BadRequestException($"Url=>{createUpdateApisDto.Url}=>已存在!");
        }

        var apis = App.Mapper.MapTo<Apis>(createUpdateApisDto);
        return await AddEntityAsync(apis);
    }

    public async Task<bool> UpdateAsync(CreateUpdateApisDto createUpdateApisDto)
    {
        var oldApis =
            await TableWhere(x => x.Id == createUpdateApisDto.Id).FirstAsync();
        if (oldApis.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (oldApis.Url != createUpdateApisDto.Url &&
            await TableWhere(a => a.Url == createUpdateApisDto.Url).AnyAsync())
        {
            throw new BadRequestException($"Url=>{createUpdateApisDto.Url}=>已存在!");
        }

        var apis = App.Mapper.MapTo<Apis>(createUpdateApisDto);
        return await UpdateEntityAsync(apis);
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var apis = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        if (apis.Count < 1)
        {
            throw new BadRequestException("数据不存在！");
        }

        return await LogicDelete<Apis>(x => ids.Contains(x.Id)) > 0;
    }

    public async Task<List<Apis>> QueryAsync(ApisQueryCriteria apisQueryCriteria, Pagination pagination)
    {
        Expression<Func<Apis, bool>> whereExpression = x => true;
        if (!apisQueryCriteria.Group.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Group.Contains(apisQueryCriteria.Group));
        }

        if (!apisQueryCriteria.Description.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Description.Contains(apisQueryCriteria.Description));
        }

        if (!apisQueryCriteria.Method.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Method == apisQueryCriteria.Method);
        }

        var queryOptions = new QueryOptions<Apis>
        {
            Pagination = pagination,
            WhereLambda = whereExpression,
        };
        return await SugarRepository.QueryPageListAsync(queryOptions);
    }

    public async Task<List<Apis>> QueryAllAsync()
    {
        return await Table.ToListAsync();
    }

    public async Task<bool> CreateAsync(List<Apis> apis)
    {
        return await AddEntityListAsync(apis);
    }
}
