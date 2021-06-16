using System;
using System.Collections.Generic;
using System.Text;
using zgcwkj.Util.Web;
using zgcwkj.Util.Enum;
using zgcwkj.Util.DbUtil.MySql;
using zgcwkj.Util.DbUtil.SqlServer;
using zgcwkj.Util.DbUtil.PostgreSql;
using zgcwkj.Util.DbUtil.SQLite;

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
                DbType dbType;
                string dbTypeStr = ConfigHelp.Get("DbType");
                switch (dbTypeStr.ToLower())
                {
                    case "sqlite":
                        dbType = DbType.SQLite;
                        break;
                    case "postgresql":
                        dbType = DbType.PostgreSql;
                        break;
                    case "sqlserver":
                        dbType = DbType.SqlServer;
                        break;
                    case "mysql":
                        dbType = DbType.MySql;
                        break;
                    default:
                        throw new Exception("未找到数据库配置");
                }
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
                //数据库连接字符
                string dbConnect;
                switch (Type)
                {
                    case DbType.SQLite:
                        dbConnect = ConfigHelp.Get("SQLiteConnect");
                        break;
                    case DbType.PostgreSql:
                        dbConnect = ConfigHelp.Get("PgsqlConnect");
                        break;
                    case DbType.SqlServer:
                        dbConnect = ConfigHelp.Get("MssqlConnect");
                        break;
                    case DbType.MySql:
                        dbConnect = ConfigHelp.Get("MysqlConnect");
                        break;
                    default:
                        throw new Exception("未找到数据库配置");
                }
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
                return 10;
            }
        }

        /// <summary>
        /// 数据库对象
        /// </summary>
        public static IDatabase Db
        {
            get
            {
                //数据库抽象操作对象
                IDatabase dataBase;
                switch (Type)
                {
                    case DbType.SQLite:
                        dataBase = new SQLiteDatabase();
                        break;
                    case DbType.PostgreSql:
                        dataBase = new PgSqlDatabase();
                        break;
                    case DbType.SqlServer:
                        dataBase = new SqlServerDatabase();
                        break;
                    case DbType.MySql:
                        dataBase = new MySqlDatabase();
                        break;
                    default:
                        throw new Exception("未找到数据库配置");
                }
                return dataBase;
            }
        }
    }
}
