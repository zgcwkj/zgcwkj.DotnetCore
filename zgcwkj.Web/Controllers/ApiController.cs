using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zgcwkj.Model.Context;
using zgcwkj.Model.Models;
using zgcwkj.Util;

namespace zgcwkj.Web.Controllers
{
    /// <summary>
    /// ApiController
    /// </summary>
    //[Authorize]
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private MyDbContext _MyDb { get; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private SQLiteDbContext _SQLite { get; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private DbAccess _Db { get; }

        /// <summary>
        /// 缓存数据上下文
        /// </summary>
        private CacheAccess _Cache { get; }

        /// <summary>
        /// 实例时
        /// </summary>
        /// <param name="myDbContext">数据库上下文</param>
        /// <param name="sQLiteDbContext">数据库上下文</param>
        /// <param name="cacheAccess">缓存数据上下文</param>
        public ApiController(MyDbContext myDbContext, SQLiteDbContext sQLiteDbContext, CacheAccess cacheAccess)
        {
            this._MyDb = myDbContext;
            this._SQLite = sQLiteDbContext;
            this._Db = DbProvider.Create(myDbContext);
            this._Cache = cacheAccess;
        }

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
            ////登录方法体
            //var myJwtv = new MyJwtValidator
            //{
            //    Account = userName,
            //    Password = password,
            //};
            //var jwtToken = JwtConfig.GetToken(myJwtv);
            ////返回结果
            //var jsonResult = new
            //{
            //    status = jwtToken.Status,
            //    token = jwtToken.Token,
            //    validTo = jwtToken.ValidTo,
            //};
            //return Json(jsonResult);
            return Json("");
        }

        /// <summary>
        /// GetData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetData()
        {
            _Db.Clear();
            _Db.SetCommandText("select * from sys_user");
            var dataTable = _Db.QueryDataTable();
            return Json(dataTable.ToList());
        }

        /// <summary>
        /// GetData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetData2()
        {
            _Db.Clear();
            _Db.SetCommandText("select * from sys_user");
            var dataTable = _Db.QueryDataList<SysUserModel>();
            return Json(dataTable);
        }

        /// <summary>
        /// GetEFData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetEFData()
        {
            var linqData = _MyDb.SysUserModel;
            return Json(linqData.ToList());
        }

        /// <summary>
        /// TestCache
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestCache()
        {
            int testCache = CacheAccess.Get<int>("TestCache");
            CacheAccess.Set("TestCache", testCache + 1);
            return Json(testCache);
        }

        /// <summary>
        /// TestCache
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestCache2()
        {
            int testCache = _Cache.Get<int>("TestCache");
            _Cache.Set("TestCache", testCache + 1);
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
