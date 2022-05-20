using System;
using System.Collections.Generic;
using System.Text;

namespace zgcwkj.Util.Data.Cache.Redis
{
    /// <summary>
    /// Redis 缓存
    /// </summary>
    public class RedisImp : ICache
    {
        /// <summary>
        /// Key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>存在</returns>
        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>数据</returns>
        public T Get<T>(string key)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">类型</param>
        /// <returns>状态</returns>
        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Key 是否存在(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns>存在</returns>
        public bool HashExists(string key, string hashKey)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns>状态</returns>
        public long HashRemove(string key, string hashKey)
        {
            throw new NotImplementedException();
        }
    }
}
