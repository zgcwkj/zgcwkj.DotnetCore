using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace zgcwkj.Util
{
    /// <summary>
    /// Cookie 帮助
    /// </summary>
    public class CookieHelper
    {
        /// <summary>
        /// 写入 Cookie
        /// </summary>
        /// <param name="sName">名称</param>
        /// <param name="sValue">值</param>
        /// <param name="httpOnly">前端脚本能否获取到的Cookie</param>
        public static bool Set(string sName, string sValue, bool httpOnly = true)
        {
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = httpOnly
            };
            hca?.HttpContext?.Response.Cookies.Append(sName, sValue, option);
            return true;
        }

        /// <summary>
        /// 写入 Cookie
        /// </summary>
        /// <param name="sName">名称</param>
        /// <param name="sValue">值</param>
        /// <param name="expires">过期时间(分钟)</param>
        /// <param name="httpOnly">前端脚本能否获取到的Cookie</param>
        public static bool Set(string sName, string sValue, int expires, bool httpOnly = true)
        {
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(expires),
                HttpOnly = httpOnly
            };
            hca?.HttpContext?.Response.Cookies.Append(sName, sValue, option);
            return true;
        }

        /// <summary>
        /// 读取 Cookie
        /// </summary>
        /// <param name="sName">名称</param>
        /// <returns>Cookie值</returns>
        public static string Get(string sName)
        {
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            return hca?.HttpContext?.Request.Cookies[sName];
        }

        /// <summary>
        /// 删除 Cookie
        /// </summary>
        /// <param name="sName">Cookie对象名称</param>
        public static bool Remove(string sName)
        {
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            hca?.HttpContext?.Response.Cookies.Delete(sName);
            return true;
        }
    }
}
