using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace zgcwkj.Util
{
    /// <summary>
    /// 全局异常捕获
    /// </summary>
    public class GlobalException : IAsyncExceptionFilter
    {
        /// <summary>
        /// 捕获到异常时
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnExceptionAsync(ExceptionContext context)
        {
            //如果异常没有被处理则进行处理
            if (context.ExceptionHandled == false)
            {
                //记录日志
                Logger.Error(context.Exception.ToTrim());
                //定义返回数据
                var result = new
                {
                    ErrorCode = -1,
                    ErrorMessage = context.Exception.Message,
                };
                //定义 Json 格式
                var serializer = new JsonSerializerOptions()
                {
                    //为空值时忽略，不返回数据
                    //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true,
                };
                //定义返回的数据
                context.Result = new ContentResult
                {
                    //返回状态码设置为200，表示成功
                    StatusCode = StatusCodes.Status200OK,
                    //设置返回格式
                    ContentType = "application/json;charset=utf-8",
                    Content = JsonSerializer.Serialize(result, serializer)
                };
            }
            //标记异常已经被处理了
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
