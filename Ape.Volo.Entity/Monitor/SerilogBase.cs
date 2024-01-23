using System;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.Monitor;

/// <summary>
/// Serilog日志基类
/// </summary>
public class SerilogBase : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 
    /// </summary>
    [SplitField]
    [SugarColumn(IsNullable = true)]
    public new DateTime CreateTime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Level { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "longtext,text,clob")]
    public string Message { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "longtext,text,clob")]
    public string MessageTemplate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "longtext,text,clob")]
    public string Properties { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsDeleted { get; set; }
}
