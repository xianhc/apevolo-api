using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.IBusiness.Dto.Monitor.Logs.Auditing;
using ApeVolo.IBusiness.Interface.Monitor.Auditing;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Monitor.Auditing;
using AutoMapper;

namespace ApeVolo.Business.Monitor.Auditing;

/// <summary>
/// 审计日志服务
/// </summary>
public class AuditInfoService : BaseServices<AuditLog>, IAuditLogService
{
    #region 构造函数

    public AuditInfoService(IAuditLogRepository auditInfoRepository, IMapper mapper)
    {
        BaseDal = auditInfoRepository;
        Mapper = mapper;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(AuditLog auditInfo)
    {
        return await BaseDal.AddReturnBoolAsync(auditInfo);
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

        var auditInfos = await BaseDal.QueryPageListAsync(whereLambda, pagination);
        return Mapper.Map<List<AuditLogDto>>(auditInfos);
    }

    public async Task<List<AuditLogDto>> QueryByCurrentAsync(string userName, Pagination pagination)
    {
        Expression<Func<AuditLog, bool>> whereLambda = x => true;
        if (!userName.IsNullOrEmpty())
        {
            whereLambda = whereLambda.AndAlso(x => x.CreateBy == userName);
        }


        Expression<Func<AuditLog, AuditLog>> expression = x => new AuditLog
        {
            Id = x.Id, Description = x.Description, RequestIp = x.RequestIp, IpAddress = x.IpAddress,
            OperatingSystem = x.OperatingSystem, DeviceType = x.DeviceType, BrowserName = x.BrowserName,
            Version = x.Version, ExecutionDuration = x.ExecutionDuration, CreateTime = x.CreateTime
        };

        var auditInfos = await BaseDal.QueryPageListAsync(whereLambda, pagination, expression);
        return Mapper.Map<List<AuditLogDto>>(auditInfos);
    }

    #endregion
}