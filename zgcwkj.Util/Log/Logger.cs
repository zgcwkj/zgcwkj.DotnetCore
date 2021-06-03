using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static bool Info(string message)
        {
            return LogOutput.OutputLogFile(message, LogType.Info);
        }

        /// <summary>
        /// 调试输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static bool Debug(string message)
        {
            return LogOutput.OutputLogFile(message, LogType.Debug);
        }

        /// <summary>
        /// 错误输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static bool Error(string message)
        {
            return LogOutput.OutputLogFile(message, LogType.Error);
        }

        /// <summary>
        /// 致命输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static bool Fatal(string message)
        {
            return LogOutput.OutputLogFile(message, LogType.Fatal);
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