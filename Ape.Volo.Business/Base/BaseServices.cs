using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.Repository.SugarHandler;
using SqlSugar;

namespace Ape.Volo.Business.Base
{
    /// <summary>
    /// 业务实现基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseServices<TEntity> : IBaseServices<TEntity> where TEntity : class, new()
    {
        #region 字段

        /// <summary>
        /// 当前操作对象仓储
        /// </summary>
        public ISugarRepository<TEntity> SugarRepository { get; set; }

        /// <summary>
        /// sugarClient
        /// </summary>
        public ISqlSugarClient SugarClient => SugarRepository.SugarClient;

        #endregion

        #region 构造函数

        public BaseServices(ISugarRepository<TEntity> sugarRepository = null)
        {
            SugarRepository = sugarRepository;
        }

        #endregion

        #region 新增

        public async Task<bool> AddEntityAsync(TEntity entity)
        {
            return await SugarRepository.AddReturnBoolAsync(entity);
        }

        public async Task<bool> AddEntityListAsync(List<TEntity> entityList)
        {
            return await SugarRepository.AddReturnBoolAsync(entityList);
        }

        #endregion

        #region 修改

        public async Task<bool> UpdateEntityAsync(TEntity entity, List<string> lstIgnoreColumns = null,
            bool isLock = true)
        {
            return await SugarRepository.UpdateAsync(entity, lstIgnoreColumns, isLock) > 0;
        }

        public async Task<bool> UpdateEntityListAsync(List<TEntity> entityList)
        {
            return await SugarRepository.UpdateAsync(entityList) > 0;
        }

        #endregion

        #region 删除

        public async Task<int> LogicDelete<T>(Expression<Func<T, bool>> exp) where T : class, ISoftDeletedEntity, new()
        {
            return await SugarClient.Updateable<T>()
                .SetColumns(it => new T() { IsDeleted = true },
                    true) //true 支持更新数据过滤器赋值字段一起更新
                .Where(exp).ExecuteCommandAsync();
        }

        #endregion

        #region Queryable

        public ISugarQueryable<TEntity> Table => SugarClient.Queryable<TEntity>();

        /// <summary>
        /// Table
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <param name="orderExpression">排序表达式</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="isClearCreateByFilter">清除创建人过滤器</param>
        /// <returns></returns>
        public ISugarQueryable<TEntity> TableWhere(Expression<Func<TEntity, bool>> whereExpression = null,
            Expression<Func<TEntity, object>> orderExpression = null, OrderByType? orderByType = null,
            bool isClearCreateByFilter = false)
        {
            orderByType ??= OrderByType.Asc;
            if (isClearCreateByFilter)
            {
                return Table.ClearFilter<ICreateByEntity>().WhereIF(whereExpression != null, whereExpression)
                    .OrderByIF(orderExpression != null, orderExpression, (OrderByType)orderByType);
            }

            return Table.WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(orderExpression != null, orderExpression, (OrderByType)orderByType);
        }

        #endregion
    }
}
