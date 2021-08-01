using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using zgcwkj.Util.Common;

namespace zgcwkj.Util
{
    /// <summary>
    /// 全局上下文
    /// </summary>
    public class GlobalContext
    {
        /// <summary>
        /// All registered service and class instance container. Which are used for dependency injection.
        /// 所有注册服务和类实例容器。用于依赖注入。
        /// </summary>
        public static IServiceCollection Services { get; set; }

        /// <summary>
        /// Configured service provider.
        /// 已配置的服务提供商。
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 配置对象(私有)
        /// </summary>
        private static IConfiguration configuration;

        /// <summary>
        /// 配置对象
        /// </summary>
        public static IConfiguration Configuration
        {
            get
            {
                if (configuration.IsNull())
                {
                    var cBuilder = new ConfigurationBuilder();
                    var currentDirectory = Directory.GetCurrentDirectory();
                    var icBuilder = cBuilder.SetBasePath(currentDirectory);
                    var builder = icBuilder.AddJsonFile("appsettings.json");
                    var config = builder.Build();
                    configuration = config;
                }
                return configuration;
            }
            set
            {
                configuration = value;
            }
        }

        /// <summary>
        /// Web主机环境
        /// </summary>
        public static IWebHostEnvironment HostingEnvironment { get; set; }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}";
        }

        /// <summary>
        /// 程序启动时，记录目录
        /// </summary>
        /// <param name="env"></param>
        public static void LogWhenStart(IWebHostEnvironment env)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("程序启动完成");
            sb.Append("\r\n");
            sb.Append($"Version：{GetVersion()}");
            sb.Append("\r\n");
            sb.Append($"IsDevelopment：{env.IsDevelopment()}");
            sb.Append("\r\n");
            sb.Append($"ContentRootPath：{env.ContentRootPath}");
            sb.Append("\r\n");
            sb.Append($"WebRootPath：{env.WebRootPath}");
            sb.Append("\r\n");
            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// 设置 Cache 控制
        /// </summary>
        /// <param name="context">静态文件响应上下文</param>
        public static void SetCacheControl(StaticFileResponseContext context)
        {
            int second = 365 * 24 * 60 * 60;
            context.Context.Response.Headers.Add("Cache-Control", new[] { "public,max-age=" + second });
            context.Context.Response.Headers.Add("Expires", new[] { DateTime.UtcNow.AddYears(1).ToString("R") }); // Format RFC1123
        }
    }
}
