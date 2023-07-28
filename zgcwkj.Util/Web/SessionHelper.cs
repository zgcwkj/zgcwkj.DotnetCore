using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace zgcwkj.Util
{
    /// <summary>
    /// Session 帮助
    /// </summary>
    public class SessionHelper
    {
        /// <summary>
        /// Http 上下文
        /// </summary>
        private static HttpContext _HttpContext
        {
            get
            {
                var hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>() ?? throw new Exception("HttpContextAccessor 未注入");
                var hc = hca.HttpContext ?? throw new Exception("HttpContext 未注入");
                return hc;
            }
        }

        /// <summary>
        /// 获取 Session 对象
        /// </summary>
        /// <returns></returns>
        public static ISession GetObj()
        {
            return _HttpContext.Session;
        }

        /// <summary>
        /// 写入 Session
        /// </summary>
        /// <typeparam name="T">Session键值的类型</typeparam>
        /// <param name="key">Session的键名</param>
        /// <param name="value">Session的键值</param>
        /// <returns>状态</returns>
        public static bool Set<T>(string key, T value) where T : notnull
        {
            if (string.IsNullOrEmpty(key)) return false;
            _HttpContext.Session.SetString(key, value.ToJson());
            return true;
        }

        /// <summary>
        /// 写入 Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        /// <param name="value">Session的键值</param>
        /// <returns>状态</returns>
        public static bool Set(string key, string value)
        {
            return Set<string>(key, value);
        }

        /// <summary>
        /// 读取 Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        /// <returns>值</returns>
        public static T? Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) return default;
            var sessionStr = _HttpContext.Session.GetString(key);
            if (string.IsNullOrEmpty(sessionStr)) return default;
            return sessionStr.ToJson<T>();
        }

        /// <summary>
        /// 读取 Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        /// <returns>值</returns>
        public static string Get(string key)
        {
            return Get<string>(key) ?? "";
        }

        /// <summary>
        /// 删除 Session
        /// </summary>
        /// <param name="key">Session的键名</param>
        /// <returns>状态</returns>
        public static bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            _HttpContext.Session.Remove(key);
            return true;
        }

        /// <summary>
        /// 清空 Session
        /// </summary>
        /// <returns>状态</returns>
        public static bool Clear()
        {
            _HttpContext.Session.Clear();
            return true;
        }
    }
}
