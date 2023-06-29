using Microsoft.AspNetCore.Rewrite;
using System.Text.RegularExpressions;

namespace zgcwkj.Web.Extensions
{
    /// <summary>
    /// 地址重写规则
    /// </summary>
    public class PathRewrite : IRule
    {
        /// <summary>
        /// 应用规则
        /// </summary>
        /// <param name="context"></param>
        public void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;
            var toScheme = request.Scheme;
            var toPath = request.Path.Value;
            var toHost = request.Host.Value;

            if (toPath == null) return;
            //重复出现多斜杠时，改为一个斜杠
            var maths = Regex.Matches(toPath, @"(.*?)[/]{2,}");
            if (maths.Count > 0)
            {
                foreach (Match math in maths)
                {
                    toPath = toPath.Replace(math.Value, "/");
                }
                context.HttpContext.Response.Redirect($"{toScheme}://{toHost}{toPath}");
                context.Result = RuleResult.EndResponse;
            }
        }
    }
}
