using Microsoft.EntityFrameworkCore.Storage;
using zgcwkj.Util.Data;

namespace zgcwkj.Util
{
    /// <summary>
    /// <b>数据库操作对象</b>
    ///
    /// <para>常规使用：using var cmd = DbProvider.Create()</para>
    /// <para>注入使用：services.AddTransient&lt;DbAccess&gt;()</para>
    /// <para>建议使用<b>EF</b>操作数据库</para>
    /// </summary>
    public class DbAccess : IDisposable
    {
        /// <summary>
        /// SQL实体
        /// </summary>
        internal SqlModel dbModel { get; set; }

        /// <summary>
        /// 数据操作对象
        /// </summary>
        internal DbCommon dbCommon { get; set; }

        /// <summary>
        /// 事务对象
        /// </summary>
        internal IDbContextTransaction dbTrans { get; set; }

        /// <summary>
        /// 实例对象时
        /// </summary>
        public DbAccess()
        {
            dbModel = new SqlModel();
            dbCommon = new DbCommon();
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            dbCommon?.Dispose();
            dbTrans?.Dispose();
        }
    }
}
