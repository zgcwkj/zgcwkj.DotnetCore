namespace zgcwkj.Util
{
    /// <summary>
    /// <b>缓存操作提供者</b>
    /// 
    /// <para>常规使用：var cache = CacheProvider.Create()</para>
    /// <para>注入使用：services.AddTransient&lt;CacheAccess&gt;()</para>
    /// </summary>
    public static class CacheProvider
    {
        /// <summary>
        /// 创建缓存命令
        /// </summary>
        /// <returns></returns>
        public static CacheAccess Create()
        {
            return new CacheAccess();
        }

        /// <summary>
        /// Key 是否存在
        /// </summary>
        /// <param name="cacheAccess">对象</param>
        /// <param name="key">键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>存在</returns>
        public static bool Exists(this CacheAccess cacheAccess, string key, int db = -1)
        {
            var cache = cacheAccess.Cache;
            return cache.Exists(key, db);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheAccess">对象</param>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="db">数据库索引</param>
        /// <param name="timeSpan">时间差</param>
        /// <returns>状态</returns>
        public static bool Set<T>(this CacheAccess cacheAccess, string key, T value, int db = -1, TimeSpan timeSpan = default)
        {
            var cache = cacheAccess.Cache;
            return cache.Set(key, value, db);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheAccess">对象</param>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>数据</returns>
        public static T Get<T>(this CacheAccess cacheAccess, string key, int db = -1)
        {
            var cache = cacheAccess.Cache;
            return cache.Get<T>(key, db);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheAccess">对象</param>
        /// <param name="key">类型</param>
        /// <param name="db">数据库索引</param>
        /// <returns>状态</returns>
        public static bool Remove(this CacheAccess cacheAccess, string key, int db = -1)
        {
            var cache = cacheAccess.Cache;
            return cache.Remove(key, db);
        }

        /// <summary>
        /// Key 是否存在(哈希)
        /// </summary>
        /// <param name="cacheAccess">对象</param>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>存在</returns>
        public static bool HashExists(this CacheAccess cacheAccess, string key, string hashKey, int db = -1)
        {
            var cache = cacheAccess.Cache;
            return cache.HashExists(key, hashKey, db);
        }

        /// <summary>
        /// 设置缓存(哈希)
        /// </summary>
        /// <param name="cacheAccess">对象</param>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="hashValue">哈希值</param>
        /// <param name="db">数据库索引</param>
        /// <returns>状态</returns>
        public static bool HashSet<T>(this CacheAccess cacheAccess, string key, string hashKey, T hashValue, int db = -1)
        {
            var cache = cacheAccess.Cache;
            return cache.HashSet(key, hashKey, hashValue, db);
        }

        /// <summary>
        /// 获取缓存(哈希)
        /// </summary>
        /// <param name="cacheAccess">对象</param>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>数据</returns>
        public static T HashGet<T>(this CacheAccess cacheAccess, string key, string hashKey, int db = -1)
        {
            var cache = cacheAccess.Cache;
            return cache.HashGet<T>(key, hashKey, db);
        }

        /// <summary>
        /// 删除缓存(哈希)
        /// </summary>
        /// <param name="cacheAccess">对象</param>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>状态</returns>
        public static long HashRemove(this CacheAccess cacheAccess, string key, string hashKey, int db = -1)
        {
            var cache = cacheAccess.Cache;
            return cache.HashRemove(key, hashKey, db);
        }
    }
}
