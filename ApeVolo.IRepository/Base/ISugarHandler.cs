using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using SqlSugar;

namespace ApeVolo.IRepository.Base;

/// <summary>
/// sqlSugar接口
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface ISugarHandler<TEntity> where TEntity : class
{
    #region 新增操作

    /// <summary>
    /// 新增实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响行数</returns>
    Task<int> AddAsync(TEntity entity);

    /// <summary>
    /// 批量新增实体
    /// </summary>
    /// <param name="entitys">实体集合</param>
    /// <returns>受影响行数</returns>
    Task<int> AddAsync(List<TEntity> entitys);

    /// <summary>
    /// 新增实体
    /// </summary>
    /// <param name="keyValues">键：字段名称，值：字段值</param>
    /// <returns>受影响行数</returns>
    Task<int> AddAsync(Dictionary<string, object> keyValues);

    /// <summary>
    /// 新增实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>返回当前实体</returns>
    Task<TEntity> AddReturnEntityAsync(TEntity entity);

    /// <summary>
    /// 新增实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>自增ID</returns>
    Task<int> AddReturnIdentityAsync(TEntity entity);

    /// <summary>
    /// 新增实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>成功或失败</returns>
    Task<bool> AddReturnBoolAsync(TEntity entity);

    /// <summary>
    /// 批量新增实体
    /// </summary>
    /// <param name="entitys">实体集合</param>
    /// <returns>成功或失败</returns>
    Task<bool> AddReturnBoolAsync(List<TEntity> entitys);

    #endregion

    #region 更新操作

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <param name="lstIgnoreColumns">忽略列</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateAsync(TEntity entity, List<string> lstIgnoreColumns = null, bool isLock = true);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entitys">实体集合</param>
    /// <param name="lstIgnoreColumns">忽略列</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateAsync(List<TEntity> entitys, List<string> lstIgnoreColumns = null, bool isLock = true);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <param name="where">条件表达式</param>
    /// <param name="lstIgnoreColumns">忽略列</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> where,
        List<string> lstIgnoreColumns = null,
        bool isLock = true);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="update">实体对象</param>
    /// <param name="where">条件表达式</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateAsync(Expression<Func<TEntity, TEntity>> update, Expression<Func<TEntity, bool>> where = null,
        bool isLock = true);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="keyValues">键:字段名称 值：值</param>
    /// <param name="where">条件表达式</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateAsync(Dictionary<string, object> keyValues, Expression<Func<TEntity, bool>> where = null,
        bool isLock = true);

    /// <summary>
    /// 批量更新实体列
    /// </summary>
    /// <param name="entitys">实体集合</param>
    /// <param name="updateColumns">要更新的列</param>
    /// <param name="wherecolumns">条件列</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateColumnsAsync(List<TEntity> entitys, Expression<Func<TEntity, object>> updateColumns,
        Expression<Func<TEntity, object>> wherecolumns = null, bool isLock = true);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <param name="lstIgnoreColumns">忽略列</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateRowVerAsync(TEntity entity, List<string> lstIgnoreColumns = null, bool isLock = true);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="update">实体对象</param>
    /// <param name="where">键:字段名称 值:值</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateRowVerAsync(Expression<Func<TEntity, TEntity>> update, Dictionary<string, object> where,
        bool isLock = true);

    #endregion

    #region 删除操作

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="id">主键ID</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<bool> DeleteByPrimaryAsync(object id, bool isLock = true);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="primaryKeyValues">主键ID集合</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> DeleteByPrimaryAsync(List<object> primaryKeyValues, bool isLock = true);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> DeleteAsync(TEntity entity, bool isLock = true);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entitys">实体集合</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> DeleteAsync(List<TEntity> entitys, bool isLock = true);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="whereLambda">条件表达式</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereLambda, bool isLock = true);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="inValues">主键集合</param>
    /// <param name="isLock">是否加锁</param>
    /// <returns>受影响行数</returns>
    Task<int> DeleteInAsync(List<dynamic> inValues, bool isLock = true);

    #endregion

    #region 单表查询

    /// <summary>
    /// 查询单个
    /// </summary>
    /// <param name="expression">返回表达式</param>
    /// <param name="whereLambda">条件表达式</param>
    /// <typeparam name="TResult">返回对象</typeparam>
    /// <returns>自定义数据</returns>
    Task<TResult> QueryAsync<TResult>(Expression<Func<TEntity, TResult>> expression,
        Expression<Func<TEntity, bool>> whereLambda = null);

    /// <summary>
    /// 实体列表
    /// </summary>
    /// <param name="expression">返回表达式</param>
    /// <param name="whereLambda">条件表达式</param>
    /// <typeparam name="TResult">返回对象</typeparam>
    /// <returns>自定义数据</returns>
    Task<List<TResult>> QueryListExpAsync<TResult>(Expression<Func<TEntity, TResult>> expression,
        Expression<Func<TEntity, bool>> whereLambda = null);

    /// <summary>
    /// 查询单个
    /// </summary>
    /// <param name="whereLambda">条件表达式</param>
    /// <returns>实体对象</returns>
    Task<TEntity> QueryFirstAsync(Expression<Func<TEntity, bool>> whereLambda = null);

    /// <summary>
    /// 实体列表
    /// </summary>
    /// <param name="whereLambda">条件表达式</param>
    /// <param name="orderFileds"></param>
    /// <param name="orderByType"></param>
    /// <returns>实体列表</returns>
    Task<List<TEntity>> QueryListAsync(Expression<Func<TEntity, bool>> whereLambda = null,
        Expression<Func<TEntity, object>> orderFileds = null, OrderByType orderByType = OrderByType.Desc);


    /// <summary>
    /// 实体列表
    /// </summary>
    /// <param name="sql">SQL</param>
    /// <returns>实体列表</returns>
    Task<List<TEntity>> QuerySqlListAsync(string sql);

    /// <summary>
    /// 实体列表 分页查询
    /// </summary>
    /// <param name="whereLambda">条件表达式</param>
    /// <param name="pagination">分页对象</param>
    /// <param name="expression"></param>
    /// <returns></returns>
    Task<List<TEntity>> QueryPageListAsync(Expression<Func<TEntity, bool>> whereLambda, Pagination pagination,
        Expression<Func<TEntity, TEntity>> expression = null);

    /// <summary>
    /// 实体列表
    /// </summary>
    /// <param name="inFieldName">指定字段名</param>
    /// <param name="inValues">值</param>
    /// <returns>实体列表</returns>
    Task<List<TEntity>> QueryListInAsync(string inFieldName, List<dynamic> inValues);

    /// <summary>
    /// 查询单个对象
    /// </summary>
    /// <param name="key">列值</param>
    /// <param name="columnName">列名 默认ID</param>
    /// <returns>实体对象</returns>
    Task<TEntity> QuerySingleAsync(object key, string columnName = "id");

    /// <summary>
    /// 实体列表
    /// </summary>
    /// <param name="values">主键集合</param>
    /// <param name="columnName">列名 默认ID</param>
    /// <returns>实体列表</returns>
    Task<List<TEntity>> QueryListInAsync(List<long> values, string columnName = "id");

    /// <summary>
    /// DataTable数据源
    /// </summary>
    /// <param name="whereLambda">条件表达式</param>
    /// <returns>DataTable</returns>
    Task<DataTable> QueryDataTableAsync(Expression<Func<TEntity, bool>> whereLambda = null);

    /// <summary>
    /// DataTable数据源
    /// </summary>
    /// <param name="sql">SQL</param>
    /// <returns>DataTable</returns>
    Task<DataTable> QueryDataTableAsync(string sql);

    /// <summary>
    /// Object
    /// </summary> 
    /// <param name="sql">SQL</param> 
    /// <returns>Object</returns>
    Task<object> QuerySqlScalarAsync(string sql);

    /// <summary>
    /// 查询单个对象
    /// </summary>
    /// <param name="whereLambda">条件表达式</param>
    /// <returns>对象.json</returns>
    Task<string> QueryJsonAsync(Expression<Func<TEntity, bool>> whereLambda = null);

    #endregion

    #region 多表联查 最大支持16个表

    Task<List<TResult>> QueryMuchAsync<T, T2, TResult>(
        Expression<Func<T, T2, object[]>> joinExpression,
        Expression<Func<T, T2, TResult>> selectExpression,
        Expression<Func<T, T2, bool>> whereLambda = null, Expression<Func<T, T2, object>> groupExpression = null,
        string sortField = "");


    Task<List<TResult>> QueryMuchAsync<T, T2, T3, TResult>(
        Expression<Func<T, T2, T3, object[]>> joinExpression,
        Expression<Func<T, T2, T3, TResult>> selectExpression,
        Expression<Func<T, T2, T3, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, object>> groupExpression = null);


    Task<List<TResult>> QueryMuchAsync<T, T2, T3, T4, TResult>(
        Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, object>> groupExpression = null);


    Task<List<TResult>> QueryMuchAsync<T, T2, T3, T4, T5, TResult>(
        Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, object>> groupExpression = null);


    Task<List<TResult>> QueryMuchAsync<T, T2, T3, T4, T5, T6, TResult>(
        Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, T6, object>> groupExpression = null);


    Task<List<TResult>> QueryMuchAsync<T, T2, T3, T4, T5, T6, T7, TResult>(
        Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> groupExpression = null);


    Task<List<TResult>> QueryMuchAsync<T, T2, T3, T4, T5, T6, T7, T8, TResult>(
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> groupExpression = null);

    Task<List<TResult>> QueryMuchAsync<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> groupExpression = null);

    Task<List<TResult>> QueryMuchAsync<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> groupExpression = null);

    #endregion

    #region 多表联查分页  最大支持16个表

    Task<List<TResult>> QueryMuchPageAsync<T, T2, TResult>(
        Pagination pagination,
        Expression<Func<T, T2, object[]>> joinExpression,
        Expression<Func<T, T2, TResult>> selectExpression,
        Expression<Func<T, T2, bool>> whereLambda = null,
        Expression<Func<T, T2, object>> groupExpression = null) where T : class, new();

    Task<List<TResult>> QueryMuchPageAsync<T, T2, T3, TResult>(
        Pagination pagination,
        Expression<Func<T, T2, T3, object[]>> joinExpression,
        Expression<Func<T, T2, T3, TResult>> selectExpression,
        Expression<Func<T, T2, T3, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, object>> groupExpression = null) where T : class, new();

    Task<List<TResult>> QueryMuchPageAsync<T, T2, T3, T4, TResult>(
        Pagination pagination,
        Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, object>> groupExpression = null) where T : class, new();

    Task<List<TResult>> QueryMuchPageAsync<T, T2, T3, T4, T5, TResult>(
        Pagination pagination,
        Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, object>> groupExpression = null) where T : class, new();

    Task<List<TResult>> QueryMuchPageAsync<T, T2, T3, T4, T5, T6, TResult>(
        Pagination pagination,
        Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, T6, object>> groupExpression = null) where T : class, new();

    Task<List<TResult>> QueryMuchPageAsync<T, T2, T3, T4, T5, T6, T7, TResult>(
        Pagination pagination,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> groupExpression = null) where T : class, new();

    Task<List<TResult>> QueryMuchPageAsync<T, T2, T3, T4, T5, T6, T7, T8, TResult>(
        Pagination pagination,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> whereLambda = null,
        Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> groupExpression = null) where T : class, new();

    #endregion

    #region 一对一 一对多查询

    /// <summary>
    /// 一对一 一对多查询
    /// </summary>
    /// <param name="mapperAction">委托方法体</param>
    /// <param name="whereLambda">条件表达式</param>
    /// <returns>泛型对象集合</returns>
    Task<List<TEntity>> QueryMapperAsync(Action<TEntity> mapperAction,
        Expression<Func<TEntity, bool>> whereLambda = null);

    /// <summary>
    /// 一对一 一对多查询
    /// </summary>
    /// <param name="mapperAction">委托方法体</param>
    /// <param name="whereLambda">条件表达式</param>
    /// <param name="pagination"></param>
    /// <returns>泛型对象集合</returns>
    Task<List<TEntity>> QueryMapperPageListAsync(Action<TEntity> mapperAction,
        Expression<Func<TEntity, bool>> whereLambda, Pagination pagination);

    /// <summary>
    /// 一对一 一对多查询
    /// </summary>
    /// <param name="mapperField"></param>
    /// <param name="whereLambda">条件表达式</param>
    /// <param name="mapperObject"></param>
    /// <param name="pagination"></param>
    /// <returns>泛型对象集合</returns>
    Task<List<TEntity>> QueryMapperPageListAsync<TObject>(Expression<Func<TEntity, List<TObject>>> mapperObject,
        Expression<Func<TEntity, object>> mapperField,
        Expression<Func<TEntity, bool>> whereLambda, Pagination pagination);

    /// <summary>
    /// 一对一 一对多查询
    /// </summary>
    /// <param name="mapperAction">委托方法体</param>
    /// <param name="whereLambda">条件表达式</param>
    /// <param name="sortField"></param>
    /// <returns>泛型对象集合</returns>
    Task<List<TEntity>> QueryMapperAsync(Action<TEntity, MapperCache<TEntity>> mapperAction,
        Expression<Func<TEntity, bool>> whereLambda, string sortField = "");

    /// <summary>
    /// 一对一 一对多查询
    /// </summary>
    /// <param name="mapperAction">委托方法体</param>
    /// <param name="whereLambda">条件表达式</param>
    /// <param name="pagination">分页对象</param>
    /// <returns>泛型对象集合</returns>
    Task<List<TEntity>> QueryMapperPageListAsync(Action<TEntity, MapperCache<TEntity>> mapperAction,
        Expression<Func<TEntity, bool>> whereLambda, Pagination pagination);

    #endregion

    #region 存储过程

    /// <summary>
    /// 执行存储过程DataSet
    /// </summary>
    /// <param name="procedureName">存储过程名称</param>
    /// <param name="parameters">参数集合</param>
    /// <returns>DataSet</returns>
    Task<DataSet> QueryProcedureDataSetAsync(string procedureName, List<SqlParameter> parameters);

    /// <summary>
    /// 执行存储过程DataTable
    /// </summary>
    /// <param name="procedureName">存储过程名称</param>
    /// <param name="parameters">参数集合</param>
    /// <returns>DataTable</returns>
    Task<DataTable> QueryProcedureAsync(string procedureName, List<SqlParameter> parameters);

    /// <summary>
    /// 执行存储过程Object
    /// </summary>
    /// <param name="procedureName">存储过程名称</param>
    /// <param name="parameters">参数集合</param>
    /// <returns>Object</returns>
    Task<object> QueryProcedureScalarAsync(string procedureName, List<SqlParameter> parameters);

    #endregion

    #region 常用函数

    /// <summary>
    /// 查询前面几条
    /// </summary>
    /// <param name="whereLambda">条件表达式</param>
    /// <param name="topNum">要多少条</param>
    /// <returns>泛型对象集合</returns>
    Task<List<TEntity>> TakeAsync(int topNum, Expression<Func<TEntity, bool>> whereLambda = null);

    /// <summary>
    /// 对象是否存在
    /// </summary>
    /// <param name="whereLambda">条件表达式</param>
    /// <returns>True or False</returns>
    Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> whereLambda = null);

    /// <summary>
    /// 总和
    /// </summary>
    /// <param name="field">字段名</param>
    /// <returns>总和</returns>
    Task<int> SumAsync(string field);

    /// <summary>
    /// 最大值
    /// </summary>
    /// <param name="field">字段名</param>
    /// <typeparam name="TResult">泛型结果</typeparam>
    /// <returns>最大值</returns>
    Task<TResult> MaxAsync<TResult>(string field);

    /// <summary>
    /// 最小值
    /// </summary>
    /// <param name="field">字段名</param>
    /// <typeparam name="TResult">泛型结果</typeparam>
    /// <returns>最小值</returns>
    Task<TResult> MinAsync<TResult>(string field);

    /// <summary>
    /// 平均值
    /// </summary>
    /// <param name="field">字段名</param>
    /// <returns>平均值</returns>
    Task<int> AvgAsync(string field);

    #endregion

    #region 流水号

    /// <summary>
    /// 生成流水号
    /// </summary>
    /// <param name="key">列名</param>
    /// <param name="prefix">前缀</param>
    /// <param name="fixedLength">流水号长度</param>
    /// <param name="dateFomart">日期格式(yyyyMMdd) 为空前缀后不加日期,反之加</param>
    /// <returns></returns>
    Task<string> CustomNumberAsync(string key, string prefix = "", int fixedLength = 4, string dateFomart = "");

    /// <summary>
    /// 生成流水号
    /// </summary>
    /// <param name="key">列名</param>
    /// <param name="num">数量</param>
    /// <param name="prefix">前缀</param>
    /// <param name="fixedLength">流水号长度</param>
    /// <param name="dateFomart">日期格式(yyyyMMdd) 为空前缀后不加日期,反之加</param>
    /// <returns></returns>
    Task<List<string>> CustomNumberAsync(string key, int num, string prefix = "", int fixedLength = 4,
        string dateFomart = "");

    #endregion
}