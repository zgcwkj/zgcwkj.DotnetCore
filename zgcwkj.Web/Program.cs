using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.FileProviders;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using zgcwkj.Model.Context;
using zgcwkj.Util;
using zgcwkj.Web.Extensions;

namespace zgcwkj.Web
{
    /// <summary>
    /// Program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.ConfigureServices(builder);
            builder.Services.AddInjection(builder);
            var app = builder.Build();
            app.Configure(builder);
            app.Run();
        }

        /// <summary>
        /// 该方法通过运行时调用
        /// 使用此方法将服务添加到容器中
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="builder">网站程序</param>
        public static void ConfigureServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //程序运行环境
            GlobalContext.HostingEnvironment = builder.Environment;
            //所有注册服务和类实例容器
            GlobalContext.Services = services;
            //返回数据首字母不小写
            services.AddMvc().AddJsonOptions(options =>
            {
                //返回数据首字不变
                //PropertyNamingPolicy = null 默认不改变
                //PropertyNamingPolicy = JsonNamingPolicy.CamelCase 默认小写
                //https://docs.microsoft.com/zh-cn/dotnet/api/system.text.json.jsonserializeroptions.propertynamingpolicy?view=net-6.0
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //数据序列化
                options.JsonSerializerOptions.Converters.Add(new DateTimeJson());
                //取消 Unicode 编码
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //空值不反回前端
                //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                //允许额外符号
                //options.JsonSerializerOptions.AllowTrailingCommas = true;
                //反序列化过程中属性名称是否使用不区分大小写的比较
                //options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
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
            //配置 Jwt
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer();
            services.ConfigureOptions<JwtConfigure>();
            services.AddSingleton<JwtConfigure>();
            services.AddSingleton<UserSession>();
            //配置 Swagger
            services.AddSwaggerJwt();
        }

        /// <summary>
        /// 该方法通过运行时调用
        /// 使用此方法配置HTTP请求流水线
        /// </summary>
        /// <param name="app">应用</param>
        /// <param name="builder">网站程序</param>
        public static void Configure(this WebApplication app, WebApplicationBuilder builder)
        {
            //程序运行环境
            GlobalContext.HostingEnvironment = app.Environment;
            //服务提供者
            GlobalContext.ServiceProvider = app.Services;
            //配置对象
            GlobalContext.Configuration = app.Configuration;
            //运行模式
            if (app.Environment.IsDevelopment())
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
                //捕获全局的请求
                app.Use(async (context, next) =>
                {
                    await next();
                    //401 错误
                    if (context.Response.StatusCode == 401)
                    {
                        context.Request.Path = "/Admin/Index";
                        //await next();
                    }
                    //404 错误
                    if (context.Response.StatusCode == 404)
                    {
                        context.Request.Path = "/Help/Error";
                        //await next();
                    }
                    //500 错误
                    if (context.Response.StatusCode == 500)
                    {
                        context.Request.Path = "/Help/Error";
                        //await next();
                    }
                });
                //用户访问地址重写
                var rewriteOptions = new RewriteOptions();
                //rewriteOptions.AddRedirectToHttps();
                rewriteOptions.Add(new PathRewrite());
                app.UseRewriter(rewriteOptions);
            }
            //默认的静态目录路径
            app.UseStaticFiles();
            //用户自定义静态目录
            var resource = Path.Combine(app.Environment.ContentRootPath, "Resource");
            if (!Directory.Exists(resource)) Directory.CreateDirectory(resource);
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/Resource",
                FileProvider = new PhysicalFileProvider(resource),
                OnPrepareResponse = GlobalContext.SetCacheControl,
            });
            //用户路由
            app.UseRouting();
            //用户鉴权
            app.UseAuthentication();
            app.UseAuthorization();
            //用户 Session
            app.UseSession();
            //用户默认路由
            app.MapControllerRoute(
                name: "areaRoute",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="builder">网站程序</param>
        public static void AddInjection(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //程序运行环境
            GlobalContext.HostingEnvironment = builder.Environment;
            //数据库上下文
            services.AddDbContext<MyDbContext>();
            services.AddDbContext<SQLiteDbContext>();
        }
    }
}
