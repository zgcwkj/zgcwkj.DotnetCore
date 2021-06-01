using System;
using System.Collections.Generic;
using System.Text;
using zgcwkj.Util.CacheUtil.Memory;
using zgcwkj.Util.CacheUtil.Redis;
using zgcwkj.Util.Common;
using zgcwkj.Util.Enum;
using zgcwkj.Util.Web;

namespace zgcwkj.Util.CacheUtil
{
    /// <summary>
    /// 缓存工厂
    /// </summary>
    public class CacheFactory
    {
        /// <summary>
        /// 缓存类型
        /// </summary>
        public static CacheType Type
        {
            get
            {
                CacheType cacheType;
                string dbTypeStr = ConfigHelp.Get("CacheType");
                switch (dbTypeStr.ToLower())
                {
                    case "redis":
                        cacheType = CacheType.Redis;
                        break;
                    case "memory":
                        cacheType = CacheType.Memory;
                        break;
                    default:
                        throw new Exception("未找到缓存配置");
                }
                return cacheType;
            }
        }

        /// <summary>
        /// 缓存连接字符串
        /// </summary>
        public static string Connect
        {
            get
            {
                //数据库连接字符
                string dbConnect;
                switch (Type)
                {
                    case CacheType.Redis:
                        dbConnect = ConfigHelp.Get("RedisConnect");
                        break;
                    case CacheType.Memory:
                        dbConnect = ConfigHelp.Get("MemoryConnect");
                        break;
                    default:
                        throw new Exception("未找到缓存配置");
                }
                return dbConnect;
            }
        }

        /// <summary>
        /// 缓存对象
        /// </summary>
        public static ICache Cache
        {
            get
            {
                ICache cache;
                switch (Type)
                {
                    case CacheType.Redis:
                        cache = new RedisCacheImp();
                        break;
                    case CacheType.Memory:
                        cache = new MemoryCacheImp();
                        break;
                    default:
                        throw new Exception("未找到缓存配置");
                }
                return cache;
            }
        }
    }
}
