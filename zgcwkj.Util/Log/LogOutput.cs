using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using zgcwkj.Util.Enum;

namespace zgcwkj.Util.Log
{
    /// <summary>
    /// 日志输出
    /// </summary>
    internal class LogOutput
    {
        /// <summary>
        /// 输出日志目录
        /// </summary>
        /// <returns></returns>
        internal static string OutputLogDirectory()
        {
            //文件夹路径
            string filePath = Directory.GetCurrentDirectory();
            filePath = $"{filePath}/Log/";
            //创建文件夹，防止文件夹没有
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            //文件路径
            return filePath;
        }

        /// <summary>
        /// 输出日志文件
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        internal static bool OutputLogFile(string message, string type = "Log")
        {
            int index = 0;
            //文件夹路径
            string filePath = OutputLogDirectory();
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

        /// <summary>
        /// 输出日志文件
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="logType">类型</param>
        /// <returns></returns>
        internal static bool OutputLogFile(string message, LogType logType = LogType.Info)
        {
            int index = 0;
            string type = logType.ToString();
            //文件夹路径
            string filePath = OutputLogDirectory();
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
