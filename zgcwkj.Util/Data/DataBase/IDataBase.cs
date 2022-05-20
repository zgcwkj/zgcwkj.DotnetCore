using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace zgcwkj.Util.Data.DataBase
{
    /// <summary>
    /// 数据库接口抽象
    /// </summary>
    public interface IDataBase
    {
        /// <summary>
        /// 数据访问对象
        /// </summary>
        DbContext MyDbContext { get; set; }

        /// <summary>
        /// 事务对象
        /// </summary>
        IDbContextTransaction MyDbAffairs { get; set; }

        /// <summary>
        /// 开始事务开始
        /// </summary>
        /// <returns></returns>
        IDataBase BeginTrans();

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        Task<IDataBase> BeginTransAsync();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTrans();

        /// <summary>
        /// 回滚事务
        /// </summary>
        Task RollbackTransAsync();

        /// <summary>
        /// 提交事务
        /// </summary>
        int CommitTrans();

        /// <summary>
        /// 提交事务
        /// </summary>
        Task<int> CommitTransAsync();

        /// <summary>
        /// 关闭连接，内存回收
        /// </summary>
        void Close();

        /// <summary>
        /// 关闭连接，内存回收
        /// </summary>
        Task CloseAsync();

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>影响行数</returns>
        int ExecuteSqlRaw(string strSql);

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>影响行数</returns>
        Task<int> ExecuteSqlRawAsync(string strSql);

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>影响行数</returns>
        int ExecuteSqlRaw(string strSql, params DbParameter[] dbParameter);

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>影响行数</returns>
        Task<int> ExecuteSqlRawAsync(string strSql, params DbParameter[] dbParameter);

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>表数据</returns>
        DataTable FindTable(string strSql);

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>表数据</returns>
        Task<DataTable> FindTableAsync(string strSql);

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>表数据</returns>
        DataTable FindTable(string strSql, DbParameter[] dbParameter);

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>表数据</returns>
        Task<DataTable> FindTableAsync(string strSql, DbParameter[] dbParameter);

        /// <summary>
        /// 查询首行首列
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>首行首列</returns>
        object FindObject(string strSql);

        /// <summary>
        /// 查询首行首列
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>首行首列</returns>
        Task<object> FindObjectAsync(string strSql);

        /// <summary>
        /// 查询首行首列
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>首行首列</returns>
        object FindObject(string strSql, DbParameter[] dbParameter);

        /// <summary>
        /// 查询首行首列
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>首行首列</returns>
        Task<object> FindObjectAsync(string strSql, DbParameter[] dbParameter);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体数据</returns>
        IQueryable<T> QuerTable<T>() where T : class, new();

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>实体数据</returns>
        IQueryable<T> QuerTable<T>(Expression<Func<T, bool>> condition) where T : class, new();

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响行数</returns>
        int Insert<T>(T entity) where T : class;

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响行数</returns>
        Task<int> InsertAsync<T>(T entity) where T : class;

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>影响行数</returns>
        int Insert<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>影响行数</returns>
        Task<int> InsertAsync<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>删除数量</returns>
        int Delete<T>(T entity) where T : class;

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>删除数量</returns>
        Task<int> DeleteAsync<T>(T entity) where T : class;

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>删除数量</returns>
        int Delete<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>删除数量</returns>
        Task<int> DeleteAsync<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>删除数量</returns>
        int Delete<T>(Expression<Func<T, bool>> condition) where T : class, new();

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>删除数量</returns>
        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> condition) where T : class, new();

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>修改数量</returns>
        int Update<T>(T entity) where T : class;

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>修改数量</returns>
        Task<int> UpdateAsync<T>(T entity) where T : class;

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>修改数量</returns>
        int Update<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>修改数量</returns>
        Task<int> UpdateAsync<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>修改数量</returns>
        int Update<T>(Expression<Func<T, bool>> condition) where T : class, new();

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>修改数量</returns>
        Task<int> UpdateAsync<T>(Expression<Func<T, bool>> condition) where T : class, new();
    }
}
