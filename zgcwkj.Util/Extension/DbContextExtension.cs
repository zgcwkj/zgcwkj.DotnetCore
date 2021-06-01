using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using zgcwkj.Util.Common;

namespace zgcwkj.Util.Extension
{
    /// <summary>
    /// 数据库上下文拓展
    /// </summary>
    public static class DbContextExtension
    {
        /// <summary>
        /// 拼接删除SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static string DeleteSql(string tableName)
        {
            StringBuilder strSql = new StringBuilder("DELETE FROM " + tableName + "");
            return strSql.ToString();
        }

        /// <summary>
        /// 拼接删除SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="propertyName">实体属性名称</param>
        /// <param name="propertyValue">字段值：数组1,2,3,4,5,6.....</param>
        /// <returns></returns>
        public static string DeleteSql(string tableName, string propertyName, string propertyValue)
        {
            StringBuilder strSql = new StringBuilder("DELETE FROM " + tableName + " WHERE " + propertyName + " = " + propertyValue + "");
            return strSql.ToString();
        }

        /// <summary>
        /// 拼接批量删除SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="propertyName">实体属性名称</param>
        /// <param name="propertyValue">字段值：数组1,2,3,4,5,6.....</param>
        /// <returns></returns>
        public static string DeleteSql(string tableName, string propertyName, string[] propertyValue)
        {
            string strSql = "DELETE FROM " + tableName + " WHERE " + propertyName + " IN (" + string.Join(",", propertyValue) + ")";
            return strSql.ToString();
        }

        /// <summary>
        /// 获取实体映射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbcontext"></param>
        /// <returns></returns>
        public static IEntityType GetEntityType<T>(DbContext dbcontext) where T : class
        {
            return dbcontext.Model.FindEntityType(typeof(T));
        }

        /// <summary>
        /// 存储过程语句
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="dbParameter">执行命令所需的sql语句对应参数</param>
        /// <returns></returns>
        public static string BuilderProc(string procName, params DbParameter[] dbParameter)
        {
            StringBuilder strSql = new StringBuilder("exec " + procName);
            if (dbParameter != null)
            {
                foreach (var item in dbParameter)
                {
                    strSql.Append(" " + item + ",");
                }
                strSql = strSql.Remove(strSql.Length - 1, 1);
            }
            return strSql.ToString();
        }

        /// <summary>
        /// 设置实体默认值
        /// </summary>
        /// <param name="dbcontext">数据上下文</param>
        public static void SetEntityDefaultValue(DbContext dbcontext)
        {
            //把 null 设置成对应属性类型的默认值
            foreach (EntityEntry entry in dbcontext.ChangeTracker.Entries().Where(p => p.State == EntityState.Added))
            {
                Type type = entry.Entity.GetType();
                PropertyInfo[] props = ReflectionTool.GetProperties(type);
                foreach (PropertyInfo prop in props)
                {
                    object value = prop.GetValue(entry.Entity, null);
                    if (value == null)
                    {
                        string sType = string.Empty;
                        if (prop.PropertyType.GenericTypeArguments.Length > 0)
                        {
                            sType = prop.PropertyType.GenericTypeArguments[0].Name;
                        }
                        else
                        {
                            sType = prop.PropertyType.Name;
                        }
                        switch (sType)
                        {
                            case "Boolean":
                                prop.SetValue(entry.Entity, false);
                                break;
                            case "Char":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "SByte":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "Byte":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "Int16":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "UInt16":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "Int32":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "UInt32":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "Int64":
                                prop.SetValue(entry.Entity, (long)0);
                                break;
                            case "UInt64":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "Single":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "Double":
                                prop.SetValue(entry.Entity, 0);
                                break;
                            case "Decimal":
                                prop.SetValue(entry.Entity, (decimal)0);
                                break;
                            case "DateTime":
                                prop.SetValue(entry.Entity, GlobalConstant.DefaultTime);
                                break;
                            case "String":
                                prop.SetValue(entry.Entity, string.Empty);
                                break;
                            default: break;
                        }
                    }
                    else if (value.ToString() == DateTime.MinValue.ToString())
                    {
                        // sql server datetime类型的的范围不到0001-01-01，所以转成1970-01-01
                        prop.SetValue(entry.Entity, GlobalConstant.DefaultTime);
                    }
                }
            }
        }
    }
}
