using System;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Ape.Volo.Repository.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(ISqlSugarClient sqlSugarClient, ILogger<UnitOfWork> logger)
    {
        _sqlSugarClient = sqlSugarClient;
        _logger = logger;
    }

    /// <summary>
    /// 获取DB，保证唯一性
    /// </summary>
    /// <returns></returns>
    public SqlSugarScope GetDbClient()
    {
        return _sqlSugarClient as SqlSugarScope;
    }

    public void BeginTran()
    {
        GetDbClient().BeginTran();
    }

    public void CommitTran()
    {
        try
        {
            GetDbClient().CommitTran();
        }
        catch (Exception ex)
        {
            GetDbClient().RollbackTran();
            _logger.LogCritical(ex.Message);
            throw;
        }
    }

    public void RollbackTran()
    {
        GetDbClient().RollbackTran();
    }
}
