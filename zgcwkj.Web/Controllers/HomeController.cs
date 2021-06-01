using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zgcwkj.Util;
using zgcwkj.Util.Common;

namespace zgcwkj.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetData()
        {
            var cmd = DbProvider.CreateCommand();
            cmd.Clear();
            cmd.SetCommandText("select * from sys_user");
            var dataTable = DbAccess.QueryDataTable(cmd);
            return Json(dataTable.ToList());
        }

        public async Task<IActionResult> GetDataAwait()
        {
            var cmd = DbProvider.CreateCommand();
            cmd.Clear();
            cmd.SetCommandText("select * from sys_user");
            var dataTable = await DbAccess.QueryDataTableAsync(cmd);
            return Json(dataTable.ToList());
        }
    }
}
