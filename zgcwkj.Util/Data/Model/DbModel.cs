﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace zgcwkj.Util
{
    /// <summary>
    /// <b>数据表对象</b>
    ///
    /// <para>继承后可以操作数据库</para>
    /// </summary>
    public abstract class DbModel
    {
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <returns></returns>
        public bool LoadData(DbContext? dbContext = default)
        {
            var type = GetType();
            var tableName = GetTableName(type);
            if (tableName.IsNull()) throw new Exception("No table name (DbModel.LoadData)");
            var data = GetTableData(type);
            string[] columns = data.keyColumns.ToArray();
            if (columns.IsNull()) throw new Exception("Column has no data (DbModel.LoadData)");
            object[] values = data.keyValues.ToArray();
            if (values.Length == 0) throw new Exception("Column has no value (DbModel.LoadData)");
            var sql = new StringBuilder();
            sql.Append($"select * from {tableName} where ");
            for (int i = 0; i < columns.Length; i++)
            {
                sql.Append($" {columns[i]} = '{values[i]}' ");
                if (i != columns.Length - 1) sql.Append($" and ");
            }
            var cmd = DbProvider.Create(dbContext);
            cmd.SetCommandText(sql.ToString());
            var dataRow = cmd.QueryDataRow();
            if (dataRow == null) return false;
            //赋值
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                foreach (var attribute in property.GetCustomAttributes())
                {
                    var columnAttribute = attribute as ColumnAttribute;//字段名称
                    if (columnAttribute != null)
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
        /// 加载数据(异步)
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <returns></returns>
        public async Task<bool> LoadDataAsync(DbContext? dbContext = default)
        {
            var type = GetType();
            var tableName = GetTableName(type);
            if (tableName.IsNull()) throw new Exception("No table name (DbModel.LoadDataAsync)");
            var data = GetTableData(type);
            string[] columns = data.keyColumns.ToArray();
            if (columns.IsNull()) throw new Exception("Column has no data (DbModel.LoadDataAsync)");
            object[] values = data.keyValues.ToArray();
            if (values.Length == 0) throw new Exception("Column has no value (DbModel.LoadDataAsync)");
            var sql = new StringBuilder();
            sql.Append($"select * from {tableName} where ");
            for (int i = 0; i < columns.Length; i++)
            {
                sql.Append($" {columns[i]} = '{values[i]}' ");
                if (i != columns.Length - 1) sql.Append($" and ");
            }
            var cmd = DbProvider.Create(dbContext);
            cmd.SetCommandText(sql.ToString());
            var dataRow = await cmd.QueryDataRowAsync();
            if (dataRow == null) return false;
            //赋值
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                foreach (var attribute in property.GetCustomAttributes())
                {
                    var columnAttribute = attribute as ColumnAttribute;//字段名称
                    if (columnAttribute != null)
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
        /// <param name="dbContext">数据库上下文</param>
        /// <returns></returns>
        public bool Insert(DbContext? dbContext = default)
        {
            var type = GetType();
            var tableName = GetTableName(type);
            if (tableName.IsNull()) throw new Exception("No table name (DbModel.Insert)");
            var data = GetTableData(type);
            string columns = string.Join(",", data.notkeyColumns);
            if (columns.IsNull()) throw new Exception("Column has no data (DbModel.Insert)");
            object[] values = data.notkeyValues.ToArray();
            if (values.Length == 0) throw new Exception("Column has no value (DbModel.Insert)");
            string keyColumns = string.Join(",", data.keyColumns);
            if (keyColumns.IsNull()) throw new Exception("No primary key (DbModel.Insert)");
            object[] keyValues = data.keyValues.ToArray();
            if (keyValues.Length == 0) throw new Exception("No primary value (DbModel.Insert)");
            var sql = new StringBuilder();
            //主键赋值状态
            var cmd = DbProvider.Create(dbContext);
            if (!keyColumns.IsNull())
            {
                var kvValues = new object[keyValues.Length + values.Length];
                keyValues.CopyTo(kvValues, 0);
                values.CopyTo(kvValues, keyValues.Length);
                sql.Append($"insert into {tableName} ({keyColumns},{columns}) values(@{keyColumns.Replace(",", ",@")},@{columns.Replace(",", ",@")})");
                cmd.SetCommandText(sql.ToString(), kvValues);
                var count = cmd.UpdateData();
                return count > 0;
            }
            else
            {
                sql.Append($"insert into {tableName} ({columns}) values(@{columns.Replace(",", ",@")})");
                cmd.SetCommandText(sql.ToString(), values);
                var count = cmd.UpdateData();
                return count > 0;
            }
        }

        /// <summary>
        /// 新增数据(异步)
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(DbContext? dbContext = default)
        {
            var type = GetType();
            var tableName = GetTableName(type);
            if (tableName.IsNull()) throw new Exception("No table name (DbModel.InsertAsync)");
            var data = GetTableData(type);
            string columns = string.Join(",", data.notkeyColumns);
            if (columns.IsNull()) throw new Exception("Column has no data (DbModel.InsertAsync)");
            object[] values = data.notkeyValues.ToArray();
            if (values.Length == 0) throw new Exception("Column has no value (DbModel.InsertAsync)");
            string keyColumns = string.Join(",", data.keyColumns);
            if (keyColumns.IsNull()) throw new Exception("No primary key (DbModel.InsertAsync)");
            object[] keyValues = data.keyValues.ToArray();
            if (keyValues.Length == 0) throw new Exception("No primary keyValue (DbModel.InsertAsync)");
            var sql = new StringBuilder();
            //主键赋值状态
            var cmd = DbProvider.Create(dbContext);
            if (!keyColumns.IsNull())
            {
                var kvValues = new object[keyValues.Length + values.Length];
                keyValues.CopyTo(kvValues, 0);
                values.CopyTo(kvValues, keyValues.Length);
                sql.Append($"insert into {tableName} ({keyColumns},{columns}) values(@{keyColumns.Replace(",", ",@")},@{columns.Replace(",", ",@")})");
                cmd.SetCommandText(sql.ToString(), kvValues);
                var count = cmd.UpdateData();
                return count > 0;
            }
            else
            {
                sql.Append($"insert into {tableName} ({columns}) values(@{columns.Replace(",", ",@")})");
                cmd.SetCommandText(sql.ToString(), values);
                var count = await cmd.UpdateDataAsync();
                return count > 0;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <returns></returns>
        public bool Update(DbContext? dbContext = default)
        {
            var type = GetType();
            var tableName = GetTableName(type);
            if (tableName.IsNull()) throw new Exception("No table name (DbModel.Update)");
            var data = GetTableData(type);
            var sql = new StringBuilder();
            string[] columns = data.notkeyColumns.ToArray();
            if (columns.Length == 0) throw new Exception("Column has no data(DbModel.Update)");
            object[] values = data.notkeyValues.ToArray();
            if (values.Length == 0) throw new Exception("Column has no value (DbModel.Update)");
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
            var cmd = DbProvider.Create(dbContext);
            cmd.SetCommandText(sql.ToString(), values.Concat(keyValues).ToArray());
            var count = cmd.UpdateData();
            return count > 0;
        }

        /// <summary>
        /// 更新数据(异步)
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(DbContext? dbContext = default)
        {
            var type = GetType();
            var tableName = GetTableName(type);
            if (tableName.IsNull()) throw new Exception("No table name (DbModel.UpdateAsync)");
            var data = GetTableData(type);
            var sql = new StringBuilder();
            string[] columns = data.notkeyColumns.ToArray();
            if (columns.Length == 0) throw new Exception("Column has no data(DbModel.UpdateAsync)");
            object[] values = data.notkeyValues.ToArray();
            if (values.Length == 0) throw new Exception("Column has no value (DbModel.UpdateAsync)");
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
            var cmd = DbProvider.Create(dbContext);
            cmd.SetCommandText(sql.ToString(), values.Concat(keyValues).ToArray());
            var count = await cmd.UpdateDataAsync();
            return count > 0;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <returns></returns>
        public bool Delete(DbContext? dbContext = default)
        {
            var type = GetType();
            var tableName = GetTableName(type);
            if (tableName.IsNull()) throw new Exception("No table name (DbModel.Delete)");
            var data = GetTableData(type);
            string[] columns = data.notkeyColumns.ToArray();
            if (columns.Length == 0) throw new Exception("Column has no data(DbModel.Delete)");
            object[] values = data.notkeyValues.ToArray();
            if (values.Length == 0) throw new Exception("Column has no value (DbModel.Delete)");
            var sql = new StringBuilder();
            sql.Append($"delete from {tableName} where ");
            for (int i = 0; i < columns.Length; i++)
            {
                sql.Append($" {columns[i]} = @{columns[i]} ");
                if (i != columns.Length - 1) sql.Append($" and ");
            }
            var cmd = DbProvider.Create(dbContext);
            cmd.SetCommandText(sql.ToString(), values);
            var count = cmd.UpdateData();
            return count > 0;
        }

        /// <summary>
        /// 删除数据(异步)
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(DbContext? dbContext = default)
        {
            var type = GetType();
            var tableName = GetTableName(type);
            if (tableName.IsNull()) throw new Exception("No table name (DbModel.DeleteAsync)");
            var data = GetTableData(type);
            string[] columns = data.notkeyColumns.ToArray();
            if (columns.Length == 0) throw new Exception("Column has no data(DbModel.DeleteAsync)");
            object[] values = data.notkeyValues.ToArray();
            if (values.Length == 0) throw new Exception("Column has no value (DbModel.DeleteAsync)");
            var sql = new StringBuilder();
            sql.Append($"delete from {tableName} where ");
            for (int i = 0; i < columns.Length; i++)
            {
                sql.Append($" {columns[i]} = @{columns[i]} ");
                if (i != columns.Length - 1) sql.Append($" and ");
            }
            var cmd = DbProvider.Create(dbContext);
            cmd.SetCommandText(sql.ToString(), values);
            var count = await cmd.UpdateDataAsync();
            return count > 0;
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private string GetTableName(Type type)
        {
            var tableName = type.Name;
            foreach (var attribute in type.GetCustomAttributes())
            {
                var tableAttribute = attribute as TableAttribute;//表
                tableName = tableAttribute?.Name ?? "";
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
            var tableSchema = "dbo";
            foreach (var attribute in type.GetCustomAttributes())
            {
                var tableAttribute = attribute as TableAttribute;//表
                tableSchema = tableAttribute?.Schema ?? "";
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
            var tableModes = GetTableObject(type);
            var keyData = tableModes.Where(T => T.IsKey == true && T.Value != null);
            var keyColumns = keyData.Select(T => T.Column).ToList();
            var keyValues = keyData.Select(T => T.Value).ToList();
            keyValues = DataFormat(keyValues);
            var notkeyData = tableModes.Where(T => T.IsKey == false && T.Value != null);
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
            var tableModes = new List<TableMode>();
            foreach (var property in type.GetProperties())
            {
                var tableMode = new TableMode();
                tableMode.Column = property.Name;
                foreach (var attribute in property.GetCustomAttributes())
                {
                    var keyAttribute = attribute as KeyAttribute;//主键
                    if (keyAttribute != null)
                    {
                        tableMode.IsKey = true;
                    }
                    var columnAttribute = attribute as ColumnAttribute;//字段
                    if (columnAttribute != null)
                    {
                        tableMode.Column = columnAttribute.Name ?? "";
                    }
                    var notMappedAttribute = attribute as NotMappedAttribute;//排除
                    if (notMappedAttribute != null)
                    {
                        tableMode.NotMapped = true;
                    }
                }
                tableMode.Value = property.GetValue(this) ?? new();
                if (!tableMode.NotMapped) tableModes.Add(tableMode);
            }
            return tableModes;
        }

        /// <summary>
        /// 数据格式化
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns></returns>
        private List<object> DataFormat(List<object> list)
        {
            var linq = list.Select(T =>
                T.GetType() == typeof(string) ? T.To<string>().Trim() :
                T.GetType() == typeof(int) ? T.To<int>() :
                T.GetType() == typeof(DateTime) ? T.ToDate("yyyy-MM-dd HH:mm:ss") :
                T
            );
            return linq.ToList();
        }
    }
}
