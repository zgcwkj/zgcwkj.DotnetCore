using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using zgcwkj.Util.Common;

namespace zgcwkj.Util
{
    /// <summary>
    /// 配置 JWT
    /// </summary>
    public static class JwtConfig
    {
        /// <summary>
        /// 有效时间
        /// </summary>
        public static TimeSpan ClockSkew { get; set; } = TimeSpan.FromDays(3);

        /// <summary>
        /// 添加 Jwt 服务并配置
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="jwtValidator">Jwt 验证</param>
        public static void AddJwtConfig(this IServiceCollection services, JwtValidator jwtValidator)
        {
            ClockSkew = TimeSpan.FromDays(3);
            services.AddJwtConfig(ClockSkew, jwtValidator);
        }

        /// <summary>
        /// 添加 Jwt 服务并配置
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="clockSkew">有效时间</param>
        /// <param name="jwtValidator">Jwt 验证</param>
        public static void AddJwtConfig(this IServiceCollection services, TimeSpan clockSkew, JwtValidator jwtValidator)
        {
            var tokenConfig = ConfigHelp.Get<JwtConfigData>("JwtConfig");
            services.AddAuthentication(options =>//添加JWT Scheme
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>//添加 Jwt 验证
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,//是否验证时间
                    ClockSkew = clockSkew,//有效时间

                    ValidateIssuer = true,//是否验证Issuer
                    ValidIssuer = tokenConfig.Issuer,

                    ValidateAudience = true,//是否验证Audience
                    //ValidAudience = tokenConfig.Audience,
                    AudienceValidator = (m, n, z) => ValidatorUser(m, n, z, jwtValidator),

                    IssuerSigningKey = GetSymmetric()//拿到SecurityKey
                };
            });
        }

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="audiences">audiences</param>
        /// <param name="securityToken">securityToken</param>
        /// <param name="validationParameters">validationParameters</param>
        /// <param name="jwtValidator">Jwt 验证</param>
        /// <returns></returns>
        private static bool ValidatorUser(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters, JwtValidator jwtValidator)
        {
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            try
            {
                //账号
                var aClaim = jwtSecurityToken.Claims.Where(T => T.Type == "account");
                var account = aClaim.FirstOrDefault().Value;
                //密码
                var pClaim = jwtSecurityToken.Claims.Where(T => T.Type == "password");
                var password = pClaim.FirstOrDefault().Value;
                //调用实现方法
                return jwtValidator.Validate(account, password);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取 Token
        /// </summary>
        /// <param name="jwtValidator">Jwt 验证</param>
        /// <returns></returns>
        public static JwtTokenResult GetToken(JwtValidator jwtValidator)
        {
            //准备返回的结构
            var jwtToken = new JwtTokenResult();
            //调用实现方法
            if (!jwtValidator.Validate()) return jwtToken;
            //
            IdentityModelEventSource.ShowPII = true;
            var tokenConfig = ConfigHelp.Get<JwtConfigData>("JwtConfig");
            jwtValidator.Password = MD5Tool.GetMd5(jwtValidator.Password);
            //Claim
            var claims = new List<Claim>();
            claims.Add(new Claim("account", jwtValidator.Account));
            claims.Add(new Claim("password", jwtValidator.Password));
            //签名秘钥
            var authSigningKey = GetSigning();
            //凭据
            var token = new JwtSecurityToken(
                         issuer: tokenConfig.Issuer,
                         //audience: tokenConfig.Audience,
                         claims: claims,
                         expires: DateTime.Now.Add(ClockSkew),
                         signingCredentials: authSigningKey);
            //结构数据
            jwtToken.Status = true;
            jwtToken.Token = new JwtSecurityTokenHandler().WriteToken(token);
            jwtToken.ValidTo = token.ValidTo;
            return jwtToken;
        }

        /// <summary>
        /// 启用 授权验证服务
        /// </summary>
        /// <param name="app">应用</param>
        public static void JwtAuthorize(this IApplicationBuilder app)
        {
            //用户认证
            app.UseAuthentication();
            //用户授权
            app.UseAuthorization();
        }

        /// <summary>
        /// 获取 SymmetricSecurityKey
        /// </summary>
        /// <returns></returns>
        public static SymmetricSecurityKey GetSymmetric()
        {
            var tokenConfig = ConfigHelp.Get<JwtConfigData>("JwtConfig");
            var secureKey = MD5Tool.GetMd5(tokenConfig.SecureKey);
            var secureKeyByte = Encoding.UTF8.GetBytes(secureKey);
            var securityKey = new SymmetricSecurityKey(secureKeyByte);
            return securityKey;
        }

        /// <summary>
        /// 获取 SigningCredentials
        /// </summary>
        /// <returns></returns>
        public static SigningCredentials GetSigning()
        {
            var authSigningKey = GetSymmetric();
            var securityAlgorithms = SecurityAlgorithms.HmacSha256;
            var credentials = new SigningCredentials(authSigningKey, securityAlgorithms);
            return credentials;
        }
    }
}
