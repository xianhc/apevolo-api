using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Dictionary;
using ApeVolo.IBusiness.Dto.Dictionary;
using ApeVolo.IBusiness.EditDto.Dict;
using ApeVolo.IBusiness.Interface.Dictionary;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Dictionary;
using AutoMapper;
using SqlSugar;

namespace ApeVolo.Business.Impl.Dictionary
{
    /// <summary>
    /// 字典详情服务
    /// </summary>
    public class DictDetailService : BaseServices<DictDetail>, IDictDetailService
    {
        #region 构造函数

        public DictDetailService(IMapper mapper, IDictDetailRepository repository)
        {
            _mapper = mapper;
            _baseDal = repository;
        }

        #endregion

        #region 基础方法

        public async Task<bool> CreateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto)
        {
            if (await IsExistAsync(dd => dd.IsDeleted == false &&
                                         dd.DictId == createUpdateDictDetailDto.dict.Id &&
                                         dd.Label == createUpdateDictDetailDto.Label &&
                                         dd.Value == createUpdateDictDetailDto.Value))
            {
                throw new BadRequestException($"字典详情资源=>{createUpdateDictDetailDto.Label}=>已存在！");
            }

            var dictDetail = _mapper.Map<DictDetail>(createUpdateDictDetailDto);
            dictDetail.DictId = createUpdateDictDetailDto.dict.Id;
            return await AddEntityAsync(dictDetail);
        }

        public async Task<bool> UpdateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto)
        {
            if (!await IsExistAsync(dd => dd.IsDeleted == false && dd.Id == createUpdateDictDetailDto.Id))
            {
                throw new BadRequestException($"字典详情资源=>{createUpdateDictDetailDto.Label}=>不存在！");
            }

            var dictDetail = _mapper.Map<DictDetail>(createUpdateDictDetailDto);
            dictDetail.DictId = createUpdateDictDetailDto.dict.Id;
            return await UpdateEntityAsync(dictDetail);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var dictDetail = await QuerySingleAsync(id);
            if (dictDetail == null || dictDetail.IsDeleted)
            {
                throw new BadRequestException("删除的资源不存在！");
            }

            return await DeleteEntityAsync(dictDetail);
        }

        public async Task<List<DictDetailDto>> QueryAsync(DictDetailQueryCriteria dictDetailQueryCriteria,
            Pagination pagination)
        {
            pagination.SortFields = new List<string> {"dict_sort asc"};
            var list = await _baseDal.QueryMuchPageAsync<Dict, DictDetail, DictDetail>(pagination,
                (d, dd) => new object[]
                {
                    JoinType.Left, d.Id == dd.DictId,
                },
                (d, dd) => dd,
                (d, dd) => d.IsDeleted == false && dd.IsDeleted == false && d.Name == dictDetailQueryCriteria.DictName
            );
            var dictDetailDtos = _mapper.Map<List<DictDetailDto>>(list);
            dictDetailDtos.ForEach(dd => dd.Dict = new DictDto2 {Id = dd.DictId});
            return dictDetailDtos;
        }

        #endregion
    }
}