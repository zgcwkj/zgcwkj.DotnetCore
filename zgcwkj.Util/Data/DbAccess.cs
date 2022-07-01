using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using zgcwkj.Util.Data;
using zgcwkj.Util.Data.Extension;

namespace zgcwkj.Util
{
    /// <summary>
    /// <b>数据库操作对象</b>
    /// 
    /// <para>常规使用：using var cmd = DbProvider.Create()</para>
    /// <para>注入使用：services.AddTransient&lt;DbAccess&gt;()</para>
    /// <para>建议使用<b>EF</b>操作数据库</para>
    /// </summary>
    public class DbAccess : IDisposable
    {
        /// <summary>
        /// SQL实体
        /// </summary>
        internal SqlModel dbModel { get; set; }

        /// <summary>
        /// 数据操作对象
        /// </summary>
        internal DbCommon dbCommon { get; set; }

        /// <summary>
        /// 事务对象
        /// </summary>
        internal IDbContextTransaction dbTrans { get; set; }

        /// <summary>
        /// 实例对象时
        /// </summary>
        public DbAccess()
        {
            dbModel = new SqlModel();
            dbCommon = new DbCommon();
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            dbCommon?.Dispose();
            dbTrans?.Dispose();
        }

        /// <summary>
        /// 查询数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>数据</returns>
        [Obsolete]
        public static DataTable QueryDataTable(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            return cmdAccess.GetData(sqlStr);
        }

        /// <summary>
        /// 查询数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>数据</returns>
        [Obsolete]
        public static async Task<DataTable> QueryDataTableAsync(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            return await cmdAccess.GetDataAsync(sqlStr);
        }

        /// <summary>
        /// 查询数据库的第一行数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>行数据</returns>
        [Obsolete]
        public static DataRow QueryDataRow(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit = strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            DataTable dataTable = cmdAccess.GetData(strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 查询数据库的第一行数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>行数据</returns>
        [Obsolete]
        public static async Task<DataRow> QueryDataRowAsync(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit = strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            DataTable dataTable = await cmdAccess.GetDataAsync(strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 查询数据库的首行首列数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>首行首列</returns>
        [Obsolete]
        public static object QueryScalar(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit = strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            DataTable dataTable = cmdAccess.GetData(strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0];
            }
            return null;
        }

        /// <summary>
        /// 查询数据库的首行首列数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>首行首列</returns>
        [Obsolete]
        public static async Task<object> QueryScalarAsync(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit= strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            DataTable dataTable = await cmdAccess.GetDataAsync(strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0];
            }
            return null;
        }

        /// <summary>
        /// 查询数据库的行数
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>行数</returns>
        [Obsolete]
        public static int QueryRowCount(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strCount = strFrom.RemoveEnter().ToLower();
            strCount = Regex.Match(strCount, @".+select.+count").Value;
            if (strCount.IsNull()) strFrom = $"select count(0) as counts from ({sqlStr}) tables";
            DataTable dataTable = cmdAccess.GetData(strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0].ToInt();
            }
            return 0;
        }

        /// <summary>
        /// 查询数据库的行数
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>行数</returns>
        [Obsolete]
        public static async Task<int> QueryRowCountAsync(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strCount = strFrom.RemoveEnter().ToLower();
            strCount = Regex.Match(strCount, @".+select.+count").Value;
            if (strCount.IsNull()) strFrom = $"select count(0) as counts from ({sqlStr}) tables";
            DataTable dataTable = await cmdAccess.GetDataAsync(strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0].ToInt();
            }
            return 0;
        }

        /// <summary>
        /// 修改数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>影响行数</returns>
        [Obsolete]
        public static int UpdateData(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            int updateCount = cmdAccess.SetData(sqlStr);
            return updateCount;
        }

        /// <summary>
        /// 修改数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>影响行数</returns>
        [Obsolete]
        public static async Task<int> UpdateDataAsync(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            int updateCount = await cmdAccess.SetDataAsync(sqlStr);
            return updateCount;
        }
    }
}
