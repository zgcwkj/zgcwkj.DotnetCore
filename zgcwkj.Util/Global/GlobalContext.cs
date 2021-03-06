using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    SetConfigFiles("appsettings.json");
                }
                return configuration;
            }
            set
            {
                configuration = value;
            }
        }

        /// <summary>
        /// 加载自定义的配置文件
        /// 例如：SetConfigFiles("appsettings.json")
        /// </summary>
        public static void SetConfigFiles(params string[] configFileNames)
        {
            var currentDirectory = GlobalConstant.GetRunPath;
            //查找配置文件所在的位置是否正确
            foreach (var configFileName in configFileNames)
            {
                if (!File.Exists($"{currentDirectory}/{configFileName}")) currentDirectory = GlobalConstant.GetRunPath;
                if (!File.Exists($"{currentDirectory}/{configFileName}")) currentDirectory = GlobalConstant.GetRunPath2;
                if (!File.Exists($"{currentDirectory}/{configFileName}")) currentDirectory = GlobalConstant.GetRunPath3;
                if (!File.Exists($"{currentDirectory}/{configFileName}")) currentDirectory = GlobalConstant.GetRunPath4;
                if (!File.Exists($"{currentDirectory}/{configFileName}")) throw new Exception($"找不到“{configFileName}”配置文件");
            }
            //加载配置文件
            var cBuilder = new ConfigurationBuilder();
            var icBuilder = cBuilder.SetBasePath(currentDirectory);
            foreach (var configFileName in configFileNames)
            {
                icBuilder.AddJsonFile(configFileName);
            }

            var config = icBuilder.Build();
            configuration = config;
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
        /// 获取环境变量
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetEnvVar(string key)
        {
            string data = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine) ?? null;
            data ??= Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process) ?? null;
            data ??= Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User) ?? null;
            return data.ToTrim();
        }

        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">值</param>
        /// <param name="lasting">永久</param>
        /// <returns></returns>
        public static bool SetEnvVar(string key, string data, bool lasting = false)
        {
            if (lasting && (GlobalConstant.SystemType == PlatformID.Win32S || GlobalConstant.SystemType == PlatformID.Win32Windows || GlobalConstant.SystemType == PlatformID.Win32NT || GlobalConstant.SystemType == PlatformID.WinCE))
            {
                try
                {
                    //管理员权限
                    Environment.SetEnvironmentVariable(key, data, EnvironmentVariableTarget.Machine);
                }
                catch (Exception)
                {
                    //无管理员权限
                    Environment.SetEnvironmentVariable(key, data, EnvironmentVariableTarget.User);
                }
            }
            else
            {
                Environment.SetEnvironmentVariable(key, data, EnvironmentVariableTarget.Process);
            }
            bool ok = GetEnvVar(key) == data;
            return ok;
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
