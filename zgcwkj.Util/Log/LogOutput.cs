using System;
using System.IO;
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
        /// 日志文件大小
        /// </summary>
        internal static int LogFileSize { get; set; } = ConfigHelp.Get("LogFileSize").ToInt();

        /// <summary>
        /// 输出日志目录
        /// </summary>
        /// <returns></returns>
        internal static string OutputLogDirectory()
        {
            //文件夹路径
            string filePath = GlobalConstant.GetRunPath;
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
        /// <param name="logType">类型</param>
        /// <returns></returns>
        internal static bool OutputLogFile(string message, LogType logType = LogType.Info)
        {
            var type = logType.ToString();
            return OutputLogFile(message, type);
        }

        /// <summary>
        /// 输出日志文件
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        internal static bool OutputLogFile(string message, string type = "Log")
        {
            //文件夹路径
            var filePath = OutputLogDirectory();
            //文件名称
            var fileName = $"{DateTime.Now:yyyyMMdd}_{type}.txt";
            //检查日志文件大小
            var fileInfo = new FileInfo($"{filePath}/{fileName}");
            if (LogFileSize != 0 && fileInfo.Exists && fileInfo.Length / (1024 * 1024) > LogFileSize)
            {
                File.Delete($"{filePath}/{fileName}");
            }
            //输出错误状态
            bool errorOK;
            //错误计数
            int index = 0;
            do
            {
                try
                {
                    //文件内容
                    string fileContent = $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n{message}\r\n\r\n";
                    File.AppendAllText($"{filePath}/{fileName}", fileContent);
                    errorOK = false;
                }
                catch (Exception ex)
                {
                    if (GlobalConstant.IsDevelopment) throw ex;
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
