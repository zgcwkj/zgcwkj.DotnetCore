using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using zgcwkj.Util.DbUtil.DbHelp;
using zgcwkj.Util.Enum;

namespace zgcwkj.Util.DbUtil
{
    /// <summary>
    /// 数据连接对象
    /// </summary>
    public class DbCommon : DbContext, IDisposable
    {
        /// <summary>
        /// 配置要使用的数据库 
        /// </summary>
        /// <param name="optionsBuilder">上下文选项生成器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //数据库类型
            var dbType = DbFactory.Type;
            //连接字符
            string dbConnect = DbFactory.Connect;
            //连接命令超时
            int dbTimeout = DbFactory.Timeout;
            //MySql
            if (dbType == DbType.MySql)
            {
                //ServerVersion serverVersion;
                //ServerVersion.TryParse(dbConnect, out serverVersion);
                //optionsBuilder.UseMySql(serverVersion, p => p.CommandTimeout(dbTimeout));
                optionsBuilder.UseMySql(dbConnect, ServerVersion.AutoDetect(dbConnect), p => p.CommandTimeout(dbTimeout));
            }
            //SqlServer
            else if (dbType == DbType.SqlServer)
            {
                optionsBuilder.UseSqlServer(dbConnect, p => p.CommandTimeout(dbTimeout));
            }
            //PostgreSql
            else if (dbType == DbType.PostgreSql)
            {
                optionsBuilder.UseNpgsql(dbConnect, p => p.CommandTimeout(dbTimeout));
                ////初始化Postgres Uuid扩展 
                //using var npgsqlConnection = new NpgsqlConnection(dbConnect);
                //npgsqlConnection.Open();
                //using var npgsqlCommand = npgsqlConnection.CreateCommand();
                //npgsqlCommand.CommandText = "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";";
                //npgsqlCommand.ExecuteNonQuery();
            }
            //数据库拦截器
            optionsBuilder.AddInterceptors(new DbInterceptor());
            //输出日志
            //optionsBuilder.UseLoggerFactory(loggerFactory);
            //
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 输出到 DeBug
        /// </summary>
        public static readonly LoggerFactory LoggerFactoryDeBug = new LoggerFactory(new[] { new DebugLoggerProvider() });

        /// <summary>
        /// 输出到Console
        /// </summary>
        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
    }
}
