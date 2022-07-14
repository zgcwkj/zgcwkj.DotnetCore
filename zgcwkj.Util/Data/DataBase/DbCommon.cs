using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using zgcwkj.Util.Enum;

namespace zgcwkj.Util.Data
{
    /// <summary>
    /// <b>数据库连接对象</b>
    /// 
    /// <para>常规使用：using var dbComm = new DbContext()</para>
    /// <para>注入使用：services.AddDbContext&lt;DbContext&gt;()</para>
    /// <para>继承此对象可以实现原生操作！by zgcwkj</para>
    /// </summary>
    public class DbCommon : DbContext, IDisposable
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        private DbType dbType { get; }

        /// <summary>
        /// 连接命令超时
        /// </summary>
        private int dbTimeout { get; }

        /// <summary>
        /// 连接字符
        /// </summary>
        private string dbConnect { get; }

        /// <summary>
        /// 数据库版本
        /// </summary>
        private int dbVersion { get; }

        /// <summary>
        /// <para>数据库连接对象</para>
        /// <para><b>使用配置文件中信息来连接</b></para>
        /// </summary>
        public DbCommon()
        {
            this.dbType = DbFactory.Type;
            this.dbTimeout = DbFactory.Timeout;
            this.dbConnect = DbFactory.Connect;
            this.dbVersion = 0;
            if (Regex.Match(DbFactory.Connect, "version=.+?;").Value.IsNotNull())
            {
                this.dbConnect = DbFactory.Connect.Replace(Regex.Match(DbFactory.Connect, "version=.+?;").Value, "");
                this.dbVersion = Regex.Match(DbFactory.Connect, "(?<=version=).+?(?=;)").Value.ToInt();
            }
        }

        /// <summary>
        /// <para>数据库连接对象</para>
        /// <para><b>使用自定义配置来连接</b></para>
        /// <para>用法：base(DbType, "SQLConnect")</para>
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="dbConnect">连接字符</param>
        /// <param name="dbTimeout">连接超时</param>
        public DbCommon(DbType dbType, string dbConnect, int dbTimeout = 10)
        {
            this.dbType = dbType;
            this.dbTimeout = dbTimeout == 10 ? dbTimeout : DbFactory.Timeout;
            this.dbConnect = dbConnect;
            this.dbVersion = 0;
            if (Regex.Match(dbConnect, "version=.+?;").Value.IsNotNull())
            {
                this.dbConnect = dbConnect.Replace(Regex.Match(dbConnect, "version=.+?;").Value, "");
                this.dbVersion = Regex.Match(dbConnect, "(?<=version=).+?(?=;)").Value.ToInt();
            }
        }

        /// <summary>-
        /// <para>数据库连接对象</para>
        /// <para><b>使用自定义配置来连接，数据库类型相同</b></para>
        /// <para>用法：base("SQLConnect")</para>
        /// </summary>
        /// <param name="dbConnect">连接字符</param>
        /// <param name="dbTimeout">连接超时</param>
        public DbCommon(string dbConnect, int dbTimeout = 10)
        {
            this.dbType = DbFactory.Type;
            this.dbTimeout = dbTimeout == 10 ? dbTimeout : DbFactory.Timeout;
            this.dbConnect = dbConnect;
            this.dbVersion = 0;
            if (Regex.Match(dbConnect, "version=.+?;").Value.IsNotNull())
            {
                this.dbConnect = dbConnect.Replace(Regex.Match(dbConnect, "version=.+?;").Value, "");
                this.dbVersion = Regex.Match(dbConnect, "(?<=version=).+?(?=;)").Value.ToInt();
            }
        }

        /// <summary>
        /// 配置要使用的数据库 
        /// </summary>
        /// <param name="optionsBuilder">上下文选项生成器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //SQLite
            if (dbType == DbType.SQLite)
            {
                optionsBuilder.UseSqlite(dbConnect, p =>
                {
                    p.CommandTimeout(dbTimeout);
                });
            }
            //PostgreSql
            else if (dbType == DbType.PostgreSql)
            {
                optionsBuilder.UseNpgsql(dbConnect, p =>
                {
                    p.CommandTimeout(dbTimeout);
                    //指定数据库版本
                    if (this.dbVersion > 0)
                    {
                        p.SetPostgresVersion(this.dbVersion, 0);
                    }
                });
            }
            //SqlServer
            else if (dbType == DbType.SqlServer)
            {
                optionsBuilder.UseSqlServer(dbConnect, p =>
                {
                    p.CommandTimeout(dbTimeout);
                });
            }
            //MySql
            else if (dbType == DbType.MySql)
            {
                optionsBuilder.UseMySql(dbConnect, ServerVersion.AutoDetect(dbConnect), p =>
                {
                    p.CommandTimeout(dbTimeout);
                });
            }
            //数据库拦截器
            //optionsBuilder.AddInterceptors(new DbInterceptor());
            //输出日志
            EFLogger.Add(optionsBuilder);
            //
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 配置通过约定发现的模型
        /// </summary>
        /// <param name="modelBuilder">模型制作者</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //提取所有模型
            var filePath = GlobalConstant.GetRunPath;
            var root = new DirectoryInfo(filePath);
            var files = root.GetFiles("*.dll");
            foreach (var file in files)
            {
                if (file.FullName.Contains("Microsoft")) continue;
                if (file.FullName.Contains("System")) continue;
                try
                {
                    var fileName = file.Name.Replace(file.Extension, "");
                    var assemblyName = new AssemblyName(fileName);
                    var entityAssembly = Assembly.Load(assemblyName);
                    var typesToRegister = entityAssembly.GetTypes()
                        .Where(p => !string.IsNullOrEmpty(p.Namespace))
                        .Where(p => !string.IsNullOrEmpty(p.GetCustomAttribute<TableAttribute>()?.Name));
                    foreach (var type in typesToRegister)
                    {
                        dynamic configurationInstance = Activator.CreateInstance(type);
                        modelBuilder.Model.AddEntityType(type);
                    }
                }
                catch { }
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
