using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace zgcwkj.Util
{
    /// <summary>
    /// Cookie 帮助
    /// </summary>
    public class CookieHelper
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
        /// 获取 Cookie 对象
        /// </summary>
        /// <returns></returns>
        public static IResponseCookies? GetObj()
        {
            return _HttpContext.Response.Cookies;
        }

        /// <summary>
        /// 写入 Cookie
        /// </summary>
        /// <param name="sName">名称</param>
        /// <param name="sValue">值</param>
        /// <param name="httpOnly">前端脚本能否获取到的Cookie</param>
        /// <returns>状态</returns>
        public static bool Set(string sName, string sValue, bool httpOnly = true)
        {
            var option = new CookieOptions
            {
                Expires = DateTime.MaxValue,
                HttpOnly = httpOnly,
            };
            _HttpContext.Response.Cookies.Append(sName, sValue, option);
            return true;
        }

        /// <summary>
        /// 写入 Cookie
        /// </summary>
        /// <param name="sName">名称</param>
        /// <param name="sValue">值</param>
        /// <param name="expires">过期时间(分钟)</param>
        /// <param name="httpOnly">前端脚本能否获取到的Cookie</param>
        /// <returns>状态</returns>
        public static bool Set(string sName, string sValue, int expires, bool httpOnly = true)
        {
            var option = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(expires),
                HttpOnly = httpOnly,
            };
            _HttpContext.Response.Cookies.Append(sName, sValue, option);
            return true;
        }

        /// <summary>
        /// 写入临时 Cookie
        /// </summary>
        /// <param name="sName">名称</param>
        /// <param name="sValue">值</param>
        /// <param name="httpOnly">前端脚本能否获取到的Cookie</param>
        /// <returns>状态</returns>
        public static bool SetTemp(string sName, string sValue, bool httpOnly = true)
        {
            var option = new CookieOptions
            {
                HttpOnly = httpOnly,
            };
            _HttpContext.Response.Cookies.Append(sName, sValue, option);
            return true;
        }

        /// <summary>
        /// 读取 Cookie
        /// </summary>
        /// <param name="sName">名称</param>
        /// <returns>值</returns>
        public static string Get(string sName)
        {
            return _HttpContext.Request.Cookies[sName] ?? "";
        }

        /// <summary>
        /// 删除 Cookie
        /// </summary>
        /// <param name="sName">Cookie对象名称</param>
        /// <returns>状态</returns>
        public static bool Remove(string sName)
        {
            _HttpContext.Response.Cookies.Delete(sName);
            return true;
        }

        /// <summary>
        /// 清空 Cookie
        /// </summary>
        /// <returns>状态</returns>
        public static bool Clear()
        {
            //请求
            var request = _HttpContext.Request;
            if (request == null) return false;
            //响应
            var response = _HttpContext.Response;
            if (response == null) return false;
            //循环所有 Cookie
            foreach (var cookie in request.Cookies)
            {
                var key = cookie.Key;
                //var value = cookie.Value;
                response.Cookies.Delete(key);
            }
            return true;
        }
    }
}