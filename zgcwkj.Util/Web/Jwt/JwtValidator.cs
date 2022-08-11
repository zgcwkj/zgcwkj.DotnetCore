namespace zgcwkj.Util
{
    /// <summary>
    /// Jwt 验证
    /// </summary>
    public abstract class JwtValidator
    {
        /// <summary>
        /// 账号
        /// </summary>
        public abstract string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public abstract string Password { get; set; }

        /// <summary>
        /// 登录时验证
        /// </summary>
        /// <returns>结果</returns>
        public abstract bool Validate();

        /// <summary>
        /// 验证登录
        /// </summary>
        /// <returns>结果</returns>
        public abstract bool Validate(string Account, string Password);
    }
}
