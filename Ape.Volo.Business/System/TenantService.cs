using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.Enums;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.ExportModel.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.Business.System;

/// <summary>
/// 租户服务
/// </summary>
public class TenantService : BaseServices<Tenant>, ITenantService
{
    #region 构造函数

    public TenantService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    public async Task<bool> CreateAsync(CreateUpdateTenantDto createUpdateTenantDtoDto)
    {
        if (await TableWhere(r => r.TenantId == createUpdateTenantDtoDto.TenantId).AnyAsync())
        {
            throw new BadRequestException($"租户Id=>{createUpdateTenantDtoDto.TenantId}=>已存在!");
        }

        if (createUpdateTenantDtoDto.TenantType == TenantType.Db)
        {
            if (createUpdateTenantDtoDto.DbType.IsNull())
            {
                throw new BadRequestException($"数据库类型不能为空");
            }

            if (createUpdateTenantDtoDto.ConfigId.IsNullOrEmpty())
            {
                throw new BadRequestException($"数据库标识ID不能为空");
            }

            if (createUpdateTenantDtoDto.Connection.IsNullOrEmpty())
            {
                throw new BadRequestException($"数据库连接不能为空");
            }

            if (await TableWhere(r => r.ConfigId == createUpdateTenantDtoDto.ConfigId).AnyAsync())
            {
                throw new BadRequestException($"标识Id=>{createUpdateTenantDtoDto.ConfigId}=>已存在!");
            }
        }

        var tenant = ApeContext.Mapper.Map<Tenant>(createUpdateTenantDtoDto);
        return await AddEntityAsync(tenant);
    }

    public async Task<bool> UpdateAsync(CreateUpdateTenantDto createUpdateTenantDtoDto)
    {
        //取出待更新数据
        var oldTenant = await TableWhere(x => x.Id == createUpdateTenantDtoDto.Id).FirstAsync();
        if (oldTenant.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (oldTenant.TenantId != createUpdateTenantDtoDto.TenantId &&
            await TableWhere(x => x.TenantId == createUpdateTenantDtoDto.TenantId).AnyAsync())
        {
            throw new BadRequestException($"租户Id=>{createUpdateTenantDtoDto.TenantId}=>已存在!");
        }

        if (createUpdateTenantDtoDto.TenantType == TenantType.Db)
        {
            if (createUpdateTenantDtoDto.DbType.IsNull())
            {
                throw new BadRequestException($"数据库类型不能为空");
            }

            if (createUpdateTenantDtoDto.ConfigId.IsNullOrEmpty())
            {
                throw new BadRequestException($"数据库标识ID不能为空");
            }

            if (createUpdateTenantDtoDto.Connection.IsNullOrEmpty())
            {
                throw new BadRequestException($"数据库连接不能为空");
            }

            if (oldTenant.ConfigId != createUpdateTenantDtoDto.ConfigId &&
                await TableWhere(x => x.ConfigId == createUpdateTenantDtoDto.ConfigId).AnyAsync())
            {
                throw new BadRequestException($"标识Id=>{createUpdateTenantDtoDto.ConfigId}=>已存在!");
            }
        }

        var tenant = ApeContext.Mapper.Map<Tenant>(createUpdateTenantDtoDto);
        return await UpdateEntityAsync(tenant, [nameof(Tenant.TenantId)]);
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var tenants = await TableWhere(x => ids.Contains(x.Id)).Includes(x => x.Users).ToListAsync();
        if (tenants.Any(x => x.Users != null && x.Users.Count != 0))
        {
            throw new BadRequestException("存在用户关联，请解除后再试！");
        }

        return await LogicDelete<Tenant>(x => ids.Contains(x.Id)) > 0;
    }

    public async Task<List<TenantDto>> QueryAsync(TenantQueryCriteria tenantQueryCriteria, Pagination pagination)
    {
        var whereExpression = GetWhereExpression(tenantQueryCriteria);
        var queryOptions = new QueryOptions<Tenant>
        {
            Pagination = pagination,
            WhereLambda = whereExpression,
        };
        return ApeContext.Mapper.Map<List<TenantDto>>(
            await SugarRepository.QueryPageListAsync(queryOptions));
    }

    public async Task<List<TenantDto>> QueryAllAsync()
    {
        return ApeContext.Mapper.Map<List<TenantDto>>(
            await Table.ToListAsync());
    }


    public async Task<List<ExportBase>> DownloadAsync(TenantQueryCriteria tenantQueryCriteria)
    {
        var whereExpression = GetWhereExpression(tenantQueryCriteria);
        var tenants = await TableWhere(whereExpression).ToListAsync();
        List<ExportBase> tenantExports = new List<ExportBase>();
        tenantExports.AddRange(tenants.Select(x => new TenantExport()
        {
            TenantId = x.TenantId,
            Name = x.Name,
            Description = x.Description,
            TenantType = x.TenantType,
            ConfigId = x.ConfigId,
            DbType = x.DbType,
            ConnectionString = x.ConnectionString,
            CreateTime = x.CreateTime
        }));
        return tenantExports;
    }


    #region 条件表达式

    private static Expression<Func<Tenant, bool>> GetWhereExpression(TenantQueryCriteria tenantQueryCriteria)
    {
        Expression<Func<Tenant, bool>> whereExpression = r => true;
        if (!tenantQueryCriteria.Name.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(r =>
                r.Name.Contains(tenantQueryCriteria.Name));
        }

        if (tenantQueryCriteria.TenantType.IsNotNull())
        {
            whereExpression = whereExpression.AndAlso(x => x.TenantType == tenantQueryCriteria.TenantType);
        }

        if (tenantQueryCriteria.DbType.IsNotNull())
        {
            whereExpression = whereExpression.AndAlso(x => x.DbType == tenantQueryCriteria.DbType);
        }

        if (tenantQueryCriteria.CreateTime.IsNotNull())
        {
            whereExpression = whereExpression.AndAlso(r =>
                r.CreateTime >= tenantQueryCriteria.CreateTime[0] &&
                r.CreateTime <= tenantQueryCriteria.CreateTime[1]);
        }

        return whereExpression;
    }

    #endregion
}
