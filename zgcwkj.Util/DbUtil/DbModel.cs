using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using zgcwkj.Util.Common;

namespace zgcwkj.Util.DbUtil
{
    /// <summary>
    /// 数据对象
    /// </summary>
    public class DbModel
    {
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        public bool LoadData()
        {
            Type type = this.GetType();
            string tableName = GetTableName(type);
            var data = GetTableData(type);
            string[] columns = data.keyColumns.ToArray();
            object[] values = data.keyValues.ToArray();
            StringBuilder sql = new StringBuilder();
            sql.Append($"select * from {tableName} where ");
            for (int i = 0; i < columns.Length; i++)
            {
                sql.Append($" {columns[i]} = '{values[i]}' ");
                if (i != columns.Length - 1) sql.Append($" and ");
            }
            var cmd = DbProvider.CreateCommand();
            cmd.SetCommandText(sql.ToString());
            var dataRow = DbAccess.QueryDataRow(cmd);
            if (dataRow.IsNull()) return false;
            //赋值
            PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                foreach (var attribute in property.GetCustomAttributes())
                {
                    var columnAttribute = attribute as ColumnAttribute;//字段名称
                    if (!columnAttribute.IsNull())
                    {
                        propertyName = columnAttribute.Name;
                    }
                }
                if (!dataRow[propertyName].IsNull())
                {
                    property.SetValue(this, dataRow[propertyName]);
                }
            }
            return true;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns></returns>
        public bool Insert()
        {
            Type type = this.GetType();
            string tableName = GetTableName(type);
            var data = GetTableData(type);
            string columns = string.Join(",", data.notkeyColumns);
            object[] values = data.notkeyValues.ToArray();
            string keyColumns = string.Join(",", data.keyColumns);
            object[] keyValues = data.keyValues.ToArray();
            StringBuilder sql = new StringBuilder();
            //主键赋值状态
            var cmd = DbProvider.CreateCommand();
            if (!keyColumns.IsNull())
            {
                object[] kvValues = new object[keyValues.Length + values.Length];
                keyValues.CopyTo(kvValues, 0);
                values.CopyTo(kvValues, keyValues.Length);
                sql.Append($"insert into {tableName} ({keyColumns},{columns}) values(@{keyColumns.Replace(",", ",@")},@{columns.Replace(",", ",@")})");
                cmd.SetCommandText(sql.ToString(), kvValues);
                int count = DbAccess.UpdateData(cmd);
                return count > 0;
            }
            else
            {
                sql.Append($"insert into {tableName} ({columns}) values(@{columns.Replace(",", ",@")})");
                cmd.SetCommandText(sql.ToString(), values);
                int count = DbAccess.UpdateData(cmd);
                return count > 0;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            Type type = this.GetType();
            string tableName = GetTableName(type);
            var data = GetTableData(type);
            StringBuilder sql = new StringBuilder();
            string[] columns = data.notkeyColumns.ToArray();
            object[] values = data.notkeyValues.ToArray();
            sql.Append($"update {tableName} set ");
            for (int i = 0; i < columns.Length; i++)
            {
                sql.Append($" {columns[i]} = @{columns[i]} ");
                if (i != columns.Length - 1) sql.Append($",");
            }
            string[] keyColumns = data.keyColumns.ToArray();
            object[] keyValues = data.keyValues.ToArray();
            sql.Append($" where ");
            for (int i = 0; i < keyColumns.Length; i++)
            {
                sql.Append($" {keyColumns[i]} = @{keyColumns[i]} ");
                if (i != keyColumns.Length - 1) sql.Append($" and ");
            }
            var cmd = DbProvider.CreateCommand();
            cmd.SetCommandText(sql.ToString(), values.Concat(keyValues).ToArray());
            int count = DbAccess.UpdateData(cmd);
            return count > 0;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            Type type = this.GetType();
            string tableName = GetTableName(type);
            var data = GetTableData(type);
            string[] columns = data.keyColumns.ToArray();
            object[] values = data.keyValues.ToArray();
            StringBuilder sql = new StringBuilder();
            sql.Append($"delete from {tableName} where ");
            for (int i = 0; i < columns.Length; i++)
            {
                sql.Append($" {columns[i]} = @{columns[i]} ");
                if (i != columns.Length - 1) sql.Append($" and ");
            }
            var cmd = DbProvider.CreateCommand();
            cmd.SetCommandText(sql.ToString(), values);
            int count = DbAccess.UpdateData(cmd);
            return count > 0;
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string GetTableName(Type type)
        {
            string tableName = type.Name;
            foreach (var attribute in type.GetCustomAttributes())
            {
                var tableAttribute = attribute as TableAttribute;//表
                tableName = tableAttribute.Name;
            }
            return tableName;
        }

        /// <summary>
        /// 获取表架构
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string GetTableSchema(Type type)
        {
            string tableSchema = "dbo";
            foreach (var attribute in type.GetCustomAttributes())
            {
                var tableAttribute = attribute as TableAttribute;//表
                tableSchema = tableAttribute.Schema;
            }
            return tableSchema;
        }

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private dynamic GetTableData(Type type)
        {
            List<TableMode> tableModes = GetTableObject(type);
            var keyData = tableModes.Where(T => T.IsKey == true && T.Value != default);
            var keyColumns = keyData.Select(T => T.Column).ToList();
            var keyValues = keyData.Select(T => T.Value).ToList();
            keyValues = DataFormat(keyValues);
            var notkeyData = tableModes.Where(T => T.IsKey == false && T.Value != default);
            var notkeyColumns = notkeyData.Select(T => T.Column).ToList();
            var notkeyValues = notkeyData.Select(T => T.Value).ToList();
            notkeyValues = DataFormat(notkeyValues);
            return new
            {
                tableModes,
                keyColumns,
                keyValues,
                notkeyColumns,
                notkeyValues,
            };
        }

        /// <summary>
        /// 获取表对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private List<TableMode> GetTableObject(Type type)
        {
            List<TableMode> tableModes = new List<TableMode>();
            foreach (var property in type.GetProperties())
            {
                TableMode tableMode = new TableMode();
                tableMode.Column = property.Name;
                foreach (var attribute in property.GetCustomAttributes())
                {
                    var keyAttribute = attribute as KeyAttribute;//是否主键
                    if (!keyAttribute.IsNull())
                    {
                        tableMode.IsKey = true;
                    }
                    var columnAttribute = attribute as ColumnAttribute;//字段名称
                    if (!columnAttribute.IsNull())
                    {
                        tableMode.Column = columnAttribute.Name;
                    }
                }
                tableMode.Value = property.GetValue(this);
                tableModes.Add(tableMode);
            }
            return tableModes;
        }

        /// <summary>
        /// 数据格式化
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public List<object> DataFormat(List<object> list)
        {
            var linq = list.Select(T =>
                T.GetType() == typeof(string) ? T.ToTrim() :
                T.GetType() == typeof(int) ? T.ToInt() :
                T.GetType() == typeof(DateTime) ? T.ToDate("yyyy-MM-dd HH:mm:ss") :
                T
            );
            return linq.ToList();
        }
    }

    /// <summary>
    /// 表对象
    /// </summary>
    partial class TableMode
    {
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type
        {
            get
            {
                return Value.GetType().Name;
            }
        }

        /// <summary>
        /// 字段数据
        /// </summary>
        public string Data
        {
            get
            {
                if (Value.GetType() == typeof(string))
                {
                    return $"'{Value}'";
                }
                else
                {
                    return $"{Value}";
                }
            }
        }
    }
}