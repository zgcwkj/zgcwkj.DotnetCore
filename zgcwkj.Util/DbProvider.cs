using System.Linq;
using System.Text.RegularExpressions;
using zgcwkj.Util.Data.DataBase;
using zgcwkj.Util.Models;

namespace zgcwkj.Util
{
    /// <summary>
    /// 数据库操作对象
    /// </summary>
    public static class DbProvider
    {
        /// <summary>
        /// 创建数据库命令
        /// </summary>
        /// <returns></returns>
        public static DbAccess Create()
        {
            return new DbAccess();
        }

        /// <summary>
        /// 清空准备的数据
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <returns>状态</returns>
        public static bool Clear(this DbAccess cmdAccess)
        {
            cmdAccess.dbModel = new SqlModel();
            cmdAccess.dataBase = DataFactory.Db;
            return true;
        }

        /// <summary>
        /// 获取执行的脚本
        /// </summary>
        /// <returns></returns>
        public static string GetSql(this DbAccess cmdAccess)
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
            //返回脚本
            return sql;
        }

        /// <summary>
        /// 设置CommandText
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="commandText">命令Text</param>
        /// <returns>设置状态</returns>
        public static bool SetCommandText(this DbAccess cmdAccess, string commandText)
        {
            //设置状态
            bool setStatus = false;
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
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="commandText">命令Text</param>
        /// <param name="commandValues">Sql命令的参数值</param>
        /// <returns>设置状态</returns>
        public static bool SetCommandText(this DbAccess cmdAccess, string commandText, params object[] commandValues)
        {
            //setStatus 设置状态
            commandText = GetScriptAfterParameter(cmdAccess, out bool setStatus, commandText, commandValues);
            cmdAccess.dbModel.Sql = commandText;

            return setStatus;
        }

        /// <summary>
        /// 末尾追加字符串，并赋参数值
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
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
        /// <param name="cmdAccess">脚本模型</param>
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
        /// <param name="cmdAccess">脚本模型</param>
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
        /// <param name="cmdAccess">脚本模型</param>
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
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="filter">字段（多字段用逗号分开）</param>
        /// <param name="values">值</param>
        /// <returns>状态</returns>
        public static bool GroupBy(this DbAccess cmdAccess, string filter, params object[] values)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                filter = GetScriptAfterParameter(cmdAccess,out _, filter, values);
                cmdAccess.dbModel.GroupBy = $"group by {filter}";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置结尾脚本
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
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
        /// 事务开始
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        public static void TransBegin(this DbAccess cmdAccess)
        {
            cmdAccess.dataBase.BeginTrans();
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        public static int TransCommit(this DbAccess cmdAccess)
        {
            return cmdAccess.dataBase.CommitTrans();
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="cmdAccess">脚本模型</param>
        public static void TransRollback(this DbAccess cmdAccess)
        {
            cmdAccess.dataBase.RollbackTrans();
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
        /// <param name="cmdAccess">脚本模型</param>
        /// <param name="isStatus">替换状态</param>
        /// <param name="sqlStr">脚本字符</param>
        /// <param name="values">参数值</param>
        /// <returns></returns>
        private static string GetScriptAfterParameter(DbAccess cmdAccess, out bool isStatus, string sqlStr, params object[] values)
        {
            isStatus = false;
            //得到脚本上的变量
            MatchCollection matchCollection = GetMatchCollection(sqlStr);
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
                string startSql = sqlStr.Substring(0, 1);
                if (startSql != " ")
                {
                    sqlStr = $" {sqlStr}";
                }
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
            string data = value.ToTrim();
            bool outLog = false;
            if (data.ToTrim().Contains("\\"))
            {
                outLog = true;
                data = data.ToTrim().Replace("\\", "");
            }
            if (data.ToTrim().Contains("'"))
            {
                outLog = true;
                data = data.ToTrim().Replace("'", "");
            }
            if (outLog)
            {
                Log.Logger.Fatal($"参数含有注入字符，Sql:({cmdAccess.GetSql()}),value:({value})");
            }
            return data;
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
