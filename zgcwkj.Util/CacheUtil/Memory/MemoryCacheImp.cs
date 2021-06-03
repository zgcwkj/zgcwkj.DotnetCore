using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using zgcwkj.Util.Log;
using zgcwkj.Util.Common;

namespace zgcwkj.Util.CacheUtil.Memory
{
    /// <summary>
    /// Memory 缓存
    /// </summary>
    public class MemoryCacheImp : ICache
    {
        /// <summary>
        /// Memory 缓存
        /// </summary>
        private static IMemoryCache memoryCache;

        /// <summary>
        /// Redis 缓存实例时
        /// </summary>
        public MemoryCacheImp()
        {
            if (memoryCache.IsNull())
            {
                var memoryCacheOptions = new MemoryCacheOptions();
                memoryCache = new MemoryCache(memoryCacheOptions);
            }
        }

        /// <summary>
        /// Key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>存在</returns>
        public bool Exists(string key)
        {
            return memoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">时间差</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan timeSpan = default)
        {
            return memoryCache.Set(key, value, timeSpan) != null;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return memoryCache.Get<T>(key);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">类型</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            memoryCache.Remove(key);
            return true;
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
        /// 设置缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="hashValue">哈希值</param>
        /// <returns>状态</returns>
        public bool HashSet<T>(string key, string hashKey, T hashValue)
        {
            int count = SetHashFieldCache(key, new Dictionary<string, T> { { hashKey, hashValue } });
            return count > 0;
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
            var dict = GetHashFieldCache<T>(key, new Dictionary<string, T> { { hashKey, default(T) } });
            return dict[hashKey];
        }

        /// <summary>
        /// 删除缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns>状态</returns>
        public long HashRemove(string key, string hashKey)
        {
            bool remove = RemoveHashFieldCache(key, hashKey);
            return remove ? 1 : 0;
        }

        /// <summary>
        /// 获取缓存(哈希)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="dict">字典</param>
        /// <returns></returns>
        public Dictionary<string, T> GetHashFieldCache<T>(string key, Dictionary<string, T> dict)
        {
            var hashFields = memoryCache.Get<Dictionary<string, T>>(key);
            foreach (KeyValuePair<string, T> keyValuePair in hashFields.Where(p => dict.Keys.Contains(p.Key)))
            {
                dict[keyValuePair.Key] = keyValuePair.Value;
            }
            return dict;
        }

        /// <summary>
        /// 设置缓存(哈希)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="dict">字典</param>
        /// <returns></returns>
        public int SetHashFieldCache<T>(string key, Dictionary<string, T> dict)
        {
            int count = 0;
            foreach (string fieldKey in dict.Keys)
            {
                count += memoryCache.Set(key, dict) != null ? 1 : 0;
            }
            return count;
        }

        /// <summary>
        /// 删除缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns></returns>
        public bool RemoveHashFieldCache(string key, string hashKey)
        {
            Dictionary<string, bool> dict = new Dictionary<string, bool> { { hashKey, false } };
            var hashFields = memoryCache.Get<Dictionary<string, object>>(key);
            foreach (string fieldKey in dict.Keys)
            {
                dict[fieldKey] = hashFields.Remove(fieldKey);
            }
            return dict[hashKey];
        }
    }
}
