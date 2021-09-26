using System;
using Microsoft.EntityFrameworkCore;
using zgcwkj.Util.Enum;

namespace zgcwkj.Util.DbUtil
{
    /// <summary>
    /// 数据库连接对象
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
            //SQLite
            if (dbType == DbType.SQLite)
            {
                optionsBuilder.UseSqlite(dbConnect, p => p.CommandTimeout(dbTimeout));
            }
            //PostgreSql
            else if (dbType == DbType.PostgreSql)
            {
                optionsBuilder.UseNpgsql(dbConnect, p => p.CommandTimeout(dbTimeout));
            }
            //SqlServer
            else if (dbType == DbType.SqlServer)
            {
                optionsBuilder.UseSqlServer(dbConnect, p => p.CommandTimeout(dbTimeout));
            }
            //MySql
            else if (dbType == DbType.MySql)
            {
                //ServerVersion.TryParse(dbConnect, out ServerVersion serverVersion);
                //optionsBuilder.UseMySql(serverVersion, p => p.CommandTimeout(dbTimeout));
                optionsBuilder.UseMySql(dbConnect, ServerVersion.AutoDetect(dbConnect), p => p.CommandTimeout(dbTimeout));
            }
            //数据库拦截器
            //optionsBuilder.AddInterceptors(new DbInterceptor());
            //输出日志
            EFLogger.Add(optionsBuilder);
            //
            base.OnConfiguring(optionsBuilder);
        }
    }
}
