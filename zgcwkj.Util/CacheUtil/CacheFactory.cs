using System;
using zgcwkj.Util.CacheUtil.Memory;
using zgcwkj.Util.CacheUtil.Redis;
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
                string dbTypeStr = ConfigHelp.Get("CacheType") ?? "memory";
                var cacheType = dbTypeStr.ToLower() switch
                {
                    "memory" => CacheType.Memory,
                    "redis" => CacheType.Redis,
                    _ => throw new Exception("未找到缓存配置"),
                };
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
                string dbConnect = Type switch
                {
                    CacheType.Memory => ConfigHelp.Get("MemoryConnect"),
                    CacheType.Redis => ConfigHelp.Get("RedisConnect"),
                    _ => throw new Exception("未找到缓存配置"),
                };
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
                ICache cache = Type switch
                {
                    CacheType.Memory => new MemoryCacheImp(),
                    CacheType.Redis => new RedisCacheImp(),
                    _ => throw new Exception("未找到缓存配置"),
                };
                return cache;
            }
        }
    }
}
