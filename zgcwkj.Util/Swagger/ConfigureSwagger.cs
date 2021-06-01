using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace zgcwkj.Util.Swagger
{
    /// <summary>
    /// 配置 Swagger
    /// </summary>
    public static class ConfigureSwagger
    {
        /// <summary>
        /// Swagger 配置
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="title">标题</param>
        /// <param name="version">版本</param>
        /// <param name="description">描述</param>
        public static void AddSwagger(this IServiceCollection services, string title = "接口文档", string version = "v1", string description = default)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //注册 Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title,
                    Version = version,
                    Description = description,
                });
                options.OrderActionsBy(o => o.RelativePath);
                //Set the comments path for the swagger json and ui
                string basePath = Directory.GetCurrentDirectory();
                DirectoryInfo root = new DirectoryInfo(basePath);
                FileInfo[] files = root.GetFiles();
                foreach (var file in files)
                {
                    if (file.Extension == ".xml")
                    {
                        //var xmlPath = Path.Combine(basePath, "Util.xml");
                        options.IncludeXmlComments(file.FullName);
                    }
                }
            });
        }

        /// <summary>
        /// Swagger 路由
        /// </summary>
        /// <param name="app">应用</param>
        /// <param name="url">地址</param>
        /// <param name="name">名称</param>
        public static void AddSwagger(this IApplicationBuilder app, string url = "/swagger/v1/swagger.json", string name = "Web API")
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(url, name);
                //修改界面打开时自动折叠
                options.DocExpansion(DocExpansion.None);
            });
        }
    }
}
