using CSRedis;
using System;
using zgcwkj.Util.Common;
using zgcwkj.Util.Log;

namespace zgcwkj.Util.CacheUtil.Redis
{
    /// <summary>
    /// Redis 缓存
    /// </summary>
    public class RedisCacheImp : ICache
    {
        /// <summary>
        /// Redis 客户端
        /// </summary>
        private static CSRedisClient csRedisClient;

        /// <summary>
        /// Redis 缓存实例时
        /// </summary>
        public RedisCacheImp()
        {
            if (csRedisClient.IsNull())
            {
                csRedisClient = new CSRedisClient(CacheFactory.Connect);
                //初始化 RedisHelper
                RedisHelper.Initialization(csRedisClient);
            }
        }

        /// <summary>
        /// Key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>存在</returns>
        public bool Exists(string key)
        {
            return RedisHelper.Exists(key);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">时间差</param>
        /// <returns>状态</returns>
        public bool Set<T>(string key, T value, TimeSpan timeSpan = default)
        {
            try
            {
                if (timeSpan == default)
                {
                    return RedisHelper.Set(key, value);
                }
                return RedisHelper.Set(key, value, timeSpan.TotalSeconds.ToInt());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>数据</returns>
        public T Get<T>(string key)
        {
            var t = default(T);
            try
            {
                var value = RedisHelper.Get<T>(key);
                if (value.IsNull())
                {
                    return t;
                }
                return value;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return t;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">类型</param>
        /// <returns>状态</returns>
        public bool Remove(string key)
        {
            var delCount = RedisHelper.Del(key);
            return delCount > 0;
        }

        /// <summary>
        /// Key 是否存在(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns>存在</returns>
        public bool HashExists(string key, string hashKey)
        {
            return RedisHelper.HExists(key, hashKey);
        }

        /// <summary>
        /// 设置缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="hashValue">哈希值</param>
        /// <returns>状态</returns>
        public bool HashSet<T>(string key, string hashKey, T hashValue)
        {
            try
            {
                return RedisHelper.HSet(key, hashKey, hashValue);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 获取缓存(哈希)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns>数据</returns>
        public T HashGet<T>(string key, string hashKey)
        {
            return RedisHelper.HGet<T>(key, hashKey);
        }

        /// <summary>
        /// 删除缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns>状态</returns>
        public long HashRemove(string key, string hashKey)
        {
            return RedisHelper.HDel(key, hashKey);
        }
    }
}
