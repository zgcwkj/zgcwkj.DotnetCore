using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            return OutputLogFile(message, "Info");
        }

        /// <summary>
        /// 调试输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static bool Debug(string message)
        {
            return OutputLogFile(message, "Debug");
        }

        /// <summary>
        /// 错误输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static bool Error(string message)
        {
            return OutputLogFile(message, "Error");
        }

        /// <summary>
        /// 致命输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static bool Fatal(string message)
        {
            return OutputLogFile(message, "Fatal");
        }

        /// <summary>
        /// 输出日志文件
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private static bool OutputLogFile(string message, string type = "Log")
        {
            int index = 0;
            //文件夹路径
            string filePath = Directory.GetCurrentDirectory();
            filePath = $"{filePath}/Log/";
            //创建文件夹，防止文件夹没有
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            //文件名称
            string fileName = $"{DateTime.Now:yyyyMMdd}_{type}.txt";
            //输出错误状态
            bool errorOK = false;
            do
            {
                try
                {
                    //文件内容
                    string fileContent = $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n{message}\r\n\r\n";
                    File.AppendAllText($"{filePath}/{fileName}", fileContent);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    index++;
                    type += $"{index}";
                    Thread.Sleep(100);
                    errorOK = true;
                }
            } while (errorOK);
            return true;
        }
    }
}