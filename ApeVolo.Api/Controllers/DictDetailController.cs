using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Interface.Dictionary;
using ApeVolo.IBusiness.Dto.Dictionary;
using ApeVolo.IBusiness.EditDto.Dict;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;
using ApeVolo.Common.AttributeExt;

namespace ApeVolo.Api.Controllers
{
    /// <summary>
    /// 字典详情管理
    /// </summary>
    [Area("DictDetail")]
    [Route("/api/dictDetail")]
    public class DictDetailController : BaseApiController
    {
        #region 字段

        private readonly IDictDetailService _dictDetailService;

        #endregion

        #region 构造函数

        public DictDetailController(IDictDetailService dictDetailService)
        {
            _dictDetailService = dictDetailService;
        }

        #endregion

        #region 内部接口

        /// <summary>
        /// 新增字典详情
        /// </summary>
        /// <param name="createUpdateDictDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("新增字典详情")]
        [ApeVoloAuthorize(new[] {"admin", "dict_add"})]
        public async Task<ActionResult<object>> Create(
            [FromBody] CreateUpdateDictDetailDto createUpdateDictDto)
        {
            await _dictDetailService.CreateAsync(createUpdateDictDto);
            return Success();
        }


        /// <summary>
        /// 更新字典详情
        /// </summary>
        /// <param name="createUpdateDictDetailDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Description("更新字典详情")]
        [ApeVoloAuthorize(new[] {"admin", "dict_edit"})]
        public async Task<ActionResult<object>> Update(
            [FromBody] CreateUpdateDictDetailDto createUpdateDictDetailDto)
        {
            await _dictDetailService.UpdateAsync(createUpdateDictDetailDto);
            return NoContent();
        }

        /// <summary>
        /// 删除字典详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Description("删除字典详情")]
        [ApeVoloAuthorize(new[] {"admin", "dict_del"})]
        public async Task<ActionResult<object>> Delete(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return Error("id is null");
            }

            await _dictDetailService.DeleteAsync(id);
            return Success();
        }

        /// <summary>
        /// 查看字典详情列表
        /// </summary>
        /// <param name="dictDetailQueryCriteria"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("查看字典详情列表")]
        [ApeVoloAuthorize(new[] {"admin", "dict_list"})]
        public async Task<ActionResult<object>> Query(DictDetailQueryCriteria dictDetailQueryCriteria,
            Pagination pagination)
        {
            var list = await _dictDetailService.QueryAsync(dictDetailQueryCriteria, pagination);
            return new ActionResultVm<DictDetailDto>()
            {
                Content = list,
                TotalElements = pagination.TotalElements
            }.ToJson();
        }

        #endregion
    }
}