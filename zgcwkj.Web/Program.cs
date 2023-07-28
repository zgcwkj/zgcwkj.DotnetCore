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
        /// �������
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
        /// �÷���ͨ������ʱ����
        /// ʹ�ô˷�����������ӵ�������
        /// </summary>
        /// <param name="services">����</param>
        /// <param name="builder">��վ����</param>
        public static void ConfigureServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //�������л���
            GlobalContext.HostingEnvironment = builder.Environment;
            //����ע��������ʵ������
            GlobalContext.Services = services;
            //������������ĸ��Сд
            services.AddMvc().AddJsonOptions(options =>
            {
                //�����������ֲ���
                //PropertyNamingPolicy = null Ĭ�ϲ��ı�
                //PropertyNamingPolicy = JsonNamingPolicy.CamelCase Ĭ��Сд
                //https://docs.microsoft.com/zh-cn/dotnet/api/system.text.json.jsonserializeroptions.propertynamingpolicy?view=net-6.0
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //�������л�
                options.JsonSerializerOptions.Converters.Add(new DateTimeJson());
                //ȡ�� Unicode ����
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //��ֵ������ǰ��
                //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                //����������
                //options.JsonSerializerOptions.AllowTrailingCommas = true;
                //�����л����������������Ƿ�ʹ�ò����ִ�Сд�ıȽ�
                //options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
            });
            //���û��湦��
            services.AddMemoryCache();
            //�������ݱ�������
            services.AddFileDataProtection();
            //���� Session
            services.AddSession(options =>
            {
                options.Cookie.Name = ".AspNetCore.Session";//����Session��Cookie��Key
                options.IdleTimeout = TimeSpan.FromMinutes(20);//����Session�Ĺ���ʱ��
                options.Cookie.HttpOnly = true;//���������������ͨ��js��ø�Cookie��ֵ
                options.Cookie.IsEssential = true;
            });
            //��� Options ģʽ
            services.AddOptions();
            //��� MVC
            services.AddMvc();
            //��� HttpContext ��ȡ��
            services.AddHttpContextAccessor();
            //ȫ���쳣����
            services.AddControllers(options =>
            {
                options.Filters.Add(new GlobalException());
            });
            //���� Jwt
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer();
            services.ConfigureOptions<JwtConfigure>();
            services.AddSingleton<JwtConfigure>();
            services.AddSingleton<UserSession>();
            //���� Swagger
            services.AddSwaggerJwt();
        }

        /// <summary>
        /// �÷���ͨ������ʱ����
        /// ʹ�ô˷�������HTTP������ˮ��
        /// </summary>
        /// <param name="app">Ӧ��</param>
        /// <param name="builder">��վ����</param>
        public static void Configure(this WebApplication app, WebApplicationBuilder builder)
        {
            //�������л���
            GlobalContext.HostingEnvironment = app.Environment;
            //�����ṩ��
            GlobalContext.ServiceProvider = app.Services;
            //���ö���
            GlobalContext.Configuration = app.Configuration;
            //����ģʽ
            if (app.Environment.IsDevelopment())
            {
                //��������չʾ�����ջҳ
                app.UseDeveloperExceptionPage();
                //�û� Swagger
                app.AddSwagger();
            }
            else
            {
                //��ʽ�����Զ������ҳ
                app.UseExceptionHandler("/Help/Error");
                //����ȫ�ֵ�����
                app.Use(async (context, next) =>
                {
                    await next();
                    //401 ����
                    if (context.Response.StatusCode == 401)
                    {
                        context.Request.Path = "/Admin/Index";
                        //await next();
                    }
                    //404 ����
                    if (context.Response.StatusCode == 404)
                    {
                        context.Request.Path = "/Help/Error";
                        //await next();
                    }
                    //500 ����
                    if (context.Response.StatusCode == 500)
                    {
                        context.Request.Path = "/Help/Error";
                        //await next();
                    }
                });
                //�û����ʵ�ַ��д
                var rewriteOptions = new RewriteOptions();
                //rewriteOptions.AddRedirectToHttps();
                rewriteOptions.Add(new PathRewrite());
                app.UseRewriter(rewriteOptions);
            }
            //Ĭ�ϵľ�̬Ŀ¼·��
            app.UseStaticFiles();
            //�û��Զ��徲̬Ŀ¼
            var resource = Path.Combine(app.Environment.ContentRootPath, "Resource");
            if (!Directory.Exists(resource)) Directory.CreateDirectory(resource);
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/Resource",
                FileProvider = new PhysicalFileProvider(resource),
                OnPrepareResponse = GlobalContext.SetCacheControl,
            });
            //�û�·��
            app.UseRouting();
            //�û���Ȩ
            app.UseAuthentication();
            app.UseAuthorization();
            //�û� Session
            app.UseSession();
            //�û�Ĭ��·��
            app.MapControllerRoute(
                name: "areaRoute",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }

        /// <summary>
        /// ����ע��
        /// </summary>
        /// <param name="services">����</param>
        /// <param name="builder">��վ����</param>
        public static void AddInjection(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //�������л���
            GlobalContext.HostingEnvironment = builder.Environment;
            //���ݿ�������
            services.AddDbContext<MyDbContext>();
            services.AddDbContext<SQLiteDbContext>();
        }
    }
}
