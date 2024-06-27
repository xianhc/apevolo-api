using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.Interface.System;
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
                dd.DictId == createUpdateDictDetailDto.DictId &&
                (dd.Label == createUpdateDictDetailDto.Label ||
                 dd.Value == createUpdateDictDetailDto.Value)).AnyAsync())
        {
            throw new BadRequestException(
                $"字典详情标签或值=>({createUpdateDictDetailDto.Label},{createUpdateDictDetailDto.Value})=>已存在!");
        }

        var dictDetail = ApeContext.Mapper.Map<DictDetail>(createUpdateDictDetailDto);
        //dictDetail.DictId = createUpdateDictDetailDto.Dict.Id;
        dictDetail.DictId = createUpdateDictDetailDto.DictId;
        return await AddEntityAsync(dictDetail);
    }

    public async Task<bool> UpdateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto)
    {
        var oldDictDetail =
            await TableWhere(x => x.Id == createUpdateDictDetailDto.Id).FirstAsync();

        if (oldDictDetail.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (oldDictDetail.DictId != createUpdateDictDetailDto.DictId &&
            await TableWhere(dd =>
                dd.DictId == createUpdateDictDetailDto.DictId &&
                (dd.Label == createUpdateDictDetailDto.Label ||
                 dd.Value == createUpdateDictDetailDto.Value)).AnyAsync())
        {
            throw new BadRequestException(
                $"字典详情标签或值=>({createUpdateDictDetailDto.Label},{createUpdateDictDetailDto.Value})=>已存在!");
        }


        var dictDetail = ApeContext.Mapper.Map<DictDetail>(createUpdateDictDetailDto);
        dictDetail.DictId = createUpdateDictDetailDto.DictId;
        return await UpdateEntityAsync(dictDetail);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var dictDetail = await TableWhere(x => x.Id == id).FirstAsync();
        if (dictDetail == null)
        {
            throw new BadRequestException("数据不存在！");
        }

        return await LogicDelete<DictDetail>(x => x.Id == id) > 0;
    }

    public async Task<List<DictDetailDto>> QueryAsync(string dictName)
    {
        //这样写生成的key太长太多
        // var list = await SugarClient.Queryable<Dict>().RightJoin<DictDetail>((d, dd) => d.Id == dd.DictId)
        //     .Where((d, dd) => d.Name == dictName).OrderBy((d, dd) => dd.DictSort).Select((d, dd) => dd).WithCache(86400)
        //     .ToListAsync();


        var dictList = await SugarClient.Queryable<Dict>().WithCache(86400).ToListAsync();
        var dictDetailList = await Table.WithCache(86400).ToListAsync();
        var dictModel = dictList.FirstOrDefault(x => x.Name == dictName);
        if (dictModel != null)
        {
            var dictDetailDtos =
                ApeContext.Mapper.Map<List<DictDetailDto>>(dictDetailList.Where(x => x.DictId == dictModel.Id)
                    .ToList());

            //dictDetailDtos.ForEach(dd => dd.Dict = new DictDto2 { Id = dd.DictId });
            return dictDetailDtos.OrderBy(x => x.DictSort).ToList();
        }

        return new List<DictDetailDto>();
    }

    #endregion
}
