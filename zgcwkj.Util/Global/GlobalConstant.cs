using System;

namespace zgcwkj.Util
{
    /// <summary>
    /// 全局常数
    /// </summary>
    public class GlobalConstant
    {
        /// <summary>
        /// 默认时间
        /// </summary>
        public static DateTime DefaultTime = DateTime.Parse("1970-01-01 00:00:00");

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
