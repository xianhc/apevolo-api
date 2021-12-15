using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.IBusiness.Dto.Logs;
using ApeVolo.IBusiness.Interface.Logs;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Logs;
using AutoMapper;

namespace ApeVolo.Business.Impl.Logs
{
    /// <summary>
    /// 审计日志服务
    /// </summary>
    public class AuditInfoService : BaseServices<AuditLog>, IAuditLogService
    {
        #region 构造函数

        public AuditInfoService(IAuditLogRepository auditInfoRepository, IMapper mapper)
        {
            _baseDal = auditInfoRepository;
            _mapper = mapper;
        }

        #endregion

        #region 基础方法

        public async Task<bool> CreateAsync(AuditLog auditInfo)
        {
            return await _baseDal.AddReturnBoolAsync(auditInfo);
        }

        public async Task<List<AuditLogDto>> QueryAsync(LogQueryCriteria logQueryCriteria,
            Pagination pagination)
        {
            Expression<Func<AuditLog, bool>> whereLambda = l => l.IsDeleted == false;
            if (!logQueryCriteria.KeyWords.IsNullOrEmpty())
            {
                whereLambda = whereLambda.And(l => l.Description.Contains(logQueryCriteria.KeyWords));
            }

            if (!logQueryCriteria.CreateTime.IsNullOrEmpty())
            {
                whereLambda = whereLambda.And(l =>
                    l.CreateTime >= logQueryCriteria.CreateTime[0] && l.CreateTime <= logQueryCriteria.CreateTime[1]);
            }

            var auditInfos = await _baseDal.QueryPageListAsync(whereLambda, pagination);
            return _mapper.Map<List<AuditLogDto>>(auditInfos);
        }

        public async Task<List<AuditLogDto>> QueryByCurrentAsync(string userName, Pagination pagination)
        {
            Expression<Func<AuditLog, bool>> whereLambda = x => x.IsDeleted == false;
            if (!userName.IsNullOrEmpty())
            {
                whereLambda = whereLambda.And(x => x.CreateBy == userName);
            }


            Expression<Func<AuditLog, AuditLog>> expression = x => new AuditLog
            {
                Id = x.Id, Description = x.Description, RequestIp = x.RequestIp, IpAddress = x.IpAddress,
                BrowserInfo = x.BrowserInfo, ExecutionDuration = x.ExecutionDuration, CreateTime = x.CreateTime
            };

            var auditInfos = await _baseDal.QueryPageListAsync(whereLambda, pagination, expression);
            return _mapper.Map<List<AuditLogDto>>(auditInfos);
        }

        #endregion
    }
}