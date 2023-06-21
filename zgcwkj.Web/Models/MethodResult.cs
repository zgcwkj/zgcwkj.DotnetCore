using System.Text.Json.Serialization;

namespace zgcwkj.Web.Models
{
    /// <summary>
    /// 方法结果
    /// </summary>
    public class MethodResult
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        [JsonPropertyName("ErrorCode")]
        public int? ErrorCode { get; set; } = null;

        /// <summary>
        /// 错误消息
        /// </summary>
        [JsonPropertyName("ErrorMessage")]
        public string ErrorMessage { get; set; } = null;

        /// <summary>
        /// 数据
        /// </summary>
        [JsonPropertyName("Data")]
        public object Data { get; set; } = null;

        /// <summary>
        /// 数据数量
        /// </summary>
        [JsonPropertyName("DataCount")]
        public int? DataCount { get; set; } = null;
    }
}
