﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zgcwkj.Model.Context;
using zgcwkj.Model.Models;
using zgcwkj.Util;
using zgcwkj.Web.Extensions;

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
        /// Jwt 配置
        /// </summary>
        private JwtConfigure _Jwt { get; }

        /// <summary>
        /// 用户会话
        /// </summary>
        private UserSession _UserSession { get; }

        /// <summary>
        /// 实例时
        /// </summary>
        public ApiController(
            MyDbContext myDbContext,
            SQLiteDbContext sQLiteDbContext,
            JwtConfigure jwtConfigure,
            UserSession userSession)
        {
            this._MyDb = myDbContext;
            this._SQLite = sQLiteDbContext;
            this._Db = DbProvider.Create(myDbContext);
            this._Jwt = jwtConfigure;
            this._UserSession = userSession;
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
            //登录方法体
            var myJwt = new
            {
                Account = userName,
                Password = password,
            };
            var myJwtMd5 = myJwt.ToJson().ToMD5();
            var jwtToken = _Jwt.GenerateToken(myJwtMd5, userName);
            //返回结果
            var jsonResult = new
            {
                status = true,
                token = jwtToken,
            };
            return Json(jsonResult);
        }

        /// <summary>
        /// GetLogin
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetLogin()
        {
            return Json(_UserSession);
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
            var testCache = CacheMemory.Get<int>("TestCache");
            CacheMemory.Set("TestCache", testCache + 1);
            return Json(testCache);
        }

        ///// <summary>
        ///// ClearCache
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult ClearCache()
        //{
        //    CacheMemory.Clear();
        //    return Json("OK");
        //}

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
