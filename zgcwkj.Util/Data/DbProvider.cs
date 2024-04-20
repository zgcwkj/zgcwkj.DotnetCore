using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;
using zgcwkj.Util.Data.Extension;

namespace zgcwkj.Util
{
    /// <summary>
    /// <b>数据库操作提供者</b>
    ///
    /// <para>常规使用：var cmd = DbProvider.Create()</para>
    /// <para>注入使用：services.AddTransient&lt;DbProvider&gt;()</para>
    /// <para>当存在多个<b>DbContext</b>时，请传递它</para>
    /// <para>建议使用<b>EF</b>操作数据库</para>
    /// </summary>
    public static class DbProvider
    {
        #region 操作对象

        /// <summary>
        /// 创建数据库命令
        /// </summary>
        /// <returns></returns>
        public static DbAccess Create(DbContext? dbContext = default)
        {
            return new DbAccess(dbContext);
        }

        /// <summary>
        /// 清空准备的数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>状态</returns>
        public static bool Clear(this DbAccess cmdAccess)
        {
            cmdAccess.dbModel = new SqlModel();
            return true;
        }

        #endregion 操作对象

        #region 操作脚本

        /// <summary>
        /// 获取执行的脚本
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns></returns>
        public static string GetSql(this DbAccess cmdAccess)
        {
            var sql = $"{cmdAccess.dbModel.Sql}";
            //追加的脚本
            if (cmdAccess.dbModel.AppendSql.IsNotNull()) sql += $" {cmdAccess.dbModel.AppendSql}";
            //排序的脚本
            if (cmdAccess.dbModel.OrderBy.IsNotNull()) sql += $" {cmdAccess.dbModel.OrderBy}";
            //组合的脚本
            if (cmdAccess.dbModel.GroupBy.IsNotNull()) sql += $" {cmdAccess.dbModel.GroupBy}";
            //结尾的脚本
            if (cmdAccess.dbModel.EndSql.IsNotNull()) sql += $" {cmdAccess.dbModel.EndSql}";
            //返回脚本
            return sql;
        }

        /// <summary>
        /// 设置CommandText
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="commandText">命令Text</param>
        /// <returns>设置状态</returns>
        public static bool SetCommandText(this DbAccess cmdAccess, string commandText)
        {
            cmdAccess.Clear();
            //设置状态
            var setStatus = false;
            //防止空字符
            if (!string.IsNullOrEmpty(commandText))
            {
                setStatus = true;
                cmdAccess.dbModel.Sql = commandText;
            }
            return setStatus;
        }

        /// <summary>
        /// 设置CommandText
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="commandText">命令Text</param>
        /// <param name="commandValues">Sql命令的参数值</param>
        /// <returns>设置状态</returns>
        public static bool SetCommandText(this DbAccess cmdAccess, string commandText, params object[] commandValues)
        {
            cmdAccess.Clear();
            //setStatus 设置状态
            commandText = GetScriptAfterParameter(cmdAccess, out bool setStatus, commandText, commandValues);
            cmdAccess.dbModel.Sql = commandText;

            return setStatus;
        }

        /// <summary>
        /// 末尾追加字符串，并赋参数值
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="filter">字段</param>
        /// <param name="values">值</param>
        /// <returns>追加状态</returns>
        public static bool Append(this DbAccess cmdAccess, string filter, params object[] values)
        {
            //addStatus 添加状态
            filter = GetScriptAfterParameter(cmdAccess, out bool addStatus, filter, values);
            //判断是否需要加 Where
            if (!cmdAccess.dbModel.Sql.ToLower().Contains("where"))
            {
                if (string.IsNullOrEmpty(cmdAccess.dbModel.AppendSql))
                {
                    cmdAccess.dbModel.AppendSql += ExtraSpace($"where {filter}");
                    return addStatus;
                }
            }
            cmdAccess.dbModel.AppendSql += ExtraSpace($"{filter}");
            //添加状态
            return addStatus;
        }

