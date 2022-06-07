using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace zgcwkj.Util.Interface
{
    /// <summary>
    /// 数据库连接抽象
    /// </summary>
    public interface IDbCommom
    {
        /// <summary>
        /// 配置要使用的数据库 
        /// </summary>
        /// <param name="optionsBuilder">上下文选项生成器</param>
        public abstract void OnConfiguring(DbContextOptionsBuilder optionsBuilder);

        /// <summary>
        /// 配置通过约定发现的模型
        /// </summary>
        /// <param name="modelBuilder">模型制作者</param>
        public abstract void OnModelCreating(ModelBuilder modelBuilder);
    }
}
