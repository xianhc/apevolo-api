using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Monitor;
using Ape.Volo.IBusiness.Dto.Monitor;
using Ape.Volo.IBusiness.Interface.Monitor;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.Business.Monitor;

/// <summary>
/// 审计日志服务
/// </summary>
public class AuditInfoService : BaseServices<AuditLog>, IAuditLogService
{
    #region 构造函数

    public AuditInfoService()
    {
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(AuditLog auditLog)
    {
        //return await SugarRepository.AddReturnBoolAsync(auditInfo);
        return await SugarRepository.SugarClient.Insertable(auditLog).SplitTable().ExecuteCommandAsync() > 0;
    }


    public async Task<bool> CreateListAsync(List<AuditLog> auditLogs)
    {
        //return await SugarRepository.AddReturnBoolAsync(auditInfo);
        return await SugarRepository.SugarClient.Insertable(auditLogs).SplitTable().ExecuteCommandAsync() > 0;
    }


    public async Task<List<AuditLogDto>> QueryAsync(LogQueryCriteria logQueryCriteria,
        Pagination pagination)
    {
        Expression<Func<AuditLog, bool>> whereLambda = l => true;
        if (!logQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereLambda = whereLambda.AndAlso(l => l.Description.Contains(logQueryCriteria.KeyWords));
        }

        if (!logQueryCriteria.CreateTime.IsNullOrEmpty())
        {
            whereLambda = whereLambda.AndAlso(l =>
                l.CreateTime >= logQueryCriteria.CreateTime[0] && l.CreateTime <= logQueryCriteria.CreateTime[1]);
        }

        var queryOptions = new QueryOptions<AuditLog>
        {
            Pagination = pagination,
            WhereLambda = whereLambda,
            IsSplitTable = true
        };

        var auditInfos = await SugarRepository.QueryPageListAsync(queryOptions);
        return App.Mapper.MapTo<List<AuditLogDto>>(auditInfos);
    }

    public async Task<List<AuditLogDto>> QueryByCurrentAsync(Pagination pagination)
    {
        Expression<Func<AuditLog, bool>> whereLambda = x => x.CreateBy == App.HttpUser.Account;


        Expression<Func<AuditLog, AuditLog>> selectExpression = x => new AuditLog
        {
            Id = x.Id, Description = x.Description, RequestIp = x.RequestIp, IpAddress = x.IpAddress,
            OperatingSystem = x.OperatingSystem, DeviceType = x.DeviceType, BrowserName = x.BrowserName,
            Version = x.Version, ExecutionDuration = x.ExecutionDuration, CreateTime = x.CreateTime
        };
        var queryOptions = new QueryOptions<AuditLog>
        {
            Pagination = pagination,
            WhereLambda = whereLambda,
            SelectExpression = selectExpression,
            IsSplitTable = true
        };
        var auditInfos = await SugarRepository.QueryPageListAsync(queryOptions);
        return App.Mapper.MapTo<List<AuditLogDto>>(auditInfos);
    }

    #endregion
}
