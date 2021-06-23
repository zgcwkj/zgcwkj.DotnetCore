using Microsoft.Extensions.Configuration;

namespace zgcwkj.Util.Web
{
    /// <summary>
    /// Config 帮助
    /// </summary>
    public class ConfigHelp
    {
        /// <summary>
        /// 读取 Config
        /// </summary>
        /// <param name="sName">名称</param>
        /// <returns>Config值</returns>
        public static T Get<T>(string sName)
        {
            if (string.IsNullOrEmpty(sName)) return default;
            return GlobalContext.Configuration.GetValue<T>(sName);
        }

        /// <summary>
        /// 读取 Config
        /// </summary>
        /// <param name="sName">名称</param>
        /// <returns>Config值</returns>
        public static string Get(string sName)
        {
            var configValue = Get<string>(sName);
            if (string.IsNullOrEmpty(configValue)) return "";
            return configValue;
        }
    }
}
