using Microsoft.EntityFrameworkCore;
using zgcwkj.Model.DefaultData;
using zgcwkj.Model.Models;
using zgcwkj.Util;
using zgcwkj.Util.Enum;
using zgcwkj.Util.Data.DataBase;

namespace zgcwkj.Model.Context
{
    /// <summary>
    /// 数据连接对象
    /// </summary>
    public class SQLiteDbContext : DbCommon
    {
        public SQLiteDbContext() : base(DbType.SQLite, ConfigHelp.Get("SQLite2Connect"))
        {

        }

        /// <summary>
        /// 配置要使用的数据库 
        /// </summary>
        /// <param name="optionsBuilder">上下文选项生成器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 配置通过约定发现的所有模型
        /// </summary>
        /// <param name="modelBuilder">模型制作者</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //初始化用户数据
            modelBuilder.Entity<SysInfoModel>().HasData(SysInfoDBInitializer.GetData);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 系统信息表
        /// </summary>
        public DbSet<SysInfoModel> SysInfoModel { get; set; }
    }
}
