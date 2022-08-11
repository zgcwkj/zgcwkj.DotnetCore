using zgcwkj.Util;

namespace zgcwkj.Web.Comm
{
    /// <summary>
    /// Jwt 验证
    /// </summary>
    public class MyJwtValidator : JwtValidator
    {
        /// <summary>
        /// 账号
        /// </summary>
        public override string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public override string Password { get; set; }

        /// <summary>
        /// 登录时验证
        /// </summary>
        /// <returns>结果</returns>
        public override bool Validate()
        {
            if (Account == Password)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证登录
        /// </summary>
        /// <returns>结果</returns>
        public override bool Validate(string Account, string Password)
        {
            if (Account == Password)
            {
                return true;
            }
            return false;
        }
    }
}
