using System;
using System.Web;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace zgcwkj.Util.Models
{
    /// <summary>
    /// 方法结果
    /// </summary>
    public class MethodResult
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int? ErrorCode { get; set; } = null;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; } = null;

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; } = null;

        /// <summary>
        /// 数据数量
        /// </summary>
        public int? DataCount { get; set; } = null;
    }
}