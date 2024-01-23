using System;
using System.Collections.Generic;
using Ape.Volo.Common.ConfigOptions;
using Serilog.Context;
using SqlSugar;

namespace Ape.Volo.Api.Serilog;

public class LoggerPropertyConfiguration : IDisposable
{
    private readonly Stack<IDisposable> _disposableStack = new();

    public static LoggerPropertyConfiguration Create => new();

    public void AddStock(IDisposable disposable)
    {
        _disposableStack.Push(disposable);
    }

    public IDisposable AddAopSqlProperty(ISqlSugarClient db, SqlLog sqlLog)
    {
        AddStock(LogContext.PushProperty(LoggerProperty.RecordSqlLog, sqlLog.Enabled));
        AddStock(LogContext.PushProperty(LoggerProperty.ToDb, sqlLog.ToDb.Enabled));
        AddStock(LogContext.PushProperty(LoggerProperty.ToFile, sqlLog.ToFile.Enabled));
        AddStock(LogContext.PushProperty(LoggerProperty.ToConsole, sqlLog.ToConsole.Enabled));
        AddStock(LogContext.PushProperty(LoggerProperty.ToElasticsearch, sqlLog.ToElasticsearch.Enabled));
        AddStock(LogContext.PushProperty(LoggerProperty.SugarActionType, db.SugarActionType));
        return this;
    }


    public void Dispose()
    {
        while (_disposableStack.Count > 0)
        {
            _disposableStack.Pop().Dispose();
        }
    }
}
