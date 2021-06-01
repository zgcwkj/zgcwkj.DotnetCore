using System;
using System.Collections.Generic;
using System.Text;

namespace zgcwkj.Util.CacheUtil
{
    /// <summary>
    /// 我的缓存
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">时间差</param>
        /// <returns></returns>
        bool SetCache<T>(string key, T value, TimeSpan timeSpan = default);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        T GetCache<T>(string key);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">类型</param>
        /// <returns></returns>
        bool RemoveCache(string key);
    }
}
