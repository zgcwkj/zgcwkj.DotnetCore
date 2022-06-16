using System;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using zgcwkj.Util;
using zgcwkj.Util.Data;
using zgcwkj.Web.Comm;

namespace zgcwkj.Web.Controllers
{
    /// <summary>
    /// ApiController
    /// </summary>
    [Authorize]
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            //登录方法体
            var myJwtv = new MyJwtValidator
            {
                Account = userName,
                Password = password,
            };
            var jwtToken = JwtConfig.GetToken(myJwtv);
            //返回结果
            var jsonResult = new
            {
                status = jwtToken.Status,
                token = jwtToken.Token,
                validTo = jwtToken.ValidTo,
            };
            return Json(jsonResult);
        }

        /// <summary>
        /// GetData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var cmd = DbProvider.Create();
            cmd.Clear();
            cmd.SetCommandText("select * from sys_user");
            var dataTable = await cmd.QueryDataTableAsync();
            return Json(dataTable.ToList());
        }

        /// <summary>
        /// TestCache
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestCache()
        {
            int testCache = DataFactory.Cache.Get<int>("TestCache");
            DataFactory.Cache.Set("TestCache", testCache + 1);
            return Json(testCache);
        }

        /// <summary>
        /// TestError
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestError()
        {
            throw new Exception("TestError");
        }
    }
}
