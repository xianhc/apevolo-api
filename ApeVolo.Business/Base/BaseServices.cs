using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Common.DI;
using ApeVolo.Common.Extention;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Base;
using ApeVolo.IRepository.Base;
using AutoMapper;

namespace ApeVolo.Business.Base
{
    /// <summary>
    /// 业务实现基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseServices<TEntity> : IDependencyService, IBaseServices<TEntity> where TEntity : class, new()
    {
        #region 字段

        /// <summary>
        /// 仓储DAL
        /// </summary>
        protected ISugarHandler<TEntity> _baseDal;

        /// <summary>
        /// 当前用户
        /// </summary>
        protected ICurrentUser _currentUser;

        /// <summary>
        /// mapper
        /// </summary>
        protected IMapper _mapper;

        #endregion

        #region 通用方法

        public async Task<bool> AddEntityAsync(TEntity entity)
        {
            entity.InitEntity();
            return await _baseDal.AddReturnBoolAsync(entity);
        }

        public async Task<bool> AddEntityListAsync(List<TEntity> entityList)
        {
            entityList.ForEach(u => { u.InitEntity(); });
            return await _baseDal.AddReturnBoolAsync(entityList);
        }

        public async Task<bool> UpdateEntityAsync(TEntity entity, List<string> lstIgnoreColumns = null,
            bool isLock = true)
        {
            entity.EditEntity();
            return await _baseDal.UpdateAsync(entity, lstIgnoreColumns, isLock) > 0;
        }

        public async Task<bool> UpdateEntityListAsync(List<TEntity> entityList)
        {
            entityList.ForEach(u => { u.EditEntity(); });
            return await _baseDal.UpdateAsync(entityList) > 0;
        }


        public async Task<bool> DeleteEntityAsync(TEntity entity)
        {
            entity.DelEntity();
            return await _baseDal.UpdateAsync(entity) > 0;
        }

        public async Task<bool> DeleteEntityListAsync(List<TEntity> entityList)
        {
            entityList.ForEach(u => { u.DelEntity(); });
            return await _baseDal.UpdateAsync(entityList) > 0;
        }

        public async Task<TEntity> QuerySingleAsync(object objId, string columnName = "id")
        {
            return await _baseDal.QuerySingleAsync(objId, columnName);
        }


        public async Task<List<TEntity>> QueryByIdsAsync(List<long> ids, string columnName = "id")
        {
            return await _baseDal.QueryListInAsync(ids, columnName);
        }

        public async Task<List<TEntity>> QueryByIdsAsync(HashSet<long> ids, string columnName = "id")
        {
            return await QueryByIdsAsync(ids.ToList(), columnName);
        }

        public async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> whereLambda)
        {
            return await _baseDal.IsExistAsync(whereLambda);
        }

        public async Task<TEntity> QueryFirstAsync(Expression<Func<TEntity, bool>> whereLambda = null)
        {
            return await _baseDal.QueryFirstAsync(whereLambda);
        }

        #endregion
    }
}