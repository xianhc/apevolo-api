using System;
using ApeVolo.Common.DI;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;

namespace ApeVolo.Common.Extention
{
    public static partial class ExtObject
    {
        /// <summary>
        /// 初始实体信息
        /// </summary>
        /// <param name="entity"></param>
        public static void InitEntity(this object entity)
        {
            var curUser = AutofacHelper.GetScopeService<ICurrentUser>();

            if (entity.ContainsProperty("Id"))
                entity.SetPropertyValue("Id", IdHelper.GetLongId());
            if (entity.ContainsProperty("CreateTime"))
                entity.SetPropertyValue("CreateTime", DateTime.Now);
            //if (entity.ContainsProperty("UpdateTime"))
            //    entity.SetPropertyValue("UpdateTime", DateTime.Now);
            if (entity.ContainsProperty("CreateBy"))
                entity.SetPropertyValue("CreateBy", curUser?.Name);
            if (entity.ContainsProperty("IsDeleted"))
                entity.SetPropertyValue("IsDeleted", false);
        }

        /// <summary>
        /// 编辑实体信息
        /// </summary>
        /// <param name="entity"></param>
        public static void EditEntity(this object entity)
        {
            var curUser = AutofacHelper.GetScopeService<ICurrentUser>();

            if (entity.ContainsProperty("UpdateTime"))
                entity.SetPropertyValue("UpdateTime", DateTime.Now);
            if (entity.ContainsProperty("UpdateBy"))
                entity.SetPropertyValue("UpdateBy", curUser?.Name);
        }

        /// <summary>
        /// 编辑实体信息
        /// </summary>
        /// <param name="entity"></param>
        public static void DelEntity(this object entity)
        {
            var curUser = AutofacHelper.GetScopeService<ICurrentUser>();

            if (entity.ContainsProperty("UpdateTime"))
                entity.SetPropertyValue("UpdateTime", DateTime.Now);
            if (entity.ContainsProperty("UpdateBy"))
                entity.SetPropertyValue("UpdateBy", curUser?.Name);
            if (entity.ContainsProperty("IsDeleted"))
                entity.SetPropertyValue("IsDeleted", true);
        }
    }
}