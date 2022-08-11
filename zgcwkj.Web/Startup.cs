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
    /// �������
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="configuration">����</param>
        /// <param name="env">����</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            //��������
            GlobalContext.HostingEnvironment = env;
            //���ö���
            GlobalContext.Configuration = configuration;
        }

        /// <summary>
        /// �÷���ͨ������ʱ����
        /// ʹ�ô˷�����������ӵ�������
        /// </summary>
        /// <param name="services">����</param>
        public void ConfigureServices(IServiceCollection services)
        {
            //����ע��������ʵ������
            GlobalContext.Services = services;
            //��ӵ���
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            //ע�����
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //������������ĸ��Сд
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
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
            //���� Swagger
            services.AddSwaggerJwt();
            //���� Jwt
            services.AddJwtConfig(new MyJwtValidator());
        }

        /// <summary>
        /// �÷���ͨ������ʱ����
        /// ʹ�ô˷�������HTTP������ˮ��
        /// </summary>
        /// <param name="app">Ӧ��</param>
        /// <param name="env">����</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //�����ṩ��
            GlobalContext.ServiceProvider = app.ApplicationServices;
            //����ģʽ
            if (env.IsDevelopment())
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
            }
            //����ȫ�ֵ�����
            app.Use(async (context, next) =>
            {
                await next();
                //401 ����
                if (context.Response.StatusCode == 401)
                {
                    context.Request.Path = "/Admin/Index";
                    await next();
                }
                //404 ����
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Help/Error";
                    await next();
                }
                //500 ����
                if (context.Response.StatusCode == 500)
                {
                    context.Request.Path = "/Help/Error";
                    await next();
                }
            });
            //Ĭ�ϵľ�̬Ŀ¼·��
            app.UseStaticFiles();
            //�û��Զ��徲̬Ŀ¼
            string resource = Path.Combine(env.ContentRootPath, "Resource");
            if (!Directory.Exists(resource)) Directory.CreateDirectory(resource);
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/Resource",
                FileProvider = new PhysicalFileProvider(resource),
                OnPrepareResponse = GlobalContext.SetCacheControl,
            });
            //�û�·��
            app.UseRouting();
            //�û� Session
            app.UseSession();
            //�û� WebSockets
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),//��Чʱ��
            };
            app.UseWebSockets(webSocketOptions);
            //���� Jwt
            app.JwtAuthorize();
            //�û����ʵ�ַ��д
            app.UseRewriter(new RewriteOptions().AddRedirect(@"(.*?)[/]{2,}$", "/"));
            //�û�Ĭ��·��
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
