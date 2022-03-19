using System;
using zgcwkj.Util.DbUtil.MySql;
using zgcwkj.Util.DbUtil.PostgreSql;
using zgcwkj.Util.DbUtil.SQLite;
using zgcwkj.Util.DbUtil.SqlServer;
using zgcwkj.Util.Enum;

namespace zgcwkj.Util.DbUtil
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public class DbFactory
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static DbType Type
        {
            get
            {
                string dbTypeStr = ConfigHelp.Get("DbType") ?? "sqlite";
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
                string dbConnect = Type switch
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
                string dbTimeoutStr = ConfigHelp.Get("DbTimeout") ?? "10";
                return dbTimeoutStr.ToInt();
            }
        }

        /// <summary>
        /// 数据库对象
        /// </summary>
        public static IDatabase Db
        {
            get
            {
                IDatabase dataBase = Type switch
                {
                    DbType.SQLite => new SQLiteDatabase(),
                    DbType.PostgreSql => new PgSqlDatabase(),
                    DbType.SqlServer => new SqlServerDatabase(),
                    DbType.MySql => new MySqlDatabase(),
                    _ => throw new Exception("未找到数据库配置"),
                };
                return dataBase;
            }
        }
    }
}
