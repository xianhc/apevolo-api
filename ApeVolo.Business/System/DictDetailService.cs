using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.System;
using ApeVolo.IBusiness.Dto.System;
using ApeVolo.IBusiness.Interface.System;
using ApeVolo.IBusiness.QueryModel;
using SqlSugar;

namespace ApeVolo.Business.System;

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
                dd.DictId == createUpdateDictDetailDto.dict.Id &&
                dd.Label == createUpdateDictDetailDto.Label &&
                dd.Value == createUpdateDictDetailDto.Value).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("DictDetail"),
                createUpdateDictDetailDto.Label));
        }

        var dictDetail = ApeContext.Mapper.Map<DictDetail>(createUpdateDictDetailDto);
        dictDetail.DictId = createUpdateDictDetailDto.dict.Id;
        return await AddEntityAsync(dictDetail);
    }

    public async Task<bool> UpdateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto)
    {
        if (!await TableWhere(dd => dd.Id == createUpdateDictDetailDto.Id).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        var dictDetail = ApeContext.Mapper.Map<DictDetail>(createUpdateDictDetailDto);
        dictDetail.DictId = createUpdateDictDetailDto.dict.Id;
        return await UpdateEntityAsync(dictDetail);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var newId = Convert.ToInt64(id);
        var dictDetail = await TableWhere(x => x.Id == newId).FirstAsync();
        if (dictDetail == null)
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
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
