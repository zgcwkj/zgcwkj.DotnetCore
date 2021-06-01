using System;
using System.Collections.Generic;
using System.Text;
using zgcwkj.Util.DbUtil;
using zgcwkj.Util.CacheUtil;

namespace zgcwkj.Util
{
    /// <summary>
    /// 数据工厂
    /// </summary>
    public class DataFactory
    {
        /// <summary>
        /// 数据库对象
        /// </summary>
        public static IDatabase Db
        {
            get
            {
                return DbFactory.Db;
            }
        }

        /// <summary>
        /// 缓存对象
        /// </summary>
        public static ICache Cache
        {
            get
            {
                return CacheFactory.Cache;
            }
        }
    }
}
