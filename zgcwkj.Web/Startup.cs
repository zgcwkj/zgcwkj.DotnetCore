using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using zgcwkj.Util;
using zgcwkj.Web.Comm;

namespace zgcwkj.Web
{
    /// <summary>
    /// 启动入口
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 启动程序
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="env">环境</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            //主机环境
            GlobalContext.HostingEnvironment = env;
            //配置对象
            GlobalContext.Configuration = configuration;
        }

        /// <summary>
        /// 该方法通过运行时调用
        /// 使用此方法将服务添加到容器中
        /// </summary>
        /// <param name="services">服务</param>
        public void ConfigureServices(IServiceCollection services)
        {
            //所有注册服务和类实例容器
            GlobalContext.Services = services;
            //添加单例
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
            //启动数据保护服务
            services.AddFileDataProtection();
            //启动 Session
            services.AddSession(options =>
            {
                options.Cookie.Name = ".AspNetCore.Session";//设置Session在Cookie的Key
                options.IdleTimeout = TimeSpan.FromMinutes(20);//设置Session的过期时间
                options.Cookie.HttpOnly = true;//设置在浏览器不能通过js获得该Cookie的值
                options.Cookie.IsEssential = true;
            });
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
            services.AddSwaggerJwt();
            //配置 Jwt
            services.AddJwtConfig(new MyJwtValidator());
        }

        /// <summary>
        /// 该方法通过运行时调用
        /// 使用此方法配置HTTP请求流水线
        /// </summary>
        /// <param name="app">应用</param>
        /// <param name="env">环境</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //服务提供者
            GlobalContext.ServiceProvider = app.ApplicationServices;
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
                OnPrepareResponse = GlobalContext.SetCacheControl,
            });
            //用户路由
            app.UseRouting();
            //用户 Session
            app.UseSession();
            //用户 WebSockets
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),//有效时长
            };
            app.UseWebSockets(webSocketOptions);
            //启用 Jwt
            app.JwtAuthorize();
            //用户访问地址重写
            app.UseRewriter(new RewriteOptions().AddRedirect(@"(.*?)[/]{2,}$", "/"));
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
        }
    }
}
