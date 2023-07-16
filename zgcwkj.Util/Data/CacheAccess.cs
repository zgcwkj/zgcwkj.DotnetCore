using zgcwkj.Util.Data;
using zgcwkj.Util.Enum;
using zgcwkj.Util.Interface;

namespace zgcwkj.Util
{
    /// <summary>
    /// <b>缓存操作对象</b>
    ///
    /// <para>常规使用：var cache = CacheProvider.Create()</para>
    /// <para>注入使用：services.AddTransient&lt;CacheAccess&gt;()</para>
    /// </summary>
    public class CacheAccess
    {
        /// <summary>
        /// 缓存对象
        /// </summary>
        internal ICache Cache { get; set; }

        /// <summary>
        /// 实例时
        /// </summary>
        public CacheAccess()
        {
            Cache = CacheFactory.Type switch
            {
                CacheType.Memory => new MemoryImp(),
                CacheType.Redis => new RedisImp(),
                _ => throw new Exception("未找到缓存配置"),
            };
        }

        /// <summary>
        /// Key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>存在</returns>
        public static bool Exists(string key, int db = -1)
        {
            var cacheAccess = new CacheAccess();
            return cacheAccess.Exists(key, db);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="db">数据库索引</param>
        /// <param name="timeSpan">时间差</param>
        /// <returns>状态</returns>
        public static bool Set<T>(string key, T value, int db = -1, TimeSpan timeSpan = default)
        {
            var cacheAccess = new CacheAccess();
            return cacheAccess.Set(key, value, db);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>数据</returns>
        public static T Get<T>(string key, int db = -1)
        {
            var cacheAccess = new CacheAccess();
            return cacheAccess.Get<T>(key, db);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">类型</param>
        /// <param name="db">数据库索引</param>
        /// <returns>状态</returns>
        public static bool Remove(string key, int db = -1)
        {
            var cacheAccess = new CacheAccess();
            return cacheAccess.Remove(key, db);
        }

        /// <summary>
        /// Key 是否存在(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>存在</returns>
        public static bool HashExists(string key, string hashKey, int db = -1)
        {
            var cacheAccess = new CacheAccess();
            return cacheAccess.HashExists(key, hashKey, db);
        }

        /// <summary>
        /// 设置缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="hashValue">哈希值</param>
        /// <param name="db">数据库索引</param>
        /// <returns>状态</returns>
        public static bool HashSet<T>(string key, string hashKey, T hashValue, int db = -1)
        {
            var cacheAccess = new CacheAccess();
            return cacheAccess.HashSet(key, hashKey, hashValue, db);
        }

        /// <summary>
        /// 获取缓存(哈希)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>数据</returns>
        public static T HashGet<T>(string key, string hashKey, int db = -1)
        {
            var cacheAccess = new CacheAccess();
            return cacheAccess.HashGet<T>(key, hashKey, db);
        }

        /// <summary>
        /// 删除缓存(哈希)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashKey">哈希键</param>
        /// <param name="db">数据库索引</param>
        /// <returns>状态</returns>
        public static long HashRemove(string key, string hashKey, int db = -1)
        {
            var cacheAccess = new CacheAccess();
            return cacheAccess.HashRemove(key, hashKey, db);
        }
    }
}