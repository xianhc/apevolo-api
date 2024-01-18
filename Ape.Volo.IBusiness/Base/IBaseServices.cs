using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using SqlSugar;

namespace Ape.Volo.IBusiness.Base;

/// <summary>
/// 业务基类 常用增删查改方法
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IBaseServices<TEntity> where TEntity : class
{
    ISqlSugarClient SugarClient { get; }

    ApeContext ApeContext { get; }


    #region 新增

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

    #endregion

    #region 修改

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

    #endregion

    #region 删除

    /// <summary>
    /// 逻辑删除 操作的类需继承ISoftDeletedEntity
    /// </summary>
    /// <param name="exp"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<int> LogicDelete<T>(Expression<Func<T, bool>> exp) where T : class, ISoftDeletedEntity, new();

    #endregion

    #region Queryable

    /// <summary>
    /// 泛型Queryable
    /// </summary>
    ISugarQueryable<TEntity> Table { get; }

    /// <summary>
    /// 泛型Queryable
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <param name="orderExpression">排序表达式</param>
    /// <param name="orderByType">排序方式</param>
    /// <returns></returns>
    ISugarQueryable<TEntity> TableWhere(Expression<Func<TEntity, bool>> whereExpression = null,
        Expression<Func<TEntity, object>> orderExpression = null, OrderByType orderByType = OrderByType.Desc);

    #endregion
}
