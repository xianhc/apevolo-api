using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
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
/// 字典服务
/// </summary>
public class DictService : BaseServices<Dict>, IDictService
{
    #region 构造函数

    public DictService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(CreateUpdateDictDto createUpdateDictDto)
    {
        if (await TableWhere(d => d.Name == createUpdateDictDto.Name).AnyAsync())
        {
            throw new BadRequestException($"名称=>{createUpdateDictDto.Name}=>已存在!");
        }

        return await AddEntityAsync(ApeContext.Mapper.Map<Dict>(createUpdateDictDto));
    }

    public async Task<bool> UpdateAsync(CreateUpdateDictDto createUpdateDictDto)
    {
        var oldDict =
            await TableWhere(x => x.Id == createUpdateDictDto.Id).FirstAsync();
        if (oldDict.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (oldDict.Name != createUpdateDictDto.Name &&
            await TableWhere(j => j.Name == createUpdateDictDto.Name).AnyAsync())
        {
            throw new BadRequestException($"名称=>{createUpdateDictDto.Name}=>已存在!");
        }

        return await UpdateEntityAsync(ApeContext.Mapper.Map<Dict>(createUpdateDictDto));
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var dicts = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        if (dicts.Count <= 0)
            throw new BadRequestException("数据不存在！");
        return await LogicDelete<Dict>(x => ids.Contains(x.Id)) > 0;
    }

    public async Task<List<DictDto>> QueryAsync(DictQueryCriteria dictQueryCriteria, Pagination pagination)
    {
        var whereExpression = GetWhereExpression(dictQueryCriteria);
        var queryOptions = new QueryOptions<Dict>
        {
            Pagination = pagination,
            WhereLambda = whereExpression,
            //IsIncludes = true
        };
        var list = await SugarRepository.QueryPageListAsync(queryOptions);
        var dicts = ApeContext.Mapper.Map<List<DictDto>>(list);
        // foreach (var item in dicts)
        // {
        //     item.DictDetails.ForEach(d => d.Dict = new DictDto2 { Id = d.DictId });
        // }

        return dicts;
    }

    public async Task<List<ExportBase>> DownloadAsync(DictQueryCriteria dictQueryCriteria)
    {
        var whereExpression = GetWhereExpression(dictQueryCriteria);
        var dicts = await Table.Includes(x => x.DictDetails).WhereIF(whereExpression != null, whereExpression)
            .ToListAsync();
        List<ExportBase> dictExports = new List<ExportBase>();

        dicts.ForEach(x =>
        {
            dictExports.AddRange(x.DictDetails.Select(d => new DictExport()
            {
                DictType = x.DictType,
                Name = x.Name,
                Description = x.Description,
                Lable = d.Label,
                Value = d.Value,
                CreateTime = x.CreateTime
            }));
        });

        return dictExports;
    }

    #endregion

    #region 条件表达式

    private static Expression<Func<Dict, bool>> GetWhereExpression(DictQueryCriteria dictQueryCriteria)
    {
        Expression<Func<Dict, bool>> whereExpression = u => true;
        if (!dictQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(d =>
                d.Name.Contains(dictQueryCriteria.KeyWords) || d.Description.Contains(dictQueryCriteria.KeyWords));
        }

        if (dictQueryCriteria.DictType.IsNotNull())
        {
            whereExpression = whereExpression.AndAlso(d =>
                d.DictType == dictQueryCriteria.DictType);
        }

        return whereExpression;
    }

    #endregion
}
