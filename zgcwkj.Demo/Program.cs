using zgcwkj.Model.Context;
using zgcwkj.Model.Models;
using zgcwkj.Util;

namespace zgcwkj.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var userID = "zgcwkj".ToMD5();

            //Cache
            Console.WriteLine("Cache >");
            CacheAccess.Set("zgcwkj", userID, 0);
            Console.WriteLine(CacheAccess.Get<string>("zgcwkj"));

            //DbContext
            using var myDbContext = new MyDbContext();
            using var sQLiteDbContext = new SQLiteDbContext();

            //Query
            Console.WriteLine("Query >");
            var sysUser = myDbContext.SysUserModel.ToList();
            Console.WriteLine(sysUser.ToJson());

            //Insert
            Console.WriteLine("Insert >");
            var newUserID = GlobalConstant.GuidMd5;
            var sysUser2 = new SysUserModel();
            sysUser2.UserID = newUserID;
            sysUser2.UserName = $"MyName{newUserID}";
            sysUser2.Password = $"MyPassword{newUserID}";
            sysUser2.Privacy = "won't insert ";
            myDbContext.Add(sysUser2);
            int insertCount = myDbContext.SaveChanges();

            //Update
            Console.WriteLine("Update >");
            var sysUser3 = (from sUser in myDbContext.SysUserModel
                            where sUser.UserID == newUserID
                            select sUser).FirstOrDefault();
            sysUser3.Password = $"NewMyPassword{newUserID}";
            int updateCount = myDbContext.SaveChanges();

            //Query
            Console.WriteLine("Query >");
            var sysUsers = myDbContext.SysUserModel.ToList();
            Console.WriteLine(sysUsers.ToJson());

            //Delete
            Console.WriteLine("Delete >");
            var sysUser4 = (from sUser in myDbContext.SysUserModel
                            where sUser.UserID == newUserID
                            select sUser).FirstOrDefault();
            myDbContext.Remove(sysUser4);
            int deleteCount = myDbContext.SaveChanges();

            //Query
            Console.WriteLine("Query >");
            var sysUsers2 = myDbContext.SysUserModel.ToList();
            Console.WriteLine(sysUsers2.ToJson());

            //Query
            Console.WriteLine("Cross-database Query >");
            var sysInfo = sQLiteDbContext.SysInfoModel.ToList();
            var suInfo = (from sInfo in sysInfo
                          join sUser in sysUsers2 on sInfo.SysID equals sUser.SysID
                          select new
                          {
                              sInfo.SysID,
                              sInfo.SysName,
                              sInfo.SysIP,
                              sInfo.SysUrl,
                              sInfo.SysStatus,
                              sUser.UserID,
                              sUser.UserName,
                              sUser.Password,
                          }).ToList();
            Console.WriteLine(suInfo.ToJson());

            Console.ReadLine();
        }
    }
}
