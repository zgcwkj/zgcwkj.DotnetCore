using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace zgcwkj.Util
{
    /// <summary>
    /// <b>数据库操作对象</b>
    ///
    /// <para>常规使用：var cmd = DbProvider.Create()</para>
    /// <para>注入使用：services.AddTransient&lt;DbProvider&gt;()</para>
    /// <para>当存在多个<b>DbContext</b>时，请传递它</para>
    /// <para>建议使用<b>EF</b>操作数据库</para>
    /// </summary>
    public class DbAccess
    {
        /// <summary>
        /// SQL实体
        /// </summary>
        internal SqlModel dbModel { get; set; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        internal DbContext dbCommon { get; }

        /// <summary>
        /// 实例对象时
        /// </summary>
        public DbAccess(DbContext? dbContext = default)
        {
            dbModel = new SqlModel();
            dbCommon = dbContext ?? GetServiceProviderDbContext() ?? throw new Exception("DbContext 未注入");
        }

        /// <summary>
        /// 从注入中的数据库上下文
        /// </summary>
        /// <returns></returns>
        private DbContext? GetServiceProviderDbContext()
        {
            return GlobalContext.ServiceProvider?.GetService<DbContext?>();
        }
    }
}