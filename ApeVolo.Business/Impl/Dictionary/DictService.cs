using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Dictionary;
using ApeVolo.IBusiness.EditDto.Dict;
using ApeVolo.IBusiness.Interface.Dictionary;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Dictionary;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Dictionary;

namespace ApeVolo.Business.Impl.Dictionary
{
    /// <summary>
    /// 字典服务
    /// </summary>
    public class DictService : BaseServices<Dict>, IDictService
    {
        #region 构造函数

        public DictService(IDictRepository dictRepository, IMapper mapper)
        {
            _baseDal = dictRepository;
            _mapper = mapper;
        }

        #endregion

        #region 基础方法

        public async Task<bool> CreateAsync(CreateUpdateDictDto createUpdateDictDto)
        {
            if (await IsExistAsync(d => d.IsDeleted == false && d.Name == createUpdateDictDto.Name))
            {
                throw new BadRequestException($"字典资源>{createUpdateDictDto.Name}=>已存在！");
            }

            return await AddEntityAsync(_mapper.Map<Dict>(createUpdateDictDto));
        }

        public async Task<bool> UpdateAsync(CreateUpdateDictDto createUpdateDictDto)
        {
            if (!await IsExistAsync(d => d.IsDeleted == false && d.Id == createUpdateDictDto.Id))
            {
                throw new BadRequestException($"字典资源=>{createUpdateDictDto.Name}=>不存在！");
            }

            return await UpdateEntityAsync(_mapper.Map<Dict>(createUpdateDictDto));
        }

        public async Task<bool> DeleteAsync(HashSet<string> ids)
        {
            var dicts = await QueryByIdsAsync(ids);
            return await DeleteEntityListAsync(dicts);
        }

        public async Task<List<DictDto>> QueryAsync(DictQueryCriteria dictQueryCriteria, Pagination pagination)
        {
            Expression<Func<Dict, bool>> whereLambda = u => (u.IsDeleted == false);
            if (!dictQueryCriteria.KeyWords.IsNullOrEmpty())
            {
                whereLambda = whereLambda.And(d =>
                    d.Name.Contains(dictQueryCriteria.KeyWords) || d.Description.Contains(dictQueryCriteria.KeyWords));
            }

            var list = await _baseDal.QueryMapperPageListAsync(it => it.DictDetails,
                it => it.DictDetails.FirstOrDefault().DictId, whereLambda, pagination);
            var dicts = _mapper.Map<List<DictDto>>(list);
            foreach (var item in dicts)
            {
                item.DictDetails.ForEach(d => d.Dict = new DictDto2() {Id = d.DictId});
            }

            return dicts;
        }

        public async Task<List<ExportRowModel>> DownloadAsync(DictQueryCriteria dictQueryCriteria)
        {
            var dictList = await QueryAsync(dictQueryCriteria, new Pagination() {PageSize = 9999});
            List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
            List<ExportColumnModel> exportColumnModels;
            int point;
            dictList.ForEach(dict =>
            {
                dict.DictDetails.ForEach(item =>
                {
                    point = 0;
                    exportColumnModels = new List<ExportColumnModel>();
                    exportColumnModels.Add(new ExportColumnModel {Key = "ID", Value = dict.Id, Point = point++});
                    exportColumnModels.Add(new ExportColumnModel {Key = "详情ID", Value = item.Id, Point = point++});
                    exportColumnModels.Add(new ExportColumnModel {Key = "字典代码", Value = dict.Name, Point = point++});
                    exportColumnModels.Add(new ExportColumnModel
                        {Key = "字典名称", Value = dict.Description.ToString(), Point = point++});
                    exportColumnModels.Add(new ExportColumnModel {Key = "字典标签", Value = item.Label, Point = point++});
                    exportColumnModels.Add(new ExportColumnModel {Key = "字典值", Value = item.Value, Point = point++});
                    exportColumnModels.Add(new ExportColumnModel
                        {Key = "创建时间", Value = dict.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++});
                    exportColumnModels.Add(new ExportColumnModel
                        {Key = "值创建时间", Value = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++});
                    exportRowModels.Add(new ExportRowModel() {exportColumnModels = exportColumnModels});
                });
            });
            return exportRowModels;
        }

        #endregion
    }
}