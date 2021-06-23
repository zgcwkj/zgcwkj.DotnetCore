using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using zgcwkj.Util.Common;
using zgcwkj.Util.Extension;

namespace zgcwkj.Util.DbUtil.MySql
{
    /// <summary>
    /// MySQL数据库
    /// </summary>
    public class MySqlDatabase : IDatabase
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public MySqlDatabase()
        {
            MyDbContext = new MySqlDbContext();
        }

        /// <summary>
        /// 数据访问对象
        /// </summary>
        public DbContext MyDbContext { get; set; }

        /// <summary>
        /// 事务对象
        /// </summary>
        public IDbContextTransaction MyDbAffairs { get; set; }

        /// <summary>
        /// 事务开始
        /// </summary>
        /// <returns></returns>
        public IDatabase BeginTrans()
        {
            DbConnection dbConnection = MyDbContext.Database.GetDbConnection();
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            MyDbAffairs = MyDbContext.Database.BeginTransaction();
            return this;
        }

        /// <summary>
        /// 事务开始
        /// </summary>
        /// <returns></returns>
        public async Task<IDatabase> BeginTransAsync()
        {
            DbConnection dbConnection = MyDbContext.Database.GetDbConnection();
            if (dbConnection.State == ConnectionState.Closed)
            {
                await dbConnection.OpenAsync();
            }
            MyDbAffairs = await MyDbContext.Database.BeginTransactionAsync();
            return this;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public int CommitTrans()
        {
            try
            {
                DbContextExtension.SetEntityDefaultValue(MyDbContext);

                int returnValue = MyDbContext.SaveChanges();
                if (MyDbAffairs != null)
                {
                    MyDbAffairs.Commit();
                    this.Close();
                }
                else
                {
                    this.Close();
                }
                return returnValue;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (MyDbAffairs == null)
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public async Task<int> CommitTransAsync()
        {
            try
            {
                DbContextExtension.SetEntityDefaultValue(MyDbContext);

                int returnValue = await MyDbContext.SaveChangesAsync();
                if (MyDbAffairs != null)
                {
                    await MyDbAffairs.CommitAsync();
                    await this.CloseAsync();
                }
                else
                {
                    await this.CloseAsync();
                }
                return returnValue;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (MyDbAffairs == null)
                {
                    await this.CloseAsync();
                }
            }
        }

        /// <summary>
        /// 把当前操作回滚成未提交状态
        /// </summary>
        public void RollbackTrans()
        {
            this.MyDbAffairs.Rollback();
            this.MyDbAffairs.Dispose();
            this.Close();
        }

        /// <summary>
        /// 把当前操作回滚成未提交状态
        /// </summary>
        public async Task RollbackTransAsync()
        {
            await this.MyDbAffairs.RollbackAsync();
            await this.MyDbAffairs.DisposeAsync();
            await this.CloseAsync();
        }

        /// <summary>
        /// 关闭连接，内存回收
        /// </summary>
        public void Close()
        {
            MyDbContext.Dispose();
        }

        /// <summary>
        /// 关闭连接，内存回收
        /// </summary>
        public async Task CloseAsync()
        {
            await MyDbContext.DisposeAsync();
        }

        //==> 脚本数据

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>影响行数</returns>
        public int ExecuteSqlRaw(string strSql)
        {
            var dbCommon = this.MyDbContext;
            if (MyDbAffairs == null)
            {
                return dbCommon.Database.ExecuteSqlRaw(strSql);
            }
            else
            {
                dbCommon.Database.ExecuteSqlRaw(strSql);
                return MyDbAffairs == null ? this.CommitTrans() : 0;
            }
        }

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>影响行数</returns>
        public async Task<int> ExecuteSqlRawAsync(string strSql)
        {
            var dbCommon = this.MyDbContext;
            if (MyDbAffairs == null)
            {
                return await dbCommon.Database.ExecuteSqlRawAsync(strSql);
            }
            else
            {
                await dbCommon.Database.ExecuteSqlRawAsync(strSql);
                return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
            }
        }

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>影响行数</returns>
        public int ExecuteSqlRaw(string strSql, params DbParameter[] dbParameter)
        {
            var dbCommon = this.MyDbContext;
            if (MyDbAffairs == null)
            {
                return dbCommon.Database.ExecuteSqlRaw(strSql, dbParameter);
            }
            else
            {
                dbCommon.Database.ExecuteSqlRaw(strSql, dbParameter);
                return MyDbAffairs == null ? this.CommitTrans() : 0;
            }
        }

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>影响行数</returns>
        public async Task<int> ExecuteSqlRawAsync(string strSql, params DbParameter[] dbParameter)
        {
            var dbCommon = this.MyDbContext;
            if (MyDbAffairs == null)
            {
                return await dbCommon.Database.ExecuteSqlRawAsync(strSql, dbParameter);
            }
            else
            {
                await dbCommon.Database.ExecuteSqlRawAsync(strSql, dbParameter);
                return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">语句</param>
        /// <returns>影响行数</returns>
        public int ExecuteProcRaw(string procName)
        {
            var dbCommon = this.MyDbContext;
            if (MyDbAffairs == null)
            {
                return dbCommon.Database.ExecuteSqlRaw(DbContextExtension.BuilderProc(procName));
            }
            else
            {
                dbCommon.Database.ExecuteSqlRaw(DbContextExtension.BuilderProc(procName));
                return MyDbAffairs == null ? this.CommitTrans() : 0;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">语句</param>
        /// <returns>影响行数</returns>
        public async Task<int> ExecuteProcRawAsync(string procName)
        {
            var dbCommon = this.MyDbContext;
            if (MyDbAffairs == null)
            {
                return await dbCommon.Database.ExecuteSqlRawAsync(DbContextExtension.BuilderProc(procName));
            }
            else
            {
                await dbCommon.Database.ExecuteSqlRawAsync(DbContextExtension.BuilderProc(procName));
                return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">过程名称</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>影响行数</returns>
        public int ExecuteProcRaw(string procName, params DbParameter[] dbParameter)
        {
            var dbCommon = this.MyDbContext;
            if (MyDbAffairs == null)
            {
                return dbCommon.Database.ExecuteSqlRaw(DbContextExtension.BuilderProc(procName, dbParameter), dbParameter);
            }
            else
            {
                dbCommon.Database.ExecuteSqlRaw(DbContextExtension.BuilderProc(procName, dbParameter), dbParameter);
                return MyDbAffairs == null ? this.CommitTrans() : 0;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">过程名称</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>影响行数</returns>
        public async Task<int> ExecuteProcRawAsync(string procName, params DbParameter[] dbParameter)
        {
            var dbCommon = this.MyDbContext;
            if (MyDbAffairs == null)
            {
                return await dbCommon.Database.ExecuteSqlRawAsync(DbContextExtension.BuilderProc(procName, dbParameter), dbParameter);
            }
            else
            {
                await dbCommon.Database.ExecuteSqlRawAsync(DbContextExtension.BuilderProc(procName, dbParameter), dbParameter);
                return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
            }
        }

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>表数据</returns>
        public DataTable FindTable(string strSql)
        {
            return FindTable(strSql, null);
        }

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>表数据</returns>
        public async Task<DataTable> FindTableAsync(string strSql)
        {
            return await FindTableAsync(strSql, null);
        }

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>表数据</returns>
        public DataTable FindTable(string strSql, params DbParameter[] dbParameter)
        {
            var dbCommon = this.MyDbContext;
            using var dbConnection = dbCommon.Database.GetDbConnection();
            DbScalarExtension dbScalar = new DbScalarExtension(dbCommon, dbConnection);
            var reader = dbScalar.ExecuteReade(CommandType.Text, strSql, dbParameter);
            return DbExtension.IDataReaderToDataTable(reader);
        }

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>表数据</returns>
        public async Task<DataTable> FindTableAsync(string strSql, params DbParameter[] dbParameter)
        {
            var dbCommon = this.MyDbContext;
            using var dbConnection = dbCommon.Database.GetDbConnection();
            DbScalarExtension dbScalar = new DbScalarExtension(dbCommon, dbConnection);
            var reader = await dbScalar.ExecuteReadeAsync(CommandType.Text, strSql, dbParameter);
            return DbExtension.IDataReaderToDataTable(reader);
        }

        /// <summary>
        /// 查询首行首列
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>首行首列</returns>
        public object FindObject(string strSql)
        {
            return FindObject(strSql, null);
        }

        /// <summary>
        /// 查询首行首列
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <returns>首行首列</returns>
        public async Task<object> FindObjectAsync(string strSql)
        {
            return await FindObjectAsync(strSql, null);
        }

        /// <summary>
        /// 查询首行首列
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>首行首列</returns>
        public object FindObject(string strSql, params DbParameter[] dbParameter)
        {
            var dbCommon = this.MyDbContext;
            using var dbConnection = dbCommon.Database.GetDbConnection();
            DbScalarExtension dbScalar = new DbScalarExtension(dbCommon, dbConnection);
            return dbScalar.ExecuteScalar(CommandType.Text, strSql, dbParameter);
        }

        /// <summary>
        /// 查询首行首列
        /// </summary>
        /// <param name="strSql">语句</param>
        /// <param name="dbParameter">参数</param>
        /// <returns>首行首列</returns>
        public async Task<object> FindObjectAsync(string strSql, params DbParameter[] dbParameter)
        {
            var dbCommon = this.MyDbContext;
            using var dbConnection = dbCommon.Database.GetDbConnection();
            DbScalarExtension dbScalar = new DbScalarExtension(dbCommon, dbConnection);
            return await dbScalar.ExecuteScalarAsync(CommandType.Text, strSql, dbParameter);
        }

        //==> 实体数据

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体数据</returns>
        public IQueryable<T> QuerTable<T>() where T : class, new()
        {
            var dbCommon = this.MyDbContext;
            return dbCommon.Set<T>();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>实体数据</returns>
        public IQueryable<T> QuerTable<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            var dbCommon = this.MyDbContext;
            return dbCommon.Set<T>().Where(condition);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响行数</returns>
        public int Insert<T>(T entity) where T : class
        {
            var dbCommon = this.MyDbContext;
            dbCommon.Entry(entity).State = EntityState.Added;
            return MyDbAffairs == null ? this.CommitTrans() : 0;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响行数</returns>
        public async Task<int> InsertAsync<T>(T entity) where T : class
        {
            var dbCommon = this.MyDbContext;
            dbCommon.Entry(entity).State = EntityState.Added;
            return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>影响行数</returns>
        public int Insert<T>(IEnumerable<T> entities) where T : class
        {
            var dbCommon = this.MyDbContext;
            foreach (var entity in entities)
            {
                dbCommon.Entry(entity).State = EntityState.Added;
            }
            return MyDbAffairs == null ? this.CommitTrans() : 0;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>影响行数</returns>
        public async Task<int> InsertAsync<T>(IEnumerable<T> entities) where T : class
        {
            var dbCommon = this.MyDbContext;
            foreach (var entity in entities)
            {
                dbCommon.Entry(entity).State = EntityState.Added;
            }
            return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>删除数量</returns>
        public int Delete<T>() where T : class
        {
            var dbCommon = this.MyDbContext;
            IEntityType entityType = DbContextExtension.GetEntityType<T>(dbCommon);
            if (entityType != null)
            {
                string tableName = entityType.GetTableName();
                return this.ExecuteSqlRaw(DbContextExtension.DeleteSql(tableName));
            }
            return -1;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>删除数量</returns>
        public async Task<int> DeleteAsync<T>() where T : class
        {
            var dbCommon = this.MyDbContext;
            IEntityType entityType = DbContextExtension.GetEntityType<T>(dbCommon);
            if (entityType != null)
            {
                string tableName = entityType.GetTableName();
                return await this.ExecuteSqlRawAsync(DbContextExtension.DeleteSql(tableName));
            }
            return -1;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>删除数量</returns>
        public int Delete<T>(T entity) where T : class
        {
            var dbCommon = this.MyDbContext;
            dbCommon.Set<T>().Attach(entity);
            dbCommon.Set<T>().Remove(entity);
            return MyDbAffairs == null ? this.CommitTrans() : 0;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>删除数量</returns>
        public async Task<int> DeleteAsync<T>(T entity) where T : class
        {
            var dbCommon = this.MyDbContext;
            dbCommon.Set<T>().Attach(entity);
            dbCommon.Set<T>().Remove(entity);
            return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>删除数量</returns>
        public int Delete<T>(IEnumerable<T> entities) where T : class
        {
            var dbCommon = this.MyDbContext;
            foreach (var entity in entities)
            {
                dbCommon.Set<T>().Attach(entity);
                dbCommon.Set<T>().Remove(entity);
            }
            return MyDbAffairs == null ? this.CommitTrans() : 0;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>删除数量</returns>
        public async Task<int> DeleteAsync<T>(IEnumerable<T> entities) where T : class
        {
            var dbCommon = this.MyDbContext;
            foreach (var entity in entities)
            {
                dbCommon.Set<T>().Attach(entity);
                dbCommon.Set<T>().Remove(entity);
            }
            return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>删除数量</returns>
        public int Delete<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            var dbCommon = this.MyDbContext;
            IEnumerable<T> entities = dbCommon.Set<T>().Where(condition).ToList();
            return entities.Count() > 0 ? Delete(entities) : 0;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>删除数量</returns>
        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            var dbCommon = this.MyDbContext;
            IEnumerable<T> entities = await dbCommon.Set<T>().Where(condition).ToListAsync();
            return entities.Count() > 0 ? await DeleteAsync(entities) : 0;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns>删除数量</returns>
        public int Delete<T>(string propertyName, string propertyValue) where T : class
        {
            var dbCommon = this.MyDbContext;
            IEntityType entityType = DbContextExtension.GetEntityType<T>(dbCommon);
            if (entityType != null)
            {
                string tableName = entityType.GetTableName();
                return this.ExecuteSqlRaw(DbContextExtension.DeleteSql(tableName, propertyName, propertyValue));
            }
            return -1;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns>删除数量</returns>
        public async Task<int> DeleteAsync<T>(string propertyName, string propertyValue) where T : class
        {
            var dbCommon = this.MyDbContext;
            IEntityType entityType = DbContextExtension.GetEntityType<T>(dbCommon);
            if (entityType != null)
            {
                string tableName = entityType.GetTableName();
                return await this.ExecuteSqlRawAsync(DbContextExtension.DeleteSql(tableName, propertyName, propertyValue));
            }
            return -1;
        }

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>修改数量</returns>
        public int Update<T>(T entity) where T : class
        {
            var dbCommon = this.MyDbContext;
            dbCommon.Set<T>().Attach(entity);
            dbCommon.Entry(entity).State = EntityState.Modified;
            return MyDbAffairs == null ? this.CommitTrans() : 0;
        }

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>修改数量</returns>
        public async Task<int> UpdateAsync<T>(T entity) where T : class
        {
            var dbCommon = this.MyDbContext;
            dbCommon.Set<T>().Attach(entity);
            dbCommon.Entry(entity).State = EntityState.Modified;
            return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
        }

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>修改数量</returns>
        public int Update<T>(IEnumerable<T> entities) where T : class
        {
            var dbCommon = this.MyDbContext;
            foreach (var entity in entities)
            {
                dbCommon.Entry<T>(entity).State = EntityState.Modified;
            }
            return MyDbAffairs == null ? this.CommitTrans() : 0;
        }

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">多个实体对象</param>
        /// <returns>修改数量</returns>
        public async Task<int> UpdateAsync<T>(IEnumerable<T> entities) where T : class
        {
            var dbCommon = this.MyDbContext;
            foreach (var entity in entities)
            {
                dbCommon.Entry<T>(entity).State = EntityState.Modified;
            }
            return MyDbAffairs == null ? await this.CommitTransAsync() : 0;
        }

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>修改数量</returns>
        public int Update<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            var dbCommon = this.MyDbContext;
            IEnumerable<T> entities = dbCommon.Set<T>().Where(condition).ToList();
            return entities.Count() > 0 ? Update(entities) : 0;
        }

        /// <summary>
        /// 修改表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">Linq</param>
        /// <returns>修改数量</returns>
        public async Task<int> UpdateAsync<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            var dbCommon = this.MyDbContext;
            IEnumerable<T> entities = await dbCommon.Set<T>().Where(condition).ToListAsync();
            return entities.Count() > 0 ? await UpdateAsync(entities) : 0;
        }
    }
}
