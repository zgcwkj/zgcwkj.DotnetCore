using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace zgcwkj.Util
{
    /// <summary>
    /// Memory 缓存
    /// </summary>
    public static class CacheMemory
    {
        /// <summary>
        /// Memory 缓存
        /// </summary>
        private static IMemoryCache? memoryCache { get; set; }

        /// <summary>
        /// Memory 缓存
        /// </summary>
        private static IMemoryCache _IMemoryCache
        {
            get
            {
                //从上下文中获取
                if (GlobalContext.ServiceProvider != null)
                {
                    memoryCache = GlobalContext.ServiceProvider?.GetService<IMemoryCache>();
                }
                //内置创建对象
                if (memoryCache == null)
                {
                    var memoryCacheOptions = new MemoryCacheOptions();
                    memoryCache = new MemoryCache(memoryCacheOptions);
                }
                //抛出异常
                if (memoryCache == null)
                {
                    throw new Exception("初始化 Memory 缓存失败");
                }
                //
                return memoryCache;
            }
        }

        /// <summary>
        /// Key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>存在</returns>
        public static bool? Exists(string key)
        {
            return _IMemoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">时间差</param>
        /// <returns></returns>
        public static bool Set<T>(string key, T value, TimeSpan timeSpan = default)
        {
            try
            {
                if (timeSpan == default)
                {
                    return _IMemoryCache.Set(key, value) != null;
                }
                return _IMemoryCache.Set(key, value, timeSpan) != null;
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
        public static T? Get<T>(string key)
        {
            return _IMemoryCache.Get<T>(key);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">类型</param>
        /// <returns></returns>
        public static bool? Remove(string key)
        {
            _IMemoryCache.Remove(key);
            return true;
        }

        /// <summary>
        /// Key 是否存在(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns>存在</returns>
        public static bool? ExistsHash(string key, string hashKey)
        {
            return ExistsHashFieldCache(key, hashKey);
        }

        /// <summary>
        /// 设置缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="hashValue">哈希值</param>
        /// <returns>状态</returns>
        public static bool SetHash<T>(string key, string hashKey, T hashValue)
        {
            var count = SetHashFieldCache(key, new Dictionary<string, T> { { hashKey, hashValue } });
            return count > 0;
        }

        /// <summary>
        /// 获取缓存(哈希)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns>数据</returns>
        public static T? GetHash<T>(string key, string hashKey)
        {
            var dict = GetHashFieldCache(key, new Dictionary<string, T> { { hashKey, default } });
            return dict[hashKey];
        }

        /// <summary>
        /// 删除缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns>状态</returns>
        public static long RemoveHash(string key, string hashKey)
        {
            var remove = RemoveHashFieldCache(key, hashKey);
            return remove ? 1 : 0;
        }

        ///// <summary>
        ///// 清理全部缓存
        ///// </summary>
        ///// <returns></returns>
        //public static bool Clear()
        //{
        //    memoryCache?.Dispose();
        //    memoryCache = null;
        //    return true;
        //}

        #region Hash

        /// <summary>
        /// 是否存在(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns></returns>
        private static bool? ExistsHashFieldCache(string key, string hashKey)
        {
            var hashFields = _IMemoryCache.Get<Dictionary<string, object>>(key);
            return hashFields?.ContainsKey(hashKey);
        }

        /// <summary>
        /// 设置缓存(哈希)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="dict">字典</param>
        /// <returns></returns>
        private static int SetHashFieldCache<T>(string key, Dictionary<string, T> dict)
        {
            var count = 0;
            
            // 获取或创建哈希字段字典
            if (!_IMemoryCache.TryGetValue(key, out Dictionary<string, object>? hashFields))
            {
                hashFields = new Dictionary<string, object>();
                _IMemoryCache.Set(key, hashFields);
            }

            if (hashFields == null) return 0;

            // 更新或添加字段
            foreach (var fieldKey in dict.Keys)
            {
                hashFields[fieldKey] = dict[fieldKey]!;
                count++;
            }

            return count;
        }

        /// <summary>
        /// 获取缓存(哈希)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="dict">字典</param>
        /// <returns></returns>
        private static Dictionary<string, T>? GetHashFieldCache<T>(string key, Dictionary<string, T> dict)
        {
            if (!_IMemoryCache.TryGetValue(key, out Dictionary<string, object>? hashFields))
            {
                return null;
            }

            if (hashFields == null) return null;

            foreach (var keyValuePair in hashFields.Where(p => dict.ContainsKey(p.Key)))
            {
                try
                {
                    dict[keyValuePair.Key] = (T)keyValuePair.Value;
                }
                catch (InvalidCastException)
                {
                    // 类型转换失败，跳过该字段
                    Logger.Other($"Hash field type conversion failed for key: {key}, hashKey: {keyValuePair.Key}", "Warn");
                }
            }

            return dict;
        }

        /// <summary>
        /// 删除缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <returns></returns>
        private static bool RemoveHashFieldCache(string key, string hashKey)
        {
            if (!_IMemoryCache.TryGetValue(key, out Dictionary<string, object>? hashFields))
            {
                return false;
            }

            if (hashFields == null) return false;

            return hashFields.Remove(hashKey);
        }

        #endregion Hash
    }
}
