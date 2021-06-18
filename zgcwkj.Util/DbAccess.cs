using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using zgcwkj.Util.Common;
using zgcwkj.Util.DbUtil;
using zgcwkj.Util.Models;

namespace zgcwkj.Util
{
    /// <summary>
    /// 数据库操作对象
    /// </summary>
    public class DbAccess
    {
        /// <summary>
        /// SQL实体
        /// </summary>
        public SqlModel dbModel;

        /// <summary>
        /// 数据操作对象
        /// </summary>
        public IDatabase dataBase;

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
        public static DataTable QueryDataTable(DbAccess cmdAccess)
        {
            string sqlStr = GetSql(cmdAccess);
            return GetData(cmdAccess, sqlStr);
        }

        /// <summary>
        /// 查询数据库数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>数据</returns>
        public static async Task<DataTable> QueryDataTableAsync(DbAccess cmdAccess)
        {
            string sqlStr = GetSql(cmdAccess);
            return await GetDataAsync(cmdAccess, sqlStr);
        }

        /// <summary>
        /// 查询数据库的第一行数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>行数据</returns>
        public static DataRow QueryDataRow(DbAccess cmdAccess)
        {
            if (!cmdAccess.dbModel.Sql.ToLower().Contains("limit"))
            {
                cmdAccess.dbModel.EndSql = "limit 1";
            }
            string sqlStr = GetSql(cmdAccess);
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
        public static async Task<DataRow> QueryDataRowAsync(DbAccess cmdAccess)
        {
            if (!cmdAccess.dbModel.Sql.ToLower().Contains("limit"))
            {
                cmdAccess.dbModel.EndSql = "limit 1";
            }
            string sqlStr = GetSql(cmdAccess);
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
        public static object QueryScalar(DbAccess cmdAccess)
        {
            if (!cmdAccess.dbModel.Sql.ToLower().Contains("limit"))
            {
                cmdAccess.dbModel.EndSql = "limit 1";
            }
            string sqlStr = GetSql(cmdAccess);
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
        public static async Task<object> QueryScalarAsync(DbAccess cmdAccess)
        {
            if (!cmdAccess.dbModel.Sql.ToLower().Contains("limit"))
            {
                cmdAccess.dbModel.EndSql = "limit 1";
            }
            string sqlStr = GetSql(cmdAccess);
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
        public static int QueryRowCount(DbAccess cmdAccess)
        {
            string sqlStr = GetSql(cmdAccess);
            int fromIndex = sqlStr.ToLower().IndexOf("from");
            //sqlStr.Substring(fromIndex, sqlStr.Length - fromIndex);
            string strFrom = sqlStr[fromIndex..];//找出主脚本
            strFrom = $"select count(0) as counts {strFrom}";
            DataTable dataTable = GetData(cmdAccess, strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return Convert.ToInt32(dataTable.Rows[0][0]);
            }
            return 0;
        }

        /// <summary>
        /// 查询数据库的行数
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>行数</returns>
        public static async Task<int> QueryRowCountAsync(DbAccess cmdAccess)
        {
            string sqlStr = GetSql(cmdAccess);
            int fromIndex = sqlStr.ToLower().IndexOf("from");
            //sqlStr.Substring(fromIndex, sqlStr.Length - fromIndex);
            string strFrom = sqlStr[fromIndex..];//找出主脚本
            strFrom = $"select count(0) as counts {strFrom}";
            DataTable dataTable = await GetDataAsync(cmdAccess, strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return Convert.ToInt32(dataTable.Rows[0][0]);
            }
            return 0;
        }

        /// <summary>
        /// 修改数据库数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns></returns>
        public static int UpdateData(DbAccess cmdAccess)
        {
            string sqlStr = GetSql(cmdAccess);
            int updateCount = SetData(cmdAccess, sqlStr);
            return updateCount;
        }

        /// <summary>
        /// 修改数据库数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns></returns>
        public static async Task<int> UpdateDataAsync(DbAccess cmdAccess)
        {
            string sqlStr = GetSql(cmdAccess);
            int updateCount = await SetDataAsync(cmdAccess, sqlStr);
            return updateCount;
        }

        /// <summary>
        /// 获取SQL
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>SQL</returns>
        private static string GetSql(DbAccess cmdAccess)
        {
            string sql = $"{cmdAccess.dbModel.Sql}";
            //追加的脚本
            if (!string.IsNullOrEmpty(cmdAccess.dbModel.AppendSql)) sql += $" {cmdAccess.dbModel.AppendSql}";
            //排序的脚本
            if (!string.IsNullOrEmpty(cmdAccess.dbModel.OrderBy)) sql += $" {cmdAccess.dbModel.OrderBy}";
            //组合的脚本
            if (!string.IsNullOrEmpty(cmdAccess.dbModel.GroupBy)) sql += $" {cmdAccess.dbModel.GroupBy}";
            //结尾的脚本
            if (!string.IsNullOrEmpty(cmdAccess.dbModel.EndSql)) sql += $" {cmdAccess.dbModel.EndSql}";
            //数据库通用脚本
            sql = GenericScript(sql);
            //调试环境输出执行的脚本
            //if (GlobalContext.HostingEnvironment.IsDevelopment()) Log.Logger.Info($"Sql：{sql}");
            return sql;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>表格</returns>
        private static DataTable GetData(DbAccess cmdAccess, string sqlStr)
        {
            return cmdAccess.dataBase.FindTable(sqlStr);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>表格</returns>
        private static async Task<DataTable> GetDataAsync(DbAccess cmdAccess, string sqlStr)
        {
            return await cmdAccess.dataBase.FindTableAsync(sqlStr);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>表格</returns>
        private static int SetData(DbAccess cmdAccess, string sqlStr)
        {
            return cmdAccess.dataBase.ExecuteSqlRaw(sqlStr);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>表格</returns>
        private static async Task<int> SetDataAsync(DbAccess cmdAccess, string sqlStr)
        {
            return await cmdAccess.dataBase.ExecuteSqlRawAsync(sqlStr);
        }

        /// <summary>
        /// 数据库通用脚本
        /// </summary>
        /// <param name="sql">Sql脚本</param>
        /// <returns>脚本</returns>
        private static string GenericScript(string sql)
        {
            //转成通用脚本
            var type = DbFactory.Type;
            if (type == Enum.DbType.MySql)
            {
                //时间函数
                if (sql.Contains("getdate()"))
                {
                    sql = sql.Replace("getdate()", "now()");
                }
                //空字符函数
                string ifnullStr = Regex.Match(sql, @"coalesce\(.+?\)").Value;
                if (!ifnullStr.IsNull())
                {
                    string ifnullStrB = Regex.Match(sql, @"(?<=coalesce\().+?(?=\))").Value;
                    sql = sql.Replace(ifnullStr, $"ifnull({ifnullStrB})");
                }
                //分页函数
                int pageSql = sql.IndexOf("offset");
                if (pageSql != -1 && sql.LastIndexOf("only") > pageSql)
                {
                    string updSql = string.Empty;
                    var page = Regex.Match(sql, @"(?<=offset).+(?=rows fetch next.+)").Value;
                    var pageSize = Regex.Match(sql, @"(?<=rows fetch next.+).+(?=rows only)").Value;
                    updSql += $" limit {page},{pageSize}";
                    sql = sql.Substring(0, pageSql) + updSql;
                }
            }
            else if (type == Enum.DbType.SqlServer)
            {
                //时间函数
                if (sql.Contains("now()"))
                {
                    sql = sql.Replace("now()", "getdate()");
                }
                //是否为空函数
                string isnullStr = Regex.Match(sql, @"isnull\(.+?\)").Value;
                if (!isnullStr.IsNull())
                {
                    string isnullStrB = Regex.Match(sql, @"(?<=isnull\().+?(?=\))").Value;
                    sql = sql.Replace(isnullStr, $"{isnullStrB} is null");
                }
                //分页函数
                int pageSql = sql.IndexOf("limit");
                if (pageSql != -1 && sql.LastIndexOf(",") > pageSql)
                {
                    string updSql = string.Empty;
                    if (!sql.ToLower().Contains("order")) updSql = "order by 1";
                    var page = Regex.Match(sql, @"(?<=limit).+(?=,.+)").Value;
                    var pageSize = Regex.Match(sql, @"(?<=limit.+,)[0-9]+").Value;
                    updSql += $" offset {page} rows fetch next {pageSize} rows only";
                    sql = sql.Substring(0, pageSql) + updSql;
                }
            }
            else if (type == Enum.DbType.PostgreSql)
            {
                //时间函数
                if (sql.Contains("rand()"))
                {
                    sql = sql.Replace("rand()", "random()");
                }
                //是否为空函数
                string isnullStr = Regex.Match(sql, @"isnull\(.+?\)").Value;
                if (!isnullStr.IsNull())
                {
                    string isnullStrB = Regex.Match(sql, @"(?<=isnull\().+?(?=\))").Value;
                    sql = sql.Replace(isnullStr, $"{isnullStrB} is null");
                }
                //空字符函数
                string ifnullStr = Regex.Match(sql, @"ifnull\(.+?\)").Value;
                if (!ifnullStr.IsNull())
                {
                    string ifnullStrB = Regex.Match(sql, @"(?<=ifnull\().+?(?=\))").Value;
                    sql = sql.Replace(ifnullStr, $"coalesce({ifnullStrB})");
                }
                //分页函数
                int pageSql = sql.IndexOf("limit");
                if (pageSql != -1 && sql.LastIndexOf(",") > pageSql)
                {
                    string updSql = string.Empty;
                    var page = Regex.Match(sql, @"(?<=limit).+(?=,.+)").Value;
                    var pageSize = Regex.Match(sql, @"(?<=limit.+,)[0-9]+").Value;
                    updSql += $"limit {pageSize} offset {page}";
                    sql = sql.Substring(0, pageSql) + updSql;
                }
            }
            return sql;
        }
    }
}
