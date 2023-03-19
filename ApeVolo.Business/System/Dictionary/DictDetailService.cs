using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Entity.System.Dictionary;
using ApeVolo.IBusiness.Dto.System.Dict;
using ApeVolo.IBusiness.Dto.System.Dict.Detail;
using ApeVolo.IBusiness.Interface.System.Dictionary;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.System.Dictionary;
using AutoMapper;
using SqlSugar;

namespace ApeVolo.Business.System.Dictionary;

/// <summary>
/// 字典详情服务
/// </summary>
public class DictDetailService : BaseServices<DictDetail>, IDictDetailService
{
    #region 构造函数

    public DictDetailService(IMapper mapper, IDictDetailRepository repository)
    {
        Mapper = mapper;
        BaseDal = repository;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto)
    {
        if (await IsExistAsync(dd =>
                dd.DictId == createUpdateDictDetailDto.dict.Id &&
                dd.Label == createUpdateDictDetailDto.Label &&
                dd.Value == createUpdateDictDetailDto.Value))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("DictDetail"),
                createUpdateDictDetailDto.Label));
        }

        var dictDetail = Mapper.Map<DictDetail>(createUpdateDictDetailDto);
        dictDetail.DictId = createUpdateDictDetailDto.dict.Id;
        return await AddEntityAsync(dictDetail);
    }

    public async Task<bool> UpdateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto)
    {
        if (!await IsExistAsync(dd => dd.Id == createUpdateDictDetailDto.Id))
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        var dictDetail = Mapper.Map<DictDetail>(createUpdateDictDetailDto);
        dictDetail.DictId = createUpdateDictDetailDto.dict.Id;
        return await UpdateEntityAsync(dictDetail);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var dictDetail = await QuerySingleAsync(id);
        if (dictDetail == null)
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        return await DeleteEntityAsync(dictDetail);
    }

    public async Task<List<DictDetailDto>> QueryAsync(DictDetailQueryCriteria dictDetailQueryCriteria,
        Pagination pagination)
    {
        pagination.SortFields = new List<string> { "dict_sort asc" };
        var list = await BaseDal.QueryMuchPageAsync<Dict, DictDetail, DictDetail>(pagination,
            (d, dd) => new object[]
            {
                JoinType.Left, d.Id == dd.DictId,
            },
            (d, dd) => dd,
            (d, dd) => d.Name == dictDetailQueryCriteria.DictName
        );
        var dictDetailDtos = Mapper.Map<List<DictDetailDto>>(list);
        dictDetailDtos.ForEach(dd => dd.Dict = new DictDto2 { Id = dd.DictId });
        return dictDetailDtos;
    }

    #endregion
}