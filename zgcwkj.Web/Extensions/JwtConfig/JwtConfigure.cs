using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using zgcwkj.Util;

namespace zgcwkj.Web.Extensions
{
    /// <summary>
    /// Jwt 配置
    /// </summary>
    public class JwtConfigure : IConfigureNamedOptions<JwtBearerOptions>
    {
        /// <summary>
        /// IConfiguration
        /// </summary>
        private IConfiguration _IConfig { get; }

        /// <summary>
        /// IServiceScopeFactory
        /// </summary>
        private IServiceScopeFactory _IServiceScopeFactory { get; }

        /// <summary>
        /// Jwt 配置
        /// </summary>
        public JwtConfigure(
            IConfiguration config,
            IServiceScopeFactory serviceScopeFactory)
        {
            this._IConfig = config;
            this._IServiceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// 配置
        /// </summary>
        public void Configure(JwtBearerOptions options)
        {
            var tokenConfig = new JwtConfigureModel(_IConfig);
            //验证
            options.TokenValidationParameters = new()
            {
                //验证时间
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(tokenConfig.TimeOut),//TimeSpan.Zero
                //验证 Issuer
                ValidateIssuer = true,
                ValidIssuer = tokenConfig.Issuer,
                //验证 SigningKey
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.SecureKey)),
                //验证 Audience
                ValidateAudience = true,
                ValidAudience = tokenConfig.Audience,
                AudienceValidator = (m, n, z) => ValidatorUser(m, n, z),
            };
            //事件
            options.Events = new()
            {
                //OnMessageReceived = (context) =>
                //{
                //    context.Token = context.Request.Headers["Authorization"];
                //    return Task.CompletedTask;
                //},
                OnChallenge = (context) =>
                {
                    context.HandleResponse();
                    //
                    var responseResult = new
                    {
                        code = (int)HttpStatusCode.Unauthorized,
                        status = false,
                        message = "无权访问！"
                    };
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.WriteAsync(responseResult.ToJson());
                    return Task.FromResult(0);
                }
            };
            //
            options.SaveToken = true;
            //options.RequireHttpsMetadata = false;
        }

        /// <summary>
        /// 配置
        /// </summary>
        public void Configure(string name, JwtBearerOptions options)
        {
            Configure(options);
        }

        /// <summary>
        /// 生成 JWT Token
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="name">用户名</param>
        /// <returns></returns>
        public string GenerateToken(string id, string name)
        {
            var tokenConfig = new JwtConfigureModel(_IConfig);
            //生成对称密钥
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.SecureKey));
            //生成签名证书
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //存储 Token 数据
            var claim = new List<Claim>
            {
                new("userID", id),
                new("userName", name),
                new("dateTime", DateTime.Now.ToString()),
            };
            //生成令牌
            var securityToken = new JwtSecurityToken(
                issuer: tokenConfig.Issuer,
                audience: tokenConfig.Audience,
                claims: claim,
                signingCredentials: signingCredentials,
                expires: DateTime.Now.AddSeconds(tokenConfig.TimeOut)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        /// <summary>
        /// 验证
        /// </summary>
        private bool ValidatorUser(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            using var scope = _IServiceScopeFactory.CreateScope();
            var _userSession = scope.ServiceProvider.GetRequiredService<UserSession>();
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            //获取 Token 数据
            var claims = jwtSecurityToken?.Claims.ToList();
            var claimsDic = new Dictionary<string, string>();
            foreach (var item in claims ?? new())
            {
                claimsDic.Add(item.Type, item.Value);
            }
            //检查数据
            if (!claimsDic.ContainsKey("userID")) return false;
            if (!claimsDic.ContainsKey("userName")) return false;
            if (!claimsDic.ContainsKey("dateTime")) return false;
            //用户数据
            _userSession.SetData(
                claimsDic["userID"],
                claimsDic["userName"],
                claimsDic["dateTime"].ToDate());
            //
            return true;
        }

        /// <summary>
        /// Jwt 配置对象
        /// </summary>
        internal class JwtConfigureModel
        {
            /// <summary>
            /// Issuer
            /// </summary>
            public string Issuer { get; }

            /// <summary>
            /// SecureKey
            /// </summary>
            public string SecureKey { get; }

            /// <summary>
            /// Audience
            /// </summary>
            public string Audience { get; }

            /// <summary>
            /// TimeOut
            /// </summary>
            public int TimeOut { get; }

            /// <summary>
            /// Jwt 配置对象
            /// </summary>
            public JwtConfigureModel(IConfiguration config)
            {
                this.Issuer = config.GetValue<string>("JwtConfig:Issuer");
                this.SecureKey = config.GetValue<string>("JwtConfig:SecureKey");
                this.Audience = config.GetValue<string>("JwtConfig:Audience");
                this.TimeOut = config.GetValue<int>("JwtConfig:TimeOut");
            }
        }
    }
}
