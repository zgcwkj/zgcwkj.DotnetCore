namespace zgcwkj.Util.Interface
{
    /// <summary>
    /// 日志工具抽象
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 信息输出
        /// </summary>
        /// <param name="messages">消息</param>
        /// <returns></returns>
        public bool Info(params string[] messages);

        /// <summary>
        /// 调试输出
        /// </summary>
        /// <param name="messages">消息</param>
        /// <returns></returns>
        public bool Debug(params string[] messages);

        /// <summary>
        /// 错误输出
        /// </summary>
        /// <param name="messages">消息</param>
        /// <returns></returns>
        public bool Error(params string[] messages);

        /// <summary>
        /// 致命输出
        /// </summary>
        /// <param name="messages">消息</param>
        /// <returns></returns>
        public bool Fatal(params string[] messages);

        /// <summary>
        /// 其它输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="logType">类型</param>
        /// <returns></returns>
        public bool Other(string message, string logType);
    }
}