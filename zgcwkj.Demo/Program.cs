﻿using System;
using System.Linq;
using zgcwkj.Model;
using zgcwkj.Model.Models;
using zgcwkj.Util;
using zgcwkj.Util.Common;

namespace zgcwkj.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            string userID = MD5Tool.GetMd5("zgcwkj");

            //var d = new RandomTool(true, true, true);
            //var dddd = d.GoRandom(10);
            var s = RandomTool.GetRandomStr(10, "abc123");

            //Cache
            //DataFactory.Cache.Set("zgcwkj", userID);
            Console.WriteLine(DataFactory.Cache.Get<string>("zgcwkj"));

            //Query
            using MyDbContext myDbContext = new MyDbContext();
            var sysUser = myDbContext.SysUserModel.ToList();
            Console.WriteLine(sysUser.ToJson());

            //Query
            var cmd = DbProvider.Create();
            cmd.Clear();
            cmd.SetCommandText("select * from sys_user");
            var dataTable = DbAccess.QueryDataTable(cmd);
            Console.WriteLine(dataTable.ToJson());

            //Affairs
            cmd.Clear();
            cmd.TransBegin();
            cmd.SetCommandText(@"insert into sys_user(user_id,user_name,password) values(@userID,@userName,@password)", "userID", "userName", "myPassword");
            var data = DbAccess.UpdateData(cmd);
            //cmd.TransCommit();
            Console.WriteLine($"updateCount > {data}");

            //Object
            SysUserModel sysUserModelB = new SysUserModel();
            sysUserModelB.UserID = userID;
            var loadOk = sysUserModelB.LoadData();
            if (loadOk)
            {
                sysUserModelB.UserName = "Update";
                sysUserModelB.Password = "Update";
                sysUserModelB.Update();
            }
            else
            {
                sysUserModelB.UserName = "Insert";
                sysUserModelB.Password = "Insert";
                sysUserModelB.Insert();
            }

            //Query
            var sysUsers = DataFactory.Db.QuerTable<SysUserModel>(T => T.UserID == userID);
            Console.WriteLine(sysUsers.ToJson());

            //Query
            var sysUserModels = DataFactory.Db.QuerTable<SysUserModel>();
            Console.WriteLine(sysUserModels.ToJson());

            Console.ReadLine();
        }
    }
}
