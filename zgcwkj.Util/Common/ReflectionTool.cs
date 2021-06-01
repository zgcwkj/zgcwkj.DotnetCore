using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace zgcwkj.Util.Common
{
    /// <summary>
    /// 反射工具
    /// </summary>
    public class ReflectionTool
    {
        /// <summary>
        /// 同步字典
        /// </summary>
        private static ConcurrentDictionary<string, object> dictCache = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 得到类里面的属性集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type, string[] columns = null)
        {
            PropertyInfo[] properties = null;
            if (dictCache.ContainsKey(type.FullName))
            {
                properties = dictCache[type.FullName] as PropertyInfo[];
            }
            else
            {
                properties = type.GetProperties();
                dictCache.TryAdd(type.FullName, properties);
            }

            if (columns != null && columns.Length > 0)
            {
                //按columns顺序返回属性
                var columnPropertyList = new List<PropertyInfo>();
                foreach (var column in columns)
                {
                    var columnProperty = properties.Where(p => p.Name == column).FirstOrDefault();
                    if (columnProperty != null)
                    {
                        columnPropertyList.Add(columnProperty);
                    }
                }
                return columnPropertyList.ToArray();
            }
            else
            {
                return properties;
            }
        }
    }
}
