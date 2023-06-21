using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace zgcwkj.Util
{
    /// <summary>
    /// <b>数据库操作对象</b>
    ///
    /// <para>常规使用：var cmd = DbProvider.Create(dbContext)</para>
    /// <para>注入使用：services.AddTransient&lt;DbAccess&gt;(dbContext)</para>
    /// <para>建议使用<b>EF</b>操作数据库</para>
    /// </summary>
    public class DbAccess
    {
        /// <summary>
        /// SQL实体
        /// </summary>
        internal SqlModel dbModel { get; set; }

        /// <summary>
        /// 数据操作对象
        /// </summary>
        internal DbContext dbCommon { get; set; }

        /// <summary>
        /// 事务对象
        /// </summary>
        internal IDbContextTransaction dbTrans { get; set; }

        /// <summary>
        /// 实例对象时
        /// </summary>
        public DbAccess(DbContext dbContext)
        {
            dbModel = new SqlModel();
            dbCommon = dbContext;
        }
    }
}
