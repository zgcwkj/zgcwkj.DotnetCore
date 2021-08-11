using System;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using zgcwkj.Util.Common;
using zgcwkj.Util.Log;

namespace zgcwkj.Util
{
    /// <summary>
    /// 数据转换拓展包
    /// </summary>
    public static class Conversion
    {
        /// <summary>
        /// 转换成 Int 类型
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="def">失败值</param>
        /// <returns></returns>
        public static int ToInt(this object value, int def = 0)
        {
            if (!value.IsNull())
            {
                try
                {
                    return Convert.ToInt32(value);
                }
                catch (Exception ex)
                {
                    string meg = ex.Message;
                    if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                }
            }
            return def;
        }

        /// <summary>
        /// 转换成 Double 类型
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="def">失败值</param>
        /// <returns></returns>
        public static double ToDouble(this object value, double def = 0)
        {
            if (!value.IsNull())
            {
                try
                {
                    return Convert.ToDouble(value);
                }
                catch (Exception ex)
                {
                    string meg = ex.Message;
                    if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                }
            }
            return def;
        }

        /// <summary>
        /// 截断 Double 类型的小数位
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="length">保留长度</param>
        /// <returns></returns>
        public static double ToTruncate(this double value, int length = 2)
        {
            var pow = Math.Pow(10, length);
            return Math.Truncate(value * pow) / pow;
        }

        /// <summary>
        /// 截断 Float 类型的小数位
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="length">保留长度</param>
        /// <returns></returns>
        public static double ToTruncate(this float value, int length = 2)
        {
            var pow = Math.Pow(10, length);
            return Math.Truncate(value * pow) / pow;
        }

        /// <summary>
        /// 转换成字符串类型
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="def">失败值</param>
        /// <returns></returns>
        public static string ToStr(this object value, string def = "")
        {
            if (!value.IsNull())
            {
                try
                {
                    return Convert.ToString(value);
                }
                catch (Exception ex)
                {
                    string meg = ex.Message;
                    if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                }
            }
            return def;
        }

        /// <summary>
        /// 转换成字符串类型
        /// 去掉前后的空格
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="def">失败值</param>
        /// <returns></returns>
        public static string ToTrim(this object value, string def = "")
        {
            if (!value.IsNull())
            {
                try
                {
                    return value.ToStr().Trim();
                }
                catch (Exception ex)
                {
                    string meg = ex.Message;
                    if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                }
            }
            return def;
        }

        /// <summary>
        /// 转换成字符串类型
        /// 去掉前面的空格
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="def">失败值</param>
        /// <returns></returns>
        public static string ToTrimStart(this object value, string def = "")
        {
            if (!value.IsNull())
            {
                try
                {
                    return value.ToStr().TrimStart();
                }
                catch (Exception ex)
                {
                    string meg = ex.Message;
                    if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                }
            }
            return def;
        }

        /// <summary>
        /// 转换成字符串类型
        /// 去掉后面的空格
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="def">失败值</param>
        /// <returns></returns>
        public static string ToTrimEnd(this object value, string def = "")
        {
            if (!value.IsNull())
            {
                try
                {
                    return value.ToStr().TrimEnd();
                }
                catch (Exception ex)
                {
                    string meg = ex.Message;
                    if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                }
            }
            return def;
        }

        /// <summary>
        /// 转换成布尔类型
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="def">失败值</param>
        /// <returns></returns>
        public static bool ToBool(this object value, bool def = false)
        {
            if (!value.IsNull())
            {
                try
                {
                    return Convert.ToBoolean(value);
                }
                catch (Exception ex)
                {
                    string meg = ex.Message;
                    if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                }
            }
            return def;
        }

        /// <summary>
        /// 转换成时间类型
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="def">失败值</param>
        /// <returns></returns>
        public static DateTime ToDate(this object value, DateTime def = default)
        {
            try
            {
                return Convert.ToDateTime(value);
            }
            catch (Exception ex)
            {
                string meg = ex.Message;
                if (GlobalConstant.IsDevelopment) Logger.Error(meg);
            }
            return def;
        }

        /// <summary>
        /// 转换成时间类型并格式化
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="dateMode">时间格式</param>
        /// <param name="def">失败值</param>
        /// <returns></returns>
        public static string ToDate(this object value, string dateMode, string def = "")
        {
            try
            {
                DateTime dateTime = value.ToDate();
                return dateTime.ToString(dateMode);
            }
            catch (Exception ex)
            {
                string meg = ex.Message;
                if (GlobalConstant.IsDevelopment) Logger.Error(meg);
            }
            return def;
        }

        /// <summary>
        /// 格式化日期时间
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <param name="dateMode">自定义格式</param>
        /// <returns>时间字符</returns>
        public static string ToStr(this DateTime dateTime, string dateMode)
        {
            return dateMode switch
            {
                "0" => dateTime.ToString("yyyy-MM-dd"),
                "1" => dateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                "2" => dateTime.ToString("yyyy/MM/dd"),
                "3" => dateTime.ToString("yyyy年MM月dd日"),
                "4" => dateTime.ToString("MM-dd"),
                "5" => dateTime.ToString("MM/dd"),
                "6" => dateTime.ToString("MM月dd日"),
                "7" => dateTime.ToString("yyyy-MM"),
                "8" => dateTime.ToString("yyyy/MM"),
                "9" => dateTime.ToString("yyyy年MM月"),
                _ => dateTime.ToString(dateMode),
            };
        }

