# zgcwkj.Util Help

> Config

```
{
  "DbType": "PostgreSql", //SqlServer MySql PostgreSql
  "MssqlConnect": "server=127.0.0.1;port=1433;username=sa;password=123456;database=dbname;", //SqlServer
  "MysqlConnect": "server=127.0.0.1;port=3306;username=root;password=root;database=dbname;charset=utf8;Pooling=true;", //MySql
  "PgsqlConnect": "server=127.0.0.1;port=5432;username=postgres;password=root;database=dbname;", //PostgreSql
  "CacheType": "Redis", //Redis Memory
  "RedisConnect": "127.0.0.1" //Redis
}

```

> Code
```
//Insert
using MyDbContext myDbContext = new MyDbContext();
var sysUser = myDbContext.SysUserModel.ToList();
Console.WriteLine(sysUser.ToJson());
```