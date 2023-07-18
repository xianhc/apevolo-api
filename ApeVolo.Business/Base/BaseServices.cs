using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Base;
using ApeVolo.Repository.SugarHandler;
using SqlSugar;

namespace ApeVolo.Business.Base
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

        /// <summary>
        /// 当前上下文
        /// </summary>
        public ApeContext ApeContext { get; }

        #endregion

        #region 构造函数

        public BaseServices(ApeContext apeContext = null, ISugarRepository<TEntity> sugarRepository = null)
        {
            ApeContext = apeContext;
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

        public ISugarQueryable<TEntity> TableWhere(Expression<Func<TEntity, bool>> whereExpression = null,
            Expression<Func<TEntity, object>> orderExpression = null, OrderByType orderByType = OrderByType.Desc)
        {
            return Table.WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(orderExpression != null, orderExpression, orderByType);
        }

        #endregion
    }
}
