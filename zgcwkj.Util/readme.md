# zgcwkj.Util Help

> Config

```
{
  "DbType": "PostgreSql", //SqlServer MySql PostgreSql
  "MssqlConnect": "server=127.0.0.1;port=1433;username=sa;password=123456;database=dbname;", //SqlServer
  "MysqlConnect": "server=127.0.0.1;port=3306;username=root;password=root;database=dbname;charset=utf8;Pooling=true;", //MySql
  "PgsqlConnect": "server=127.0.0.1;port=5432;username=postgres;password=root;database=dbname;", //PostgreSql integrated security=false;
  "CacheType": "Redis", //Redis Memory
  "RedisConnect": "127.0.0.1" //Redis
}

```

> Code
```
//Load Config
var cBuilder = new ConfigurationBuilder();
var icBuilder = cBuilder.SetBasePath(Directory.GetCurrentDirectory());
var builder = icBuilder.AddJsonFile("appsettings.json");
var config = builder.Build();
GlobalContext.Configuration = config;

//Insert
TableModel table = new TableModel();
table.ID = GlobalConstant.Guid;
table.Name = "Name";
table.Title = "Title";
table.Like = "Like";
var data = DbFactory.DB.Insert(table);

//Update Data
var dbParameters = new List<DbParameter>();
dbParameters.Add(DbParameterExtension.CreateDbParameter("@id", "iddata"));
var data = DbFactory.DB.ExecuteSqlRaw("delete from table where id=@id", dbParameters.ToArray());

//Update Data 2
var dbParameters = new List<DbParameter>();
dbParameters.Add(DbParameterExtension.CreateDbParameter("@id", "2"));
var data = DbFactory.DB.FindTable("select * from table where 1 = @id", dbParameters);

//Update Data 3
TableModel table = new TableModel();
table.ID = GlobalConstant.Guid;
table.Name = "Name";
table.Title = "Title";
table.Like = "Like";
var data3 = DbFactory.DB.Update(table);

//Delete
var expression = LinqExtension.True<TableModel>();
expression = expression.And(T ⇒ T.id == "iddata");
var data1 = DbFactory.DB.Delete(expression);
var data2 = DbFactory.DB.Delete<TableModel>(T ⇒ T.id == "iddata");

//Query
var linq = DbFactory.DB.QuerTable<TableModel>(T ⇒ T.id == "iddata");
var data = linq.ToList();
```