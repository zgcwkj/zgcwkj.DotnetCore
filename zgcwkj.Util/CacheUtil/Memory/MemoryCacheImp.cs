using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zgcwkj.Util.Log;

namespace zgcwkj.Util.CacheUtil.Memory
{
    /// <summary>
    /// Memory 缓存
    /// </summary>
    public class MemoryCacheImp : ICache
    {
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T GetCache<T>(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">类型</param>
        /// <returns></returns>
        public bool RemoveCache(string key)
        {
            throw new NotImplementedException();
        }
    }
}
