using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;
using SqlSugar;

namespace Ape.Volo.Business.System;

/// <summary>
/// 字典详情服务
/// </summary>
public class DictDetailService : BaseServices<DictDetail>, IDictDetailService
{
    #region 构造函数

    public DictDetailService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto)
    {
        if (await TableWhere(dd =>
                dd.DictId == createUpdateDictDetailDto.Dict.Id &&
                dd.Label == createUpdateDictDetailDto.Label &&
                dd.Value == createUpdateDictDetailDto.Value).AnyAsync())
        {
            throw new BadRequestException($"字典详情标签=>{createUpdateDictDetailDto.Label}=>已存在!");
        }

        var dictDetail = ApeContext.Mapper.Map<DictDetail>(createUpdateDictDetailDto);
        dictDetail.DictId = createUpdateDictDetailDto.Dict.Id;
        return await AddEntityAsync(dictDetail);
    }

    public async Task<bool> UpdateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto)
    {
        if (!await TableWhere(dd => dd.Id == createUpdateDictDetailDto.Id).AnyAsync())
        {
            throw new BadRequestException("数据不存在！");
        }

        var dictDetail = ApeContext.Mapper.Map<DictDetail>(createUpdateDictDetailDto);
        dictDetail.DictId = createUpdateDictDetailDto.Dict.Id;
        return await UpdateEntityAsync(dictDetail);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var newId = Convert.ToInt64(id);
        var dictDetail = await TableWhere(x => x.Id == newId).FirstAsync();
        if (dictDetail == null)
        {
            throw new BadRequestException("数据不存在！");
        }

        var logidId = Convert.ToInt64(id);
        return await LogicDelete<DictDetail>(x => x.Id == logidId) > 0;
    }

    public async Task<List<DictDetailDto>> QueryAsync(DictDetailQueryCriteria dictDetailQueryCriteria,
        Pagination pagination)
    {
        pagination.SortFields = new List<string> { "dict_sort asc" };
        var list = await SugarRepository.QueryMuchPageAsync<Dict, DictDetail, DictDetail>(pagination,
            (d, dd) => new object[]
            {
                JoinType.Left, d.Id == dd.DictId,
            },
            (d, dd) => dd,
            (d, dd) => d.Name == dictDetailQueryCriteria.DictName
        );
        var dictDetailDtos = ApeContext.Mapper.Map<List<DictDetailDto>>(list);
        dictDetailDtos.ForEach(dd => dd.Dict = new DictDto2 { Id = dd.DictId });
        return dictDetailDtos;
    }

    #endregion
}
