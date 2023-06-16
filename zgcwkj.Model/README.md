                     _/\__
               ---==/    \\
         ___  ___   |.    \|\
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\

## 官方文档：

https://docs.microsoft.com/zh-cn/ef/core/cli/dotnet


### 卸载当前版本

```
dotnet tool uninstall -g dotnet-ef
```

### 安装最新版本

```
dotnet tool install -g dotnet-ef
```

### 使用说明

```
dotnet ef
```

### 添加新的对象时生成数据库模板

```
dotnet ef migrations add 名称（比如描述什么功能）
```

### 撤回上一次的模板添加

```
dotnet ef migrations remove
```

### 获取模板的完整脚本

```
dotnet ef migrations script
```

### 应用到数据库中去（应用后好像无法撤回）

```
dotnet ef database update
```

## 引用不同的数据库

### Sqlite

```
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

创建一个数据库上下文并继承 DbCommon，在 OnConfiguring 连接数据库时

//读取配置
var dbConnect = ConfigHelp.Get("SQLiteConnect");
var dbTimeout = 10;
//SQLite
optionsBuilder.UseSqlite(dbConnect, p =>
{
    p.CommandTimeout(dbTimeout);
});

连接字符串示例值
//data source=zgcwkj_db.db
```

### SqlServer

```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

创建一个数据库上下文并继承 DbCommon，在 OnConfiguring 连接数据库时

//读取配置
var dbConnect = ConfigHelp.Get("SqlServerConnect");
var dbTimeout = 10;
//SqlServer
optionsBuilder.UseSqlServer(dbConnect, p =>
{
    p.CommandTimeout(dbTimeout);
});

连接字符串示例值
//server=127.0.0.1,1433;uid=sa;pwd=root;database=zgcwkj_db;MultipleActiveResultSets=true;
```

### PostgreSql

```
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

创建一个数据库上下文并继承 DbCommon，在 OnConfiguring 连接数据库时

//读取配置
var dbConnect = ConfigHelp.Get("PgSqlConnect");
var dbTimeout = 10;
//PostgreSql
optionsBuilder.UseNpgsql(dbConnect, p =>
{
    p.CommandTimeout(dbTimeout);
    //指定数据库版本
    if (dbParameters.ContainsKey("version"))
    {
        p.SetPostgresVersion(dbParameters["version"].ToInt(), 0);
    }
    //时间兼容（旧的时间行为）
    if (dbParameters.ContainsKey("olddatetimebehavior"))
    {
        var oldDateTimeBehavior = dbParameters["olddatetimebehavior"].ToBool();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", oldDateTimeBehavior);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", oldDateTimeBehavior);
    }
});

连接字符串示例值
//server=127.0.0.1;port=5432;username=postgres;password=root;database=zgcwkj_db;version=13;
```

### MySql

```
dotnet add package Pomelo.EntityFrameworkCore.MySql

创建一个数据库上下文并继承 DbCommon，在 OnConfiguring 连接数据库时

//读取配置
var dbConnect = ConfigHelp.Get("MySqlConnect");
var dbTimeout = 10;
//MySql
optionsBuilder.UseMySql(dbConnect, ServerVersion.AutoDetect(dbConnect), p =>
{
    p.CommandTimeout(dbTimeout);
});

连接字符串示例值
//server=127.0.0.1;port=3306;username=root;password=root;database=zgcwkj_db;charset=utf8;Pooling=true;
```
