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
using ApeVolo.Entity.System.Dictionary;
using ApeVolo.IBusiness.Dto.System.Dict;
using ApeVolo.IBusiness.ExportModel.System;
using ApeVolo.IBusiness.Interface.System.Dictionary;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.System.Dictionary;
using AutoMapper;

namespace ApeVolo.Business.System.Dictionary;

/// <summary>
/// 字典服务
/// </summary>
public class DictService : BaseServices<Dict>, IDictService
{
    #region 构造函数

    public DictService(IDictRepository dictRepository, IMapper mapper, ICurrentUser currentUser)
    {
        BaseDal = dictRepository;
        Mapper = mapper;
        CurrentUser = currentUser;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(CreateUpdateDictDto createUpdateDictDto)
    {
        if (await IsExistAsync(d => d.Name == createUpdateDictDto.Name))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Dict"),
                createUpdateDictDto.Name));
        }

        return await AddEntityAsync(Mapper.Map<Dict>(createUpdateDictDto));
    }

    public async Task<bool> UpdateAsync(CreateUpdateDictDto createUpdateDictDto)
    {
        var oldDict =
            await QueryFirstAsync(x => x.Id == createUpdateDictDto.Id);
        if (oldDict.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldDict.Name != createUpdateDictDto.Name && await IsExistAsync(j => j.Id == createUpdateDictDto.Id))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Dict"),
                createUpdateDictDto.Name));
        }

        return await UpdateEntityAsync(Mapper.Map<Dict>(createUpdateDictDto));
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var dicts = await QueryByIdsAsync(ids);
        return await DeleteEntityListAsync(dicts);
    }

    public async Task<List<DictDto>> QueryAsync(DictQueryCriteria dictQueryCriteria, Pagination pagination)
    {
        Expression<Func<Dict, bool>> whereLambda = u => true;
        if (!dictQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereLambda = whereLambda.AndAlso(d =>
                d.Name.Contains(dictQueryCriteria.KeyWords) || d.Description.Contains(dictQueryCriteria.KeyWords));
        }

        var list = await BaseDal.QueryMapperPageListAsync(it => it.DictDetails,
            it => it.DictDetails.FirstOrDefault().DictId, whereLambda, pagination);
        var dicts = Mapper.Map<List<DictDto>>(list);
        foreach (var item in dicts)
        {
            item.DictDetails.ForEach(d => d.Dict = new DictDto2 { Id = d.DictId });
        }

        return dicts;
    }

    public async Task<List<ExportBase>> DownloadAsync(DictQueryCriteria dictQueryCriteria)
    {
        var dicts = await QueryAsync(dictQueryCriteria, new Pagination { PageSize = 9999 });
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
}