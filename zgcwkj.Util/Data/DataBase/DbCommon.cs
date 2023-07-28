using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace zgcwkj.Util.Data
{
    /// <summary>
    /// <b>数据库连接对象</b>
    ///
    /// <para>常规使用：using var dbComm = new DbCommon()</para>
    /// <para>注入使用：services.AddDbContext&lt;DbCommon&gt;()</para>
    /// <para>此处的 DbCommon 为继承后的对象名</para>
    /// <para>数据库需要继承此对象</para>
    /// </summary>
    public abstract class DbCommon : DbContext, IDisposable
    {
        /// <summary>
        /// 配置要使用的数据库
        /// </summary>
        /// <param name="optionsBuilder">上下文选项生成器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //数据库拦截器
            //optionsBuilder.AddInterceptors(new DbInterceptor());
            //输出日志
            EFLogger.Add(optionsBuilder);
            //
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 配置通过约定发现的模型
        /// </summary>
        /// <param name="modelBuilder">模型制作者</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ////排除以实现的
            //var dbsetNames = new List<string>();
            //var inheritType = GetType();
            //foreach (var property in inheritType.GetProperties())
            //{
            //    var pName = property.PropertyType.Name;
            //    if (pName.ToLower().Contains("dbset"))
            //    {
            //        dbsetNames.Add(property.Name);
            //    }
            //}
            //注册所有可能会用到的对象
            var filePath = GlobalConstant.GetRunPath;
            var root = new DirectoryInfo(filePath);
            var files = root.GetFiles("*.dll");
            foreach (var file in files)
            {
                try
                {
                    if (file.FullName.Contains("Microsoft")) continue;
                    if (file.FullName.Contains("System")) continue;
                    var fileName = file.Name.Replace(file.Extension, "");
                    var assemblyName = new AssemblyName(fileName);
                    var entityAssembly = Assembly.Load(assemblyName);
                    var entityAssemblyType = entityAssembly.GetTypes();
                    var typesToRegister = entityAssemblyType
                        .Where(p => p.Namespace != null)//排除没有 命名空间
                        .Where(p => p.GetCustomAttribute<TableAttribute>() != null)//排除没有 Table
                        .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null);//排除标记 NotMapped
                    foreach (var type in typesToRegister)
                    {
                        var createInstance = Activator.CreateInstance(type);
                        modelBuilder.Model.AddEntityType(type);
                    }
                }
                catch { }
            }
            //
            base.OnModelCreating(modelBuilder);
        }
    }
}
