namespace zgcwkj.Web.Extensions
{
    /// <summary>
    /// 用户会话
    /// </summary>
    public class UserSession
    {
        /// <summary>
        /// 用户会话
        /// </summary>
        public UserSession()
        {
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string? UserID { get; private set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string? UserName { get; private set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; private set; }

        /// <summary>
        /// 设置数据
        /// </summary>
        public bool SetData(string userID, string userName, DateTime loginTime)
        {
            this.UserID = userID;
            this.UserName = userName;
            this.LoginTime = loginTime;
            return true;
        }
    }
}
