using System;
using zgcwkj.Util.Enum;

namespace zgcwkj.Util.Data
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    internal class DbFactory
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static DbType Type
        {
            get
            {
                var dbTypeStr = ConfigHelp.Get("DbType") ?? "sqlite";
                var dbType = dbTypeStr.ToLower() switch
                {
                    "sqlite" => DbType.SQLite,
                    "postgresql" => DbType.PostgreSql,
                    "sqlserver" => DbType.SqlServer,
                    "mysql" => DbType.MySql,
                    _ => throw new Exception("未找到数据库配置"),
                };
                return dbType;
            }
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string Connect
        {
            get
            {
                var dbConnect = Type switch
                {
                    DbType.SQLite => ConfigHelp.Get("SQLiteConnect"),
                    DbType.PostgreSql => ConfigHelp.Get("PgsqlConnect"),
                    DbType.SqlServer => ConfigHelp.Get("MssqlConnect"),
                    DbType.MySql => ConfigHelp.Get("MysqlConnect"),
                    _ => throw new Exception("未找到数据库配置"),
                };
                return dbConnect;
            }
        }

        /// <summary>
        /// 数据库连接命令超时
        /// </summary>
        public static int Timeout
        {
            get
            {
                var dbTimeoutStr = ConfigHelp.Get("DbTimeout");
                return dbTimeoutStr.IsNullOrZero() ? 10 : dbTimeoutStr.ToInt();
            }
        }
    }
}
