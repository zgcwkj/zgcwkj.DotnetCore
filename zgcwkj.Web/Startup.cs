using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using zgcwkj.Util;
using zgcwkj.Util.Swagger;

namespace zgcwkj.Web
{
    /// <summary>
    /// 启动入口
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Web主机环境
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        /// <summary>
        /// 启动程序
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="env">环境</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
            //启动日志
            GlobalContext.LogWhenStart(env);
            //主机环境
            GlobalContext.HostingEnvironment = env;
        }

        /// <summary>
        /// 该方法通过运行时调用
        /// 使用此方法将服务添加到容器中
        /// </summary>
        /// <param name="services">服务</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            //注册编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //返回数据首字母不小写
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            //启用缓存功能
            services.AddMemoryCache();
            //启动 Session
            services.AddSession();
            //启动数据保护
            string protection = Path.Combine(Directory.GetCurrentDirectory(), "Protection");
            if (!Directory.Exists(protection)) Directory.CreateDirectory(protection);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(protection));
            //添加 Options 模式
            services.AddOptions();
            //添加 MVC
            services.AddMvc();
            //添加 HttpContext 存取器 
            services.AddHttpContextAccessor();
            //全局异常捕获
            services.AddControllers(options =>
            {
                options.Filters.Add(new GlobalException());
            });
            //配置 Swagger
            services.AddSwagger();
            //所有注册服务和类实例容器
            GlobalContext.Services = services;
            //配置对象
            GlobalContext.Configuration = Configuration;
        }

        /// <summary>
        /// 该方法通过运行时调用
        /// 使用此方法配置HTTP请求流水线
        /// </summary>
        /// <param name="app">应用</param>
        /// <param name="env">环境</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //运行模式
            if (env.IsDevelopment())
            {
                //开发环境展示错误堆栈页
                app.UseDeveloperExceptionPage();
                //用户 Swagger
                app.AddSwagger();
            }
            else
            {
                //正式环境自定义错误页
                app.UseExceptionHandler("/Help/Error");
            }
            //捕获全局的请求
            app.Use(async (context, next) =>
            {
                await next();
                //401 错误
                if (context.Response.StatusCode == 401)
                {
                    context.Request.Path = "/Admin/Index";
                    await next();
                }
                //404 错误
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Help/Error";
                    await next();
                }
                //500 错误
                if (context.Response.StatusCode == 500)
                {
                    context.Request.Path = "/Help/Error";
                    await next();
                }
            });
            //默认的静态目录路径
            app.UseStaticFiles();
            //用户自定义静态目录
            string resource = Path.Combine(env.ContentRootPath, "Resource");
            if (!Directory.Exists(resource)) Directory.CreateDirectory(resource);
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/Resource",
                FileProvider = new PhysicalFileProvider(resource),
                OnPrepareResponse = GlobalContext.SetCacheControl
            });
            //用户 Session
            app.UseSession();
            //用户路由
            app.UseRouting();
            //用户授权
            app.UseAuthorization();
            //用户访问地址重写
            app.UseRewriter(new RewriteOptions()
                .AddRedirect(@"(.*?)[/]{2,}$", "/"));
            //用户默认路由
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //服务提供者
            GlobalContext.ServiceProvider = app.ApplicationServices;
        }
    }
}