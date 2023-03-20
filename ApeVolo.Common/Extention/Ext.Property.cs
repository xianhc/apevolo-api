using System;
using ApeVolo.Common.DI;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;

namespace ApeVolo.Common.Extention;

public static partial class ExtObject
{
    /// <summary>
    /// 初始实体信息
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="currentUser"></param>
    public static void InitEntity(this object entity, ICurrentUser currentUser)
    {
        if (entity.ContainsProperty("Id"))
            entity.SetPropertyValue("Id", IdHelper.GetLongId());
        if (entity.ContainsProperty("CreateBy"))
            entity.SetPropertyValue("CreateBy", currentUser?.Name);
        if (entity.ContainsProperty("CreateTime"))
            entity.SetPropertyValue("CreateTime", DateTime.Now);
    }

    /// <summary>
    /// 编辑实体信息
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="currentUser"></param>
    public static void EditEntity(this object entity, ICurrentUser currentUser)
    {
        if (entity.ContainsProperty("UpdateBy"))
            entity.SetPropertyValue("UpdateBy", currentUser?.Name);
        if (entity.ContainsProperty("UpdateTime"))
            entity.SetPropertyValue("UpdateTime", DateTime.Now);
    }

    /// <summary>
    /// 删除实体信息
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="currentUser"></param>
    public static void DelEntity(this object entity, ICurrentUser currentUser)
    {
        if (entity.ContainsProperty("IsDeleted"))
            entity.SetPropertyValue("IsDeleted", true);
        if (entity.ContainsProperty("DeletedBy"))
            entity.SetPropertyValue("DeletedBy", currentUser?.Name);
        if (entity.ContainsProperty("DeletedTime"))
            entity.SetPropertyValue("DeletedTime", DateTime.Now);
    }
}