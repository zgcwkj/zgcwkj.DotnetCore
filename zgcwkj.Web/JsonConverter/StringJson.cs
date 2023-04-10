using System.Text.Json;
using System.Text.Json.Serialization;
using zgcwkj.Util;

namespace zgcwkj.Web
{
    /// <summary>
    /// 字符串序列化
    /// </summary>
    public class StringJson : JsonConverter<string>
    {
        /// <summary>
        /// 读取
        /// </summary>
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (str != null)
            {
                return str.ToStr();
            }
            return string.Empty;
        }

        /// <summary>
        /// 写入
        /// </summary>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToStr());
        }
    }
}