        /// <summary>
        /// 末尾追加字符串，并赋参数值
        /// 追加与条件
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="filter">字段</param>
        /// <param name="values">值</param>
        /// <returns>追加状态</returns>
        public static bool AppendAnd(this DbAccess cmdAccess, string filter, params object[] values)
        {
            //addStatus 添加状态
            filter = GetScriptAfterParameter(cmdAccess, out bool addStatus, filter, values);
            //是否添加
            if (addStatus)
            {
                //判断是否需要加 Where
                if (!cmdAccess.dbModel.Sql.ToLower().Contains("where"))
                {
                    if (string.IsNullOrEmpty(cmdAccess.dbModel.AppendSql))
                    {
                        cmdAccess.dbModel.AppendSql += ExtraSpace($"where {filter}");
                        return addStatus;
                    }
                }
                cmdAccess.dbModel.AppendSql += ExtraSpace($"and {filter}");
            }
            //添加状态
            return addStatus;
        }

        /// <summary>
        /// 末尾追加字符串，并赋参数值
        /// 追加或条件
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="filter">字段</param>
        /// <param name="values">值</param>
        /// <returns>追加状态</returns>
        public static bool AppendOr(this DbAccess cmdAccess, string filter, params object[] values)
        {
            //addStatus 添加状态
            filter = GetScriptAfterParameter(cmdAccess, out bool addStatus, filter, values);
            //是否添加
            if (addStatus)
            {
                //判断是否需要加 Where
                if (!cmdAccess.dbModel.Sql.ToLower().Contains("where"))
                {
                    if (string.IsNullOrEmpty(cmdAccess.dbModel.AppendSql))
                    {
                        cmdAccess.dbModel.AppendSql += ExtraSpace($"where {filter}");
                        return addStatus;
                    }
                    cmdAccess.dbModel.AppendSql += ExtraSpace($"or {filter}");
                }
            }
            //添加状态
            return addStatus;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="filter">字段（多字段用逗号分开）</param>
        /// <param name="values">值</param>
        /// <returns>状态</returns>
        public static bool OrderBy(this DbAccess cmdAccess, string filter, params object[] values)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                filter = GetScriptAfterParameter(cmdAccess, out _, filter, values);
                cmdAccess.dbModel.OrderBy = $"order by {filter}";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="filter">字段（多字段用逗号分开）</param>
        /// <param name="values">值</param>
        /// <returns>状态</returns>
        public static bool GroupBy(this DbAccess cmdAccess, string filter, params object[] values)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                filter = GetScriptAfterParameter(cmdAccess, out _, filter, values);
                cmdAccess.dbModel.GroupBy = $"group by {filter}";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置结尾脚本
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="sqlStr">添加的字符串</param>
        /// <returns>状态</returns>
        public static bool SetEndSql(this DbAccess cmdAccess, string sqlStr)
        {
            if (!string.IsNullOrEmpty(sqlStr))
            {
                cmdAccess.dbModel.EndSql = $"{sqlStr}";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取对应的参数
        /// </summary>
        /// <param name="sqlStr">脚本字符</param>
        /// <returns>匹配到的数据</returns>
        private static MatchCollection GetMatchCollection(string sqlStr)
        {
            return Regex.Matches(sqlStr, @"(?<=[^0-9a-zA-Z])@(?!@)[0-9a-zA-Z_$#@]+");
        }

        /// <summary>
        /// 获取参数后的脚本
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="isStatus">替换状态</param>
        /// <param name="sqlStr">脚本字符</param>
        /// <param name="values">参数值</param>
        /// <returns></returns>
        private static string GetScriptAfterParameter(DbAccess cmdAccess, out bool isStatus, string sqlStr, params object[] values)
        {
            isStatus = false;
            //得到脚本上的变量
            var matchCollection = GetMatchCollection(sqlStr);
            //循环变量
            for (int i = 0; i < values.Count(); i++)
            {
                if (values[i] != null)
                {
                    if (!string.IsNullOrEmpty(values[i].ToString()))
                    {
                        isStatus = true;
                        sqlStr = sqlStr.Replace(matchCollection[i].Value, $"'{values[i].PreventInjection(cmdAccess)}'");
                    }
                }
            }
            return sqlStr;
        }

        /// <summary>
        /// 额外空格（决定是否需要添加）
        /// </summary>
        /// <param name="sqlStr">脚本字符</param>
        /// <returns>添加空格的脚本</returns>
        private static string ExtraSpace(string sqlStr)
        {
            if (sqlStr.Length > 0)
            {
                var startSql = sqlStr.Substring(0, 1);
                if (startSql != " ") sqlStr = $" {sqlStr}";
            }
            return sqlStr;
        }

        /// <summary>
        /// 防止数据脚本的注入
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="cmdAccess">操作对象（在注入时，逆推用）</param>
        /// <returns></returns>
        private static string PreventInjection(this object value, DbAccess cmdAccess)
        {
            var data = value.To<string>().Trim();
            var outLog = false;
            if (data.To<string>().Trim().Contains("\\"))
            {
                outLog = true;
                data = data.To<string>().Trim().Replace("\\", "");
            }
            if (data.To<string>().Trim().Contains("'"))
            {
                outLog = true;
                data = data.To<string>().Trim().Replace("'", "");
            }
            if (outLog)
            {
                Logger.Fatal($"参数含有注入字符，Sql:({cmdAccess.GetSql()}),value:({value})");
            }
            return data;
        }

        ///// <summary>
        ///// 数据库通用脚本
        ///// </summary>
        ///// <param name="sql">Sql脚本</param>
        ///// <returns>脚本</returns>
        //private static string GenericScript(string sql)
        //{
        //    //转成通用脚本
        //    var type = DbFactory.Type;
        //    if (type == Enum.DbType.MySql)
        //    {
        //        //时间函数
        //        if (sql.Contains("getdate()"))
        //        {
        //            sql = sql.Replace("getdate()", "now()");
        //        }
        //        //分页函数
        //        int pageSql = sql.IndexOf("offset");
        //        if (pageSql != -1 && sql.LastIndexOf("only") > pageSql)
        //        {
        //            var updSql = string.Empty;
        //            var page = Regex.Match(sql, @"(?<=offset).+(?=rows fetch next.+)").Value;
        //            var pageSize = Regex.Match(sql, @"(?<=rows fetch next.+).+(?=rows only)").Value;
        //            updSql += $" limit {page},{pageSize}";
        //            sql = sql.Substring(0, pageSql) + updSql;
        //        }
        //        //记录数函数
        //        var topStr = Regex.Match(sql, @"select.+top.+?[0-9]+").Value;
        //        if (!topStr.IsNull())
        //        {
        //            var countStr = Regex.Match(topStr, @"(?<=select.+top.+?)[0-9]+").Value;
        //            sql = sql.Replace(topStr, "select ");
        //            if (!sql.Contains("limit")) sql = $"{sql} limit {countStr}";
        //        }
        //    }
        //    else if (type == Enum.DbType.SqlServer)
        //    {
        //        //时间函数
        //        if (sql.Contains("now()"))
        //        {
        //            sql = sql.Replace("now()", "getdate()");
        //        }
        //        //是否为空函数
        //        var isnullStr = Regex.Match(sql, @"isnull\(.+?\)").Value;
        //        if (!isnullStr.IsNull())
        //        {
        //            var isnullStrB = Regex.Match(sql, @"(?<=isnull\().+?(?=\))").Value;
        //            sql = sql.Replace(isnullStr, $"{isnullStrB} is null");
        //        }
        //        //分页函数
        //        int pageSql = sql.IndexOf("limit");
        //        if (pageSql != -1)
        //        {
        //            if (sql.LastIndexOf(",") > pageSql)
        //            {
        //                var updSql = string.Empty;
        //                if (!sql.ToLower().Contains("order")) updSql = "order by 1";
        //                var page = Regex.Match(sql, @"(?<=limit).+(?=,.+)").Value;
        //                var pageSize = Regex.Match(sql, @"(?<=limit.+,)[0-9]+").Value;
        //                updSql += $" offset {page} rows fetch next {pageSize} rows only";
        //                sql = sql.Substring(0, pageSql) + updSql;
        //            }
        //            else
        //            {
        //                var limitStr = Regex.Match(sql, @"limit.+[0-9]+").Value;
        //                var countStr = Regex.Match(sql, @"(?<=limit).+[0-9]+").Value;
        //                sql = sql.Replace(limitStr, "");
        //                sql = $"select top {countStr} * from ({sql}) tables";
        //            }
        //        }
        //    }
        //    else if (type == Enum.DbType.PostgreSql)
        //    {
        //        //随机函数
        //        if (sql.Contains("rand()"))
        //        {
        //            sql = sql.Replace("rand()", "random()");
        //        }
        //        //是否为空函数
        //        var isnullStr = Regex.Match(sql, @"isnull\(.+?\)").Value;
        //        if (!isnullStr.IsNull())
        //        {
        //            var isnullStrB = Regex.Match(sql, @"(?<=isnull\().+?(?=\))").Value;
        //            sql = sql.Replace(isnullStr, $"{isnullStrB} is null");
        //        }
        //        //分页函数
        //        int pageSql = sql.IndexOf("limit");
        //        if (pageSql != -1 && sql.LastIndexOf(",") > pageSql)
        //        {
        //            var updSql = string.Empty;
        //            var page = Regex.Match(sql, @"(?<=limit).+(?=,.+)").Value;
        //            var pageSize = Regex.Match(sql, @"(?<=limit.+,)[0-9]+").Value;
        //            updSql += $"limit {pageSize} offset {page}";
        //            sql = sql.Substring(0, pageSql) + updSql;
        //        }
        //        //记录数函数
        //        var topStr = Regex.Match(sql, @"select.+top.+?[0-9]+").Value;
        //        if (!topStr.IsNull())
        //        {
        //            var countStr = Regex.Match(topStr, @"(?<=select.+top.+?)[0-9]+").Value;
        //            sql = sql.Replace(topStr, "select ");
        //            if (!sql.Contains("limit")) sql = $"{sql} limit {countStr}";
        //        }
        //    }
        //    return sql;
        //}

        #endregion 操作脚本

        #region 操作数据库

        /// <summary>
        /// 查询数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>数据</returns>
        public static DataTable QueryDataTable(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            return cmdAccess.GetData(sqlStr);
        }

        /// <summary>
        /// 查询数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>数据</returns>
        public static async Task<DataTable> QueryDataTableAsync(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            return await cmdAccess.GetDataAsync(sqlStr);
        }

        /// <summary>
        /// 查询数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>数据</returns>
        public static List<T> QueryDataList<T>(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            return cmdAccess.GetList<T>(sqlStr);
        }

        /// <summary>
        /// 查询数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>数据</returns>
        public static async Task<List<T>> QueryDataListAsync<T>(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            return await cmdAccess.GetListAsync<T>(sqlStr);
        }

        /// <summary>
        /// 查询数据库的第一行数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>行数据</returns>
        public static DataRow? QueryDataRow(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit = strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            var dataTable = cmdAccess.GetData(strFrom);
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
        public static async Task<DataRow?> QueryDataRowAsync(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit = strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            var dataTable = await cmdAccess.GetDataAsync(strFrom);
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
        /// <returns>数据对象</returns>
        public static T? QueryObj<T>(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit = strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            var dataList = cmdAccess.GetList<T>(strFrom);
            return dataList.FirstOrDefault();
        }

        /// <summary>
        /// 查询数据库的第一行数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>数据对象</returns>
        public static async Task<T?> QueryObjAsync<T>(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit = strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            var dataList = await cmdAccess.GetListAsync<T>(strFrom);
            return dataList.FirstOrDefault();
        }

        /// <summary>
        /// 查询数据库的首行首列数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>首行首列</returns>
        public static object? QueryScalar(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit = strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            var dataTable = cmdAccess.GetData(strFrom);
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
        public static async Task<object?> QueryScalarAsync(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strLimit = strFrom.RemoveEnter().ToLower();
            strLimit = Regex.Match(strLimit, @".+limit").Value;
            if (strLimit.IsNull()) strFrom = $"{strFrom} limit 1";
            var dataTable = await cmdAccess.GetDataAsync(strFrom);
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
        public static int QueryRowCount(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strCount = strFrom.RemoveEnter().ToLower();
            strCount = Regex.Match(strCount, @".+select.+count").Value;
            if (strCount.IsNull()) strFrom = $"select count(0) as counts from ({sqlStr}) tables";
            var dataTable = cmdAccess.GetData(strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0].To<int>();
            }
            return 0;
        }

        /// <summary>
        /// 查询数据库的行数
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>行数</returns>
        public static async Task<int> QueryRowCountAsync(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var strFrom = $" {sqlStr}";
            var strCount = strFrom.RemoveEnter().ToLower();
            strCount = Regex.Match(strCount, @".+select.+count").Value;
            if (strCount.IsNull()) strFrom = $"select count(0) as counts from ({sqlStr}) tables";
            var dataTable = await cmdAccess.GetDataAsync(strFrom);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0].To<int>();
            }
            return 0;
        }

        /// <summary>
        /// 修改数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>影响行数</returns>
        public static int UpdateData(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var updateCount = cmdAccess.SetData(sqlStr);
            return updateCount;
        }

        /// <summary>
        /// 修改数据库数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <returns>影响行数</returns>
        public static async Task<int> UpdateDataAsync(this DbAccess cmdAccess)
        {
            var sqlStr = cmdAccess.GetSql();
            var updateCount = await cmdAccess.SetDataAsync(sqlStr);
            return updateCount;
        }

        #endregion 操作数据库

        #region 操作数据库（核心）

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>数据</returns>
        internal static DataTable GetData(this DbAccess cmdAccess, string sqlStr)
        {
            var dbCommon = cmdAccess.dbCommon;
            var dbConnection = dbCommon.Database.GetDbConnection();
            var dbScalar = new DbScalarExtension(dbCommon, dbConnection);
            var dataReader = dbScalar.ExecuteReade(CommandType.Text, sqlStr);
            var dataTable = DbExtension.IDataReaderToDataTable(dataReader);
            return dataTable;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>数据</returns>
        internal static async Task<DataTable> GetDataAsync(this DbAccess cmdAccess, string sqlStr)
        {
            var dbCommon = cmdAccess.dbCommon;
            var dbConnection = dbCommon.Database.GetDbConnection();
            var dbScalar = new DbScalarExtension(dbCommon, dbConnection);
            var dataReader = await dbScalar.ExecuteReadeAsync(CommandType.Text, sqlStr);
            var dataTable = DbExtension.IDataReaderToDataTable(dataReader);
            return dataTable;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>数据</returns>
        internal static List<T> GetList<T>(this DbAccess cmdAccess, string sqlStr)
        {
            var dbCommon = cmdAccess.dbCommon;
            var dbConnection = dbCommon.Database.GetDbConnection();
            var dbScalar = new DbScalarExtension(dbCommon, dbConnection);
            var dataReader = dbScalar.ExecuteReade(CommandType.Text, sqlStr);
            var dataList = DbExtension.IDataReaderToList<T>(dataReader);
            return dataList;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>数据</returns>
        internal static async Task<List<T>> GetListAsync<T>(this DbAccess cmdAccess, string sqlStr)
        {
            var dbCommon = cmdAccess.dbCommon;
            var dbConnection = dbCommon.Database.GetDbConnection();
            var dbScalar = new DbScalarExtension(dbCommon, dbConnection);
            var dataReader = await dbScalar.ExecuteReadeAsync(CommandType.Text, sqlStr);
            var dataList = DbExtension.IDataReaderToList<T>(dataReader);
            return dataList;
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>影响行数</returns>
        internal static int SetData(this DbAccess cmdAccess, string sqlStr)
        {
            var dbCommon = cmdAccess.dbCommon;
            var count = dbCommon.Database.ExecuteSqlRaw(sqlStr);
            return count;
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="cmdAccess">对象</param>
        /// <param name="sqlStr">Sql脚本</param>
        /// <returns>影响行数</returns>
        internal static async Task<int> SetDataAsync(this DbAccess cmdAccess, string sqlStr)
        {
            var dbCommon = cmdAccess.dbCommon;
            var count = await dbCommon.Database.ExecuteSqlRawAsync(sqlStr);
            return count;
        }

        #endregion 操作数据库（核心）

        #region 其他

        /// <summary>
        /// 删除回车
        /// </summary>
        /// <param name="content">要删除的字符串</param>
        /// <returns>删除后的字符串</returns>
        internal static string RemoveEnter(this string content)
        {
            var okStr = content;
            okStr = okStr.Replace("\r", "");
            okStr = okStr.Replace("\n", "");
            return okStr;
        }

        #endregion 其他
    }
}
