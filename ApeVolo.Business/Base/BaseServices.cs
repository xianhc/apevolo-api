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

namespace ApeVolo.Business.Base;

/// <summary>
/// 业务实现基类
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class BaseServices<TEntity> : IDependencyService, IBaseServices<TEntity> where TEntity : class, new()
{
    /// <summary>
    /// 仓储DAL
    /// </summary>
    protected ISugarHandler<TEntity> BaseDal;

    /// <summary>
    /// 当前用户
    /// </summary>
    protected ICurrentUser CurrentUser;

    /// <summary>
    /// mapper
    /// </summary>
    protected IMapper Mapper;

    #region 字段

    #endregion

    #region 通用方法

    public async Task<bool> AddEntityAsync(TEntity entity)
    {
        entity.InitEntity(CurrentUser);
        return await BaseDal.AddReturnBoolAsync(entity);
    }

    public async Task<bool> AddEntityListAsync(List<TEntity> entityList)
    {
        entityList.ForEach(u => { u.InitEntity(CurrentUser); });
        return await BaseDal.AddReturnBoolAsync(entityList);
    }

    public async Task<bool> UpdateEntityAsync(TEntity entity, List<string> lstIgnoreColumns = null,
        bool isLock = true)
    {
        entity.EditEntity(CurrentUser);
        return await BaseDal.UpdateAsync(entity, lstIgnoreColumns, isLock) > 0;
    }

    public async Task<bool> UpdateEntityListAsync(List<TEntity> entityList)
    {
        entityList.ForEach(u => { u.EditEntity(CurrentUser); });
        return await BaseDal.UpdateAsync(entityList) > 0;
    }


    public async Task<bool> DeleteEntityAsync(TEntity entity)
    {
        entity.DelEntity(CurrentUser);
        return await BaseDal.UpdateAsync(entity) > 0;
    }

    public async Task<bool> DeleteEntityListAsync(List<TEntity> entityList)
    {
        entityList.ForEach(u => { u.DelEntity(CurrentUser); });
        return await BaseDal.UpdateAsync(entityList) > 0;
    }

    public async Task<TEntity> QuerySingleAsync(object objId, string columnName = "id")
    {
        return await BaseDal.QuerySingleAsync(objId, columnName);
    }


    public async Task<List<TEntity>> QueryByIdsAsync(List<long> ids, string columnName = "id")
    {
        return await BaseDal.QueryListInAsync(ids, columnName);
    }

    public async Task<List<TEntity>> QueryByIdsAsync(HashSet<long> ids, string columnName = "id")
    {
        return await QueryByIdsAsync(ids.ToList(), columnName);
    }

    public async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> whereLambda)
    {
        return await BaseDal.IsExistAsync(whereLambda);
    }

    public async Task<TEntity> QueryFirstAsync(Expression<Func<TEntity, bool>> whereLambda = null)
    {
        return await BaseDal.QueryFirstAsync(whereLambda);
    }

    #endregion
}