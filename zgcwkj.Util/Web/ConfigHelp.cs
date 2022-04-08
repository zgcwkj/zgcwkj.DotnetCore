using System;
using Microsoft.Extensions.Configuration;
using zgcwkj.Util.Log;
using zgcwkj.Util.Common;

namespace zgcwkj.Util
{
    /// <summary>
    /// Config 帮助
    /// </summary>
    public class ConfigHelp
    {
        /// <summary>
        /// 获取 Config 对象
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetObj()
        {
            return GlobalContext.Configuration;
        }

        /// <summary>
        /// 读取 Config
        /// </summary>
        /// <param name="sName">名称</param>
        /// <returns>值</returns>
        public static T Get<T>(string sName)
        {
            try
            {
                var config = GlobalContext.Configuration;
                if (string.IsNullOrEmpty(sName)) return default;
                var configValue = config.GetValue<T>(sName);
                if (configValue.IsNull())
                {
                    var sectionValue = config.GetSection(sName);
                    configValue = (T)sectionValue.Get(typeof(T));
                }
                return configValue;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                //Logger.Error($"获取配置异常：{message}");
                return default;
            }
        }

        /// <summary>
        /// 读取 Config
        /// </summary>
        /// <param name="sName">名称</param>
        /// <returns>值</returns>
        public static string Get(string sName)
        {
            var configValue = Get<string>(sName);
            if (string.IsNullOrEmpty(configValue)) return "";
            return configValue;
        }
    }
}
