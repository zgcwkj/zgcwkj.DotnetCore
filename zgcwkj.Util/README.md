# zgcwkj.Util Help

> Config

```
{
  "SQLConnect": "data source=dbName.db", //SQLite
}
```

> Code
```
//Cache
Console.WriteLine("Cache >");
CacheMemory.Set("zgcwkj", userID);
Console.WriteLine(CacheMemory.Get<string>("zgcwkj"));

//DbContext
using var myDbContext = new MyDbContext();
using var sQLiteDbContext = new SQLiteDbContext();

//Query SQL
var dbAccess = DbProvider.Create(myDbContext);
dbAccess.SetCommandText("select * from sys_user");
var sysUserTable = dbAccess.QueryDataTable();
var sysUserList = dbAccess.QueryDataList<SysUserModel>();

//Query
Console.WriteLine("Query >");
var sysUser = myDbContext.SysUserModel.ToList();
Console.WriteLine(sysUser.To<string>());

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
Console.WriteLine(sysUsers.To<string>());

//Delete
Console.WriteLine("Delete >");
var sysUser4 = (from sUser in myDbContext.SysUserModel
                where sUser.UserID == newUserID
                select sUser).FirstOrDefault();
if (sysUser4 != null) myDbContext.Remove(sysUser4);
int deleteCount = myDbContext.SaveChanges();

//Query
Console.WriteLine("Query >");
var sysUsers2 = myDbContext.SysUserModel.ToList();
Console.WriteLine(sysUsers2.To<string>());

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
Console.WriteLine(suInfo.To<string>());
```
