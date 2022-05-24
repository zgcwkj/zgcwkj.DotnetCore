﻿using zgcwkj.Util.Interface;

namespace zgcwkj.Util.Data
{
    /// <summary>
    /// 数据工厂
    /// </summary>
    public class DataFactory
    {
        /// <summary>
        /// 数据库对象
        /// </summary>
        public static IDataBase Db
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