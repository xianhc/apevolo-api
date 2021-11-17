using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApeVolo.IBusiness.Base
{
    /// <summary>
    /// 业务基类 常用增删查改方法
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseServices<TEntity> where TEntity : class
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<bool> AddEntityAsync(TEntity entity);


        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns></returns>
        Task<bool> AddEntityListAsync(List<TEntity> entitys);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns></returns>
        Task<bool> UpdateEntityAsync(TEntity entity, List<string> lstIgnoreColumns = null, bool isLock = true);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns></returns>
        Task<bool> UpdateEntityListAsync(List<TEntity> entitys);

        /// <summary>
        /// 批量删除实体 系统使用的是软删除  实际只是执行 update t set isdeleted=false where ...
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<bool> DeleteEntityAsync(TEntity entity);

        /// <summary>
        /// 批量删除实体 系统使用的是软删除  实际只是执行 update t set isdeleted=false where ...
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<bool> DeleteEntityListAsync(List<TEntity> entitys);

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="id">列值</param>
        /// <param name="columnName">列名 默认ID</param>
        /// <returns></returns>
        Task<TEntity> QuerySingleAsync(object id, string columnName = "id");

        /// <summary>
        /// 批量查询实体
        /// </summary>
        /// <param name="ids">列值</param>
        /// <param name="columnName">列名 默认ID</param>
        /// <returns></returns>
        Task<List<TEntity>> QueryByIdsAsync(List<string> ids, string columnName = "id");

        /// <summary>
        /// 批量查询实体
        /// </summary>
        /// <param name="ids">列值</param>
        /// <param name="columnName">列名 默认ID</param>
        /// <returns></returns>
        Task<List<TEntity>> QueryByIdsAsync(HashSet<string> ids, string columnName = "id");

        /// <summary>
        /// 对象是否存在
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> whereLambda);

        /// <summary>
        /// 获取第一个
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        Task<TEntity> QueryFirstAsync(Expression<Func<TEntity, bool>> whereLambda = null);
    }
}