﻿using System;
using System.Data;
using System.Threading.Tasks;
using zgcwkj.Util.Data;
using zgcwkj.Util.Interface;

namespace zgcwkj.Util
{
    /// <summary>
    /// <b>数据库操作对象</b>
    /// 
    /// <para>常规使用：DbProvider.Create()</para>
    /// <para>注入使用：services.AddTransient&lt;DbAccess&gt;()</para>
    /// <para>建议使用<b>EF</b>操作数据库，打代码更爽！by zgcwkj</para>
    /// </summary>
    public class DbAccess
    {
        /// <summary>
        /// SQL实体
        /// </summary>
        internal SqlModel dbModel;

        /// <summary>
        /// 数据操作对象
        /// </summary>
        internal IDataBase dataBase;

        /// <summary>
        /// 实例对象时
        /// </summary>
        public DbAccess()
        {
            dbModel = new SqlModel();
            dataBase = DataFactory.Db;
        }

        /// <summary>
        /// 查询数据库数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>数据</returns>
        [Obsolete]
        public static DataTable QueryDataTable(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            return GetData(cmdAccess, sqlStr);
        }

        /// <summary>
        /// 查询数据库数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>数据</returns>
        [Obsolete]
        public static async Task<DataTable> QueryDataTableAsync(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            return await GetDataAsync(cmdAccess, sqlStr);
        }

        /// <summary>
        /// 查询数据库的第一行数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>行数据</returns>
        [Obsolete]
        public static DataRow QueryDataRow(DbAccess cmdAccess)
        {
            if (!cmdAccess.dbModel.Sql.ToLower().Contains("limit"))
            {
                cmdAccess.dbModel.EndSql = "limit 1";
            }
            string sqlStr = cmdAccess.GetSql();
            DataTable dataTable = GetData(cmdAccess, sqlStr);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 查询数据库的第一行数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>行数据</returns>
        [Obsolete]
        public static async Task<DataRow> QueryDataRowAsync(DbAccess cmdAccess)
        {
            if (!cmdAccess.dbModel.Sql.ToLower().Contains("limit"))
            {
                cmdAccess.dbModel.EndSql = "limit 1";
            }
            string sqlStr = cmdAccess.GetSql();
            DataTable dataTable = await GetDataAsync(cmdAccess, sqlStr);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 查询数据库的首行首列数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>首行首列</returns>
        [Obsolete]
        public static object QueryScalar(DbAccess cmdAccess)
        {
            if (!cmdAccess.dbModel.Sql.ToLower().Contains("limit"))
            {
                cmdAccess.dbModel.EndSql = "limit 1";
            }
            string sqlStr = cmdAccess.GetSql();
            DataTable dataTable = GetData(cmdAccess, sqlStr);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0];
            }
            return null;
        }

        /// <summary>
        /// 查询数据库的首行首列数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>首行首列</returns>
        [Obsolete]
        public static async Task<object> QueryScalarAsync(DbAccess cmdAccess)
        {
            if (!cmdAccess.dbModel.Sql.ToLower().Contains("limit"))
            {
                cmdAccess.dbModel.EndSql = "limit 1";
            }
            string sqlStr = cmdAccess.GetSql();
            DataTable dataTable = await GetDataAsync(cmdAccess, sqlStr);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0];
            }
            return null;
        }

        /// <summary>
        /// 查询数据库的行数
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>行数</returns>
        [Obsolete]
        public static int QueryRowCount(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            int fromIndex = sqlStr.ToLower().IndexOf("from");
            //sqlStr.Substring(fromIndex, sqlStr.Length - fromIndex);
            string strFrom = sqlStr[fromIndex..];//找出主脚本
            strFrom = $"select count(0) as counts {strFrom}";
            DataTable dataTable = GetData(cmdAccess, strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0].ToInt();
            }
            return 0;
        }

        /// <summary>
        /// 查询数据库的行数
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>行数</returns>
        [Obsolete]
        public static async Task<int> QueryRowCountAsync(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            int fromIndex = sqlStr.ToLower().IndexOf("from");
            //sqlStr.Substring(fromIndex, sqlStr.Length - fromIndex);
            string strFrom = sqlStr[fromIndex..];//找出主脚本
            strFrom = $"select count(0) as counts {strFrom}";
            DataTable dataTable = await GetDataAsync(cmdAccess, strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0].ToInt();
            }
            return 0;
        }

        /// <summary>
        /// 修改数据库数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>影响行数</returns>
        [Obsolete]
        public static int UpdateData(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            int updateCount = SetData(cmdAccess, sqlStr);
            return updateCount;
        }

        /// <summary>
        /// 修改数据库数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>影响行数</returns>
        [Obsolete]
        public static async Task<int> UpdateDataAsync(DbAccess cmdAccess)
        {
            string sqlStr = cmdAccess.GetSql();
            int updateCount = await SetDataAsync(cmdAccess, sqlStr);
            return updateCount;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>数据</returns>
        [Obsolete]
        private static DataTable GetData(DbAccess cmdAccess, string sqlStr)
        {
            return cmdAccess.dataBase.FindTable(sqlStr);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>数据</returns>
        [Obsolete]
        private static async Task<DataTable> GetDataAsync(DbAccess cmdAccess, string sqlStr)
        {
            return await cmdAccess.dataBase.FindTableAsync(sqlStr);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>影响行数</returns>
        [Obsolete]
        private static int SetData(DbAccess cmdAccess, string sqlStr)
        {
            return cmdAccess.dataBase.ExecuteSqlRaw(sqlStr);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>影响行数</returns>
        [Obsolete]
        private static async Task<int> SetDataAsync(DbAccess cmdAccess, string sqlStr)
        {
            return await cmdAccess.dataBase.ExecuteSqlRawAsync(sqlStr);
        }
    }
}
