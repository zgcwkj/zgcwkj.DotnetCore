using System;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using zgcwkj.Util.Data.DataBase;

namespace zgcwkj.Util.Data.Extension
{
    /// <summary>
    /// 数据库参数拓展
    /// </summary>
    public class DbParameterExtension
    {
        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter()
        {
            return DbFactory.Type switch
            {
                Enum.DbType.MySql => new MySqlParameter(),
                Enum.DbType.SqlServer => new SqlParameter(),
                Enum.DbType.PostgreSql => new NpgsqlParameter(),
                Enum.DbType.SQLite => new SqliteParameter(),
                _ => throw new Exception("未找到数据库配置"),
            };
        }

        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(string paramName, object value)
        {
            DbParameter param = CreateDbParameter();
            param.ParameterName = paramName;
            param.Value = value;
            return param;
        }

        /// <summary>
        /// 转换对应的数据库参数
        /// </summary>
        /// <param name="dbParameter">参数</param>
        /// <returns></returns>
        public static DbParameter[] ToDbParameter(DbParameter[] dbParameter)
        {
            int i = 0;
            int size = dbParameter.Length;
            DbParameter[] _dbParameter;
            switch (DbFactory.Type)
            {
                case Enum.DbType.MySql:
                    _dbParameter = new MySqlParameter[size];
                    while (i < size)
                    {
                        _dbParameter[i] = new MySqlParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                        i++;
                    }
                    break;
                case Enum.DbType.SqlServer:
                    _dbParameter = new SqlParameter[size];
                    while (i < size)
                    {
                        _dbParameter[i] = new SqlParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                        i++;
                    }
                    break;
                case Enum.DbType.PostgreSql:
                    _dbParameter = new NpgsqlParameter[size];
                    while (i < size)
                    {
                        _dbParameter[i] = new NpgsqlParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                        i++;
                    }
                    break;
                default:
                    throw new Exception("未找到数据库配置");
            }
            return _dbParameter;
        }
    }
}
