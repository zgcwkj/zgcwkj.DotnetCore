using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace zgcwkj.Web.Extensions
{
    /// <summary>
    /// Int64 序列化
    /// </summary>
    public class LongJson : JsonConverter<long>
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader">读</param>
        /// <param name="typeToConvert">类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (long.TryParse(reader.GetString(), out long data)) return data;
            }
            return reader.GetInt64();
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer">写</param>
        /// <param name="value">值</param>
        /// <param name="options">选项</param>
        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            var zhCN = new CultureInfo("zh-CN");//时区
            var setValue = value.ToString(zhCN);
            writer?.WriteStringValue(setValue);
        }
    }
}
