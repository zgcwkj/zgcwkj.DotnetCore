using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using System;
using System.Collections.Generic;
using System.Text;
using zgcwkj.Util.Common;

namespace zgcwkj.Util.DbUtil
{
    /// <summary>
    /// 执行的脚本日志
    /// </summary>
    public class LogFactory
    {
        /// <summary>
        /// 输出到 DeBug
        /// </summary>
        public static readonly ILoggerFactory LoggerFactoryDeBug = LoggerFactory.Create(builder => { builder.AddDebug(); });

        /// <summary>
        /// 输出到 Console
        /// </summary>
        public static readonly ILoggerFactory loggerFactoryConsole = LoggerFactory.Create(builder => { builder.AddConsole(); });

        /// <summary>
        /// 输出数据
        /// </summary>
        public static void Add(DbContextOptionsBuilder optionsBuilder)
        {
            //控制器
            if (GlobalContext.HostingEnvironment.IsDevelopment())
            {
                optionsBuilder.UseLoggerFactory(loggerFactoryConsole);
            }
            //DeBug
            //if (GlobalContext.HostingEnvironment.IsDevelopment())
            //{
            //    optionsBuilder.UseLoggerFactory(LoggerFactoryDeBug);
            //}
        }
    }
}
