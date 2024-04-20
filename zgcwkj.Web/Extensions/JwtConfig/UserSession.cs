using System.IdentityModel.Tokens.Jwt;

namespace zgcwkj.Web.Extensions
{
    /// <summary>
    /// 用户会话
    /// </summary>
    public class UserSession
    {
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

        //==>

        /// <summary>
        /// HttpContext 存取器
        /// </summary>
        private IHttpContextAccessor _IHttpContextAccessor { get; }

        /// <summary>
        /// 用户会话
        /// </summary>
        public UserSession(IHttpContextAccessor httpContextAccessor)
        {
            this._IHttpContextAccessor = httpContextAccessor;
            //获取请求数据
            if (httpContextAccessor.HttpContext != null)
            {
                var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].To<string>();
                token = token.Replace("Bearer", "").To<string>().Trim();
                if (string.IsNullOrEmpty(token)) return;
                //登录信息
                var jwtSecurityToken = new JwtSecurityTokenHandler();
                var jwtSecurity = jwtSecurityToken.ReadJwtToken(token);
                var claims = jwtSecurity?.Claims;
                if (claims == null) return;
                var userID = claims.Where(w => w.Type == "userID").First().Value.To<string>();
                var userName = claims.Where(w => w.Type == "userName").First().Value.To<string>();
                var dateTime = claims.Where(w => w.Type == "dateTime").First().Value.To<DateTime>();
                ////比对用户信息
                //var userData = GetUserInfo(userID);
                //if (userData == null || userData.Guid == Guid.Empty) return;
                //用户信息
                this.UserID = userID;
                this.UserName = userName;
                this.LoginTime = dateTime;
            }
        }
    }
}
