using System;
using System.Linq;
using System.Text.RegularExpressions;
using zgcwkj.Util.Common;
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
        /// <returns>追加状态</returns>
        public static bool Clear(this DbAccess cmdAccess)
        {
            cmdAccess.dbModel = new SqlModel();
            cmdAccess.dataBase = DataFactory.Db;
            return true;
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
            //设置状态
            bool setStatus = false;
            //得到脚本上的变量
            MatchCollection matchCollection = GetMatchCollection(commandText);
            //循环变量
            for (int i = 0; i < commandValues.Count(); i++)
            {
                setStatus = true;
                commandText = commandText.Replace(matchCollection[i].Value, $"'{commandValues[i].PreventInjection()}'");
            }
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
            //添加状态
            bool addStatus = false;
            //得到脚本上的变量
            MatchCollection matchCollection = GetMatchCollection(filter);
            //循环变量
            for (int i = 0; i < values.Count(); i++)
            {
                addStatus = true;
                filter = filter.Replace(matchCollection[i].Value, $"'{values[i].PreventInjection()}'");
            }
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
            //添加状态
            bool addStatus = false;
            //得到脚本上的变量
            MatchCollection matchCollection = GetMatchCollection(filter);
            //循环变量
            for (int i = 0; i < values.Count(); i++)
            {
                if (values[i] != null)
                {
                    if (!string.IsNullOrEmpty(values[i].ToString()))
                    {
                        addStatus = true;
                        filter = filter.Replace(matchCollection[i].Value, $"'{values[i].PreventInjection()}'");
                    }
                }
            }
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
            //添加状态
            bool addStatus = false;
            //得到脚本上的变量
            MatchCollection matchCollection = GetMatchCollection(filter);
            //循环变量
            for (int i = 0; i < values.Count(); i++)
            {
                if (values[i] != null)
                {
                    if (!string.IsNullOrEmpty(values[i].ToString()))
                    {
                        addStatus = true;
                        filter = filter.Replace(matchCollection[i].Value, $"'{values[i].PreventInjection()}'");
                    }
                }
            }
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
        /// <returns>追加状态</returns>
        public static bool OrderBy(this DbAccess cmdAccess, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
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
        /// <returns>追加状态</returns>
        public static bool GroupBy(this DbAccess cmdAccess, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
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
        /// <returns></returns>
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
        /// 时序库的开始时间
        /// 末尾追加 00:00:00
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>构造值</returns>
        public static string ToTdStartTimeValue(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return $"{value} 00:00:00";
            }
            return string.Empty;
        }

        /// <summary>
        /// 时序库的结束时间
        /// 末尾追加 23:59:59
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>构造值</returns>
        public static string ToTdEndTimeValue(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return $"{value} 23:59:59";
            }
            return string.Empty;
        }

        /// <summary>
        /// 转成符合时序库的时间
        /// 格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>构造值</returns>
        public static string ToTdDateTimeValue(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    string mes = ex.Message;
                    throw;
                }
            }
            return string.Empty;
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
        /// <returns></returns>
        private static string PreventInjection(this object val)
        {
            string data = val.ToTrim();
            data = data.ToTrim().Replace("\\", "\\\\");
            data = data.ToTrim().Replace("'", "\\'");
            return data;
        }
    }
}
