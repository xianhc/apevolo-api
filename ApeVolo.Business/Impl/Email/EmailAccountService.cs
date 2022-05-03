using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Email;
using ApeVolo.IBusiness.Dto.Email;
using ApeVolo.IBusiness.EditDto.Email;
using ApeVolo.IBusiness.Interface.Email;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Email;
using AutoMapper;

namespace ApeVolo.Business.Impl.Email;

public class EmailAccountService : BaseServices<EmailAccount>, IEmailAccountService
{
    #region 字段

    #endregion

    #region 构造函数

    public EmailAccountService(IEmailAccountRepository emailAccountRepository, IMapper mapper)
    {
        BaseDal = emailAccountRepository;
        Mapper = mapper;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="createUpdateEmailAccountDto"></param>
    /// <returns></returns>
    public async Task<bool> CreateAsync(CreateUpdateEmailAccountDto createUpdateEmailAccountDto)
    {
        if (await IsExistAsync(x => x.IsDeleted == false
                                    && x.Email == createUpdateEmailAccountDto.Email))
        {
            throw new BadRequestException($"邮箱=>{createUpdateEmailAccountDto.Email}=>已存在!");
        }

        var emailAccount = Mapper.Map<EmailAccount>(createUpdateEmailAccountDto);
        return await AddEntityAsync(emailAccount);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateEmailAccountDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(CreateUpdateEmailAccountDto createUpdateEmailAccountDto)
    {
        if (!await IsExistAsync(x => x.IsDeleted == false
                                     && x.Id == createUpdateEmailAccountDto.Id))
        {
            throw new BadRequestException($"邮箱=>{nameof(EmailAccount)}=>不存在!");
        }

        var emailAccount = Mapper.Map<EmailAccount>(createUpdateEmailAccountDto);
        return await UpdateEntityAsync(emailAccount);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var emailAccounts = await QueryByIdsAsync(ids);
        if (emailAccounts.Count < 1)
            throw new BadRequestException("无可删除数据!");

        return await DeleteEntityListAsync(emailAccounts);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="emailAccountQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<EmailAccountDto>> QueryAsync(EmailAccountQueryCriteria emailAccountQueryCriteria,
        Pagination pagination)
    {
        Expression<Func<EmailAccount, bool>> whereExpression = x => x.IsDeleted == false;
        if (!emailAccountQueryCriteria.Username.IsNullOrEmpty())
        {
            whereExpression = whereExpression.And(x => x.Username.Contains(emailAccountQueryCriteria.Username));
        }

        if (!emailAccountQueryCriteria.DisplayName.IsNullOrEmpty())
        {
            whereExpression =
                whereExpression.And(x => x.DisplayName.Contains(emailAccountQueryCriteria.DisplayName));
        }

        if (!emailAccountQueryCriteria.CreateTime.IsNullOrEmpty() && emailAccountQueryCriteria.CreateTime.Count > 1)
        {
            whereExpression = whereExpression.And(x =>
                x.CreateTime >= emailAccountQueryCriteria.CreateTime[0] &&
                x.CreateTime <= emailAccountQueryCriteria.CreateTime[1]);
        }

        return Mapper.Map<List<EmailAccountDto>>(await BaseDal.QueryPageListAsync(whereExpression, pagination));
    }

    public async Task<List<ExportRowModel>> DownloadAsync(EmailAccountQueryCriteria emailAccountQueryCriteria)
    {
        var emailAccountDtos =
            await QueryAsync(emailAccountQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
        List<ExportColumnModel> exportColumnModels;
        int point;
        emailAccountDtos.ForEach(emailAccountDto =>
        {
            point = 0;
            exportColumnModels = new List<ExportColumnModel>();
            exportColumnModels.Add(new ExportColumnModel
                { Key = "ID", Value = emailAccountDto.Id.ToString(), Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "邮箱地址", Value = emailAccountDto.Email, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "显示名称", Value = emailAccountDto.DisplayName, Point = point++ });
            exportColumnModels.Add(
                new ExportColumnModel { Key = "主机", Value = emailAccountDto.Host, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "端口", Value = emailAccountDto.Port.ToString(), Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "用户名", Value = emailAccountDto.Username, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "是否SSL", Value = emailAccountDto.EnableSsl ? "是" : "否", Point = point++ });
            ;
            exportColumnModels.Add(new ExportColumnModel
                { Key = "发送系统票据", Value = emailAccountDto.UseDefaultCredentials ? "是" : "否", Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
                { Key = "创建人", Value = emailAccountDto.CreateBy, Point = point++ });
            exportColumnModels.Add(new ExportColumnModel
            {
                Key = "创建时间", Value = emailAccountDto.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
            });
            exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
        });
        return exportRowModels;
    }

    #endregion
}