        /// <summary>
        /// 时间戳转时间
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns></returns>
        public static DateTime ToUnixByDate(this double timeStamp)
        {
            DateTime nowTime;
            if (timeStamp.ToString().Length == 13)
            {
                nowTime = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(timeStamp);
            }
            else
            {
                nowTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timeStamp);
            }
            return nowTime;
        }

        /// <summary>
        /// 时间转时间戳
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <returns></returns>
        public static double ToDateByUnix(this DateTime dateTime)
        {
            TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return timeSpan.TotalSeconds;
        }

        /// <summary>
        /// 时间转时间戳字符
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <returns></returns>
        public static string ToDateByUnixStr(this DateTime dateTime)
        {
            TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Math.Truncate(timeSpan.TotalSeconds).ToStr();
        }

        /// <summary>
        /// 对象转 Json
        /// </summary>
        /// <returns></returns>
        public static string ToJson(this object value)
        {
            if (value.GetType() == typeof(DataTable))
            {
                var dataTable = value as DataTable;
                return JsonSerializer.Serialize(dataTable.ToList());
            }
            return JsonSerializer.Serialize(value);
        }

        /// <summary>
        /// Json 转对象
        /// </summary>
        /// <returns></returns>
        public static T ToJson<T>(this string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }

        /// <summary>
        /// 编码 Base64
        /// </summary>
        /// <param name="value">Data</param>
        /// <returns></returns>
        public static string ToBase64(this string value)
        {
            try
            {
                return Encoding.UTF8.GetBytes(value).ToBase64();
            }
            catch (Exception ex)
            {
                string meg = ex.Message;
                if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                return "";
            }
        }

        /// <summary>
        /// 编码 Base64
        /// </summary>
        /// <param name="value">Data</param>
        /// <returns></returns>
        public static string ToBase64(this byte[] value)
        {
            try
            {
                return Convert.ToBase64String(value);
            }
            catch (Exception ex)
            {
                string meg = ex.Message;
                if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                return "";
            }
        }

        /// <summary>
        /// 解码 Base64
        /// </summary>
        /// <param name="value">Base64</param>
        /// <returns></returns>
        public static string ToUnBase64(this string value)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(value));
            }
            catch (Exception ex)
            {
                string meg = ex.Message;
                if (GlobalConstant.IsDevelopment) Logger.Error(meg);
                return "";
            }
        }

        /// <summary>
        /// 获取 MD5
        /// </summary>
        /// <returns></returns>
        public static string ToMD5(this string value)
        {
            return MD5Tool.GetMd5(value);
        }

        /// <summary>
        /// DataTable 转换成 List
        /// </summary>
        /// <param name="dataTable">需要转换的 DataTable</param>
        /// <returns>转换后的 List</returns>
        public static List<Dictionary<string, object>> ToList(this DataTable dataTable)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    //列名
                    var columnName = dataColumn.ColumnName;
                    //列值
                    var columnValue = dataRow[dataColumn.ColumnName];
                    //列类型
                    var columnType = columnValue.GetType();
                    //处理时间类型格式
                    if (columnType == typeof(DateTime))
                    {
                        dictionary.Add(columnName, columnValue.ToDate("yyyy-MM-dd HH:mm:ss"));
                        continue;
                    }
                    //处理一般
                    dictionary.Add(columnName, columnValue);
                }
                list.Add(dictionary);
            }
            return list;
        }

        /// <summary>
        /// List 转换成 DataTable
        /// </summary>
        /// <param name="list">需要转换的 List</param>
        /// <returns>转换后的 DataTable</returns>
        public static DataTable ToDataTable(this List<Dictionary<string, object>> list)
        {
            DataTable dataTable = new DataTable();
            if (list.Count > 0)
            {
                //==>创建 Table 表头
                foreach (KeyValuePair<string, object> keyValuePair in list[0])
                {
                    dataTable.Columns.Add(keyValuePair.Key, typeof(string));
                }
                //==>创建 Table 数据
                foreach (Dictionary<string, object> dictionary in list)
                {
                    DataRow dr = dataTable.NewRow();
                    foreach (KeyValuePair<string, object> keyValuePair in dictionary)
                    {
                        dr[keyValuePair.Key] = keyValuePair.Value;
                    }
                    dataTable.Rows.Add(dr);
                }
            }
            return dataTable;
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsNull(this object value)
        {
            try
            {
                if (value == null)
                {
                    return true;
                }
                else if (string.IsNullOrEmpty(value.ToString()))
                {
                    return true;
                }
                else if (value.ToString().Length > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string meg = ex.Message;
                if (GlobalConstant.IsDevelopment) Logger.Error(meg);
            }
            return true;
        }

        /// <summary>
        /// 判断是否为空或零
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsNullOrZero(this object value)
        {
            if (IsNull(value))
            {
                return true;
            }
            else if (value.ToStr() == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
