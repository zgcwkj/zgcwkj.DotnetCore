using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using zgcwkj.Util.Common;

namespace zgcwkj.Util
{
    /// <summary>
    /// 全局常数
    /// </summary>
    public class GlobalConstant
    {
        /// <summary>
        /// 是否正式环境
        /// </summary>
        public static bool IsProduction
        {
            get
            {
                var isDevelopment = GlobalContext.HostingEnvironment?.IsProduction();
                return isDevelopment.ToBool();
            }
        }

        /// <summary>
        /// 是否开发环境
        /// </summary>
        public static bool IsDevelopment
        {
            get
            {
                var isDevelopment = GlobalContext.HostingEnvironment?.IsDevelopment();
                return isDevelopment.ToBool();
            }
        }

        /// <summary>
        /// 程序运行路径
        /// </summary>
        public static string GetRunPath
        {
            get
            {
                string filePath = Path.GetDirectoryName(typeof(GlobalConstant).Assembly.Location);
                return filePath;
            }
        }

        /// <summary>
        /// 当前时间戳
        /// </summary>
        public static string TimeStamp
        {
            get
            {
                return DateTime.Now.ToDateByUnix().ToTruncate(0).ToTrim();
            }
        }

        /// <summary>
        /// 获取 GUID
        /// </summary>
        public static string Guid
        {
            get
            {
                Guid guid = System.Guid.NewGuid();
                string md5Guid = Common.MD5Tool.GetMd5(guid.ToString());
                return md5Guid;
            }
        }
    }
}
