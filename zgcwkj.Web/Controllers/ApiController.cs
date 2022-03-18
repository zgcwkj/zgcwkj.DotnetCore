using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using zgcwkj.Util;

namespace zgcwkj.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {
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
            var dataTable = await DbAccess.QueryDataTableAsync(cmd);
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
