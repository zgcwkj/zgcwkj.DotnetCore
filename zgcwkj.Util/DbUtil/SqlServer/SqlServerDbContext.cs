using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace zgcwkj.Util.DbUtil.SqlServer
{
    /// <summary>
    /// SqlServer 数据上下文
    /// </summary>
    public class SqlServerDbContext : DbCommon
    {
        /// <summary>
        /// 配置通过约定发现的模型
        /// </summary>
        /// <param name="modelBuilder">模型制作者</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //提取所有模型
            string filePath = GlobalConstant.GetRunPath;
            //string filePath = Directory.GetCurrentDirectory();
            DirectoryInfo root = new DirectoryInfo(filePath);
            FileInfo[] files = root.GetFiles();
            foreach (var file in files)
            {
                if (file.FullName.Contains("Microsoft")) continue;
                if (file.FullName.Contains("System")) continue;
                if (file.Extension == ".dll")
                {
                    try
                    {
                        string fileName = file.Name.Replace(file.Extension, "");
                        AssemblyName assemblyName = new AssemblyName(fileName);
                        Assembly entityAssembly = Assembly.Load(assemblyName);
                        IEnumerable<Type> typesToRegister = entityAssembly.GetTypes()
                            .Where(p => !string.IsNullOrEmpty(p.Namespace))
                            .Where(p => !string.IsNullOrEmpty(p.GetCustomAttribute<TableAttribute>()?.Name));
                        foreach (Type type in typesToRegister)
                        {
                            dynamic configurationInstance = Activator.CreateInstance(type);
                            modelBuilder.Model.AddEntityType(type);
                        }
                    }
                    catch { }
                }
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
