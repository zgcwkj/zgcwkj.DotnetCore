using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CSRedis;
using zgcwkj.Util.Log;
using zgcwkj.Util.Web;
using zgcwkj.Util.DbUtil;
using zgcwkj.Util.Common;

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
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">时间差</param>
        /// <returns></returns>
        public bool SetCache<T>(string key, T value, TimeSpan timeSpan = default)
        {
            try
            {
                if (timeSpan == default)
                {
                    RedisHelper.Set(key, value);
                }
                else
                {
                    RedisHelper.Set(key, value, timeSpan.TotalSeconds.ToInt());
                }
                return true;
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
        /// <returns></returns>
        public T GetCache<T>(string key)
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
        /// <returns></returns>
        public bool RemoveCache(string key)
        {
            var delCount = RedisHelper.Del(key);
            return delCount > 0;
        }
    }
}
