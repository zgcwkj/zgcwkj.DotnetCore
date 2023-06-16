using zgcwkj.Util.Enum;

namespace zgcwkj.Util.Log
{
    /// <summary>
    /// 日志工具
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 信息输出
        /// </summary>
        /// <param name="messages">消息</param>
        /// <returns></returns>
        public static bool Info(params string[] messages)
        {
            return LogOutput.OutputLogFile(string.Join(",", messages), LogType.Info);
        }

        /// <summary>
        /// 调试输出
        /// </summary>
        /// <param name="messages">消息</param>
        /// <returns></returns>
        public static bool Debug(params string[] messages)
        {
            return LogOutput.OutputLogFile(string.Join(",", messages), LogType.Debug);
        }

        /// <summary>
        /// 错误输出
        /// </summary>
        /// <param name="messages">消息</param>
        /// <returns></returns>
        public static bool Error(params string[] messages)
        {
            return LogOutput.OutputLogFile(string.Join(",", messages), LogType.Error);
        }

        /// <summary>
        /// 致命输出
        /// </summary>
        /// <param name="messages">消息</param>
        /// <returns></returns>
        public static bool Fatal(params string[] messages)
        {
            return LogOutput.OutputLogFile(string.Join(",", messages), LogType.Fatal);
        }

        /// <summary>
        /// 其它输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="logType">类型</param>
        /// <returns></returns>
        public static bool Other(string message, string logType = "Log")
        {
            return LogOutput.OutputLogFile(message, logType);
        }
    }
}
