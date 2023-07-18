using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.System;
using ApeVolo.IBusiness.Dto.System;
using ApeVolo.IBusiness.ExportModel.System;
using ApeVolo.IBusiness.Interface.System;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.Business.System;

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
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Dict"),
                createUpdateDictDto.Name));
        }

        return await AddEntityAsync(ApeContext.Mapper.Map<Dict>(createUpdateDictDto));
    }

    public async Task<bool> UpdateAsync(CreateUpdateDictDto createUpdateDictDto)
    {
        var oldDict =
            await TableWhere(x => x.Id == createUpdateDictDto.Id).FirstAsync();
        if (oldDict.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldDict.Name != createUpdateDictDto.Name &&
            await TableWhere(j => j.Id == createUpdateDictDto.Id).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Dict"),
                createUpdateDictDto.Name));
        }

        return await UpdateEntityAsync(ApeContext.Mapper.Map<Dict>(createUpdateDictDto));
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var dicts = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        if (dicts.Count <= 0)
            throw new BadRequestException(Localized.Get("DataNotExist"));
        return await LogicDelete<Dict>(x => ids.Contains(x.Id)) > 0;
    }

    public async Task<List<DictDto>> QueryAsync(DictQueryCriteria dictQueryCriteria, Pagination pagination)
    {
        var whereExpression = GetWhereExpression(dictQueryCriteria);
        var list = await SugarRepository.QueryMapperPageListAsync(it => it.DictDetails,
            it => it.DictDetails.FirstOrDefault().DictId, whereExpression, pagination);
        var dicts = ApeContext.Mapper.Map<List<DictDto>>(list);
        foreach (var item in dicts)
        {
            item.DictDetails.ForEach(d => d.Dict = new DictDto2 { Id = d.DictId });
        }

        return dicts;
    }

    public async Task<List<ExportBase>> DownloadAsync(DictQueryCriteria dictQueryCriteria)
    {
        var whereExpression = GetWhereExpression(dictQueryCriteria);
        var dicts = await TableWhere(whereExpression).ToListAsync();
        List<ExportBase> dictExports = new List<ExportBase>();

        dicts.ForEach(x =>
        {
            dictExports.AddRange(x.DictDetails.Select(d => new DictExport()
            {
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

        return whereExpression;
    }

    #endregion
}
