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
    /// �������
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ����
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Web��������
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="configuration">����</param>
        /// <param name="env">����</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
            //������־
            GlobalContext.LogWhenStart(env);
            //��������
            GlobalContext.HostingEnvironment = env;
        }

        /// <summary>
        /// �÷���ͨ������ʱ����
        /// ʹ�ô˷�����������ӵ�������
        /// </summary>
        /// <param name="services">����</param>
        public void ConfigureServices(IServiceCollection services)
        {
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
            //���� Session
            services.AddSession();
            //�������ݱ���
            string protection = Path.Combine(Directory.GetCurrentDirectory(), "Protection");
            if (!Directory.Exists(protection)) Directory.CreateDirectory(protection);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(protection));
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
            services.AddSwagger();
            //����ע��������ʵ������
            GlobalContext.Services = services;
            //���ö���
            GlobalContext.Configuration = Configuration;
        }

        /// <summary>
        /// �÷���ͨ������ʱ����
        /// ʹ�ô˷�������HTTP������ˮ��
        /// </summary>
        /// <param name="app">Ӧ��</param>
        /// <param name="env">����</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                OnPrepareResponse = GlobalContext.SetCacheControl
            });
            //�û� Session
            app.UseSession();
            //�û�·��
            app.UseRouting();
            //�û���Ȩ
            app.UseAuthorization();
            //�û����ʵ�ַ��д
            app.UseRewriter(new RewriteOptions()
                .AddRedirect(@"(.*?)[/]{2,}$", "/"));
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
            //�����ṩ��
            GlobalContext.ServiceProvider = app.ApplicationServices;
        }
    }
}