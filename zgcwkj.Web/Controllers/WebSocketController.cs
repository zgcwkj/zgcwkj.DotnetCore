using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Claims;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using zgcwkj.Util;
using zgcwkj.Util.Common;

namespace zgcwkj.Web.Controllers
{
    /// <summary>
    /// WebSocket 控制器
    /// </summary>
    public class WebSocketController : Controller
    {
        /// <summary>
        /// 客户端列表
        /// </summary>
        private static readonly ConcurrentDictionary<string, WebSocket> SocketClients = new ConcurrentDictionary<string, WebSocket>();

        /// <summary>
        /// 群组列表
        /// </summary>
        private static readonly Dictionary<string, List<string>> SocketGroups = new Dictionary<string, List<string>>();

        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 删除群组
        /// </summary>
        [HttpGet("/rmgr")]
        public IActionResult Group(string gr = "")
        {
            var grMd5 = MD5Tool.GetMd5(gr);
            //删除群组
            if (SocketGroups.ContainsKey(grMd5))
            {
                SocketGroups.Remove(grMd5);
            }

            return Json(new { start = "ok" });
        }

        /// <summary>
        /// WebSocket 接口
        /// </summary>
        /// <param name="my">发送者</param>
        /// <param name="to">接收者</param>
        /// <param name="gr">群组</param>
        /// <returns></returns>
        [HttpGet("/ws")]
        public async Task WebSocket(string my = "", string to = "", string gr = "")
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var myMd5 = MD5Tool.GetMd5(my);
                var toMd5 = MD5Tool.GetMd5(to);
                var grMd5 = MD5Tool.GetMd5(gr);
                //创建群组
                if (gr.IsNotNull())
                {
                    //创建群组
                    if (!SocketGroups.ContainsKey(grMd5)) SocketGroups.Add(grMd5, new List<string>());
                    //群组成员
                    SocketGroups.TryGetValue(grMd5, out var list);
                    //加入
                    if (!list.Contains(myMd5)) list.Add(myMd5);
                }
                //退出
                await ExitSocketAsync(myMd5);
                //获取连接对象
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                //存储连接对象
                SocketClients.TryAdd(myMd5, webSocket);
                //消息传递
                while (!webSocket.CloseStatus.HasValue)
                {
                    //读取
                    var response = await ReceiveStringAsync(myMd5);
                    if (response.IsNull()) continue;
                    //发送
                    if (gr.IsNull())//发送给单个人
                    {
                        //写入
                        var isOK = await SendStringAsync(toMd5, response);
                        //发送失败时告诉自己
                        if (!isOK) await SendStringAsync(myMd5, "发送失败");
                    }
                    else if (gr.IsNotNull())//发送给群组
                    {
                        SocketGroups.TryGetValue(grMd5, out var group);
                        if (group.IsNull()) continue;
                        for (int i = 0; i < group.Count(); i++)
                        {
                            //排除自己
                            if (group[i] == myMd5) continue;
                            //写入
                            await SendStringAsync(group[i], response);
                        }
                    }
                }
                //退出
                await ExitSocketAsync(myMd5);
            }
            else
            {
                //重定向
                HttpContext.Response.Headers.Add("Location", "/WebSocket");
                HttpContext.Response.StatusCode = StatusCodes.Status301MovedPermanently;
            }
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="socketID"></param>
        /// <param name="data"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<bool> SendStringAsync(string socketID, string data, CancellationToken ct = default)
        {
            try
            {
                SocketClients.TryGetValue(socketID, out var socket);
                if (socket.IsNull()) return false;

                var buffer = Encoding.UTF8.GetBytes(data);
                var segment = new ArraySegment<byte>(buffer);
                await socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
                return true;
            }
            catch
            {
                await ExitSocketAsync(socketID);
                return false;
            }
        }

        /// <summary>
        /// 接受信息
        /// </summary>
        /// <param name="socketID"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<string> ReceiveStringAsync(string socketID, CancellationToken ct = default)
        {
            try
            {
                SocketClients.TryGetValue(socketID, out var socket);
                if (socket.IsNull()) return string.Empty;

                var buffer = new ArraySegment<byte>(new byte[1024]);
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return string.Empty;
                }

                using var reader = new StreamReader(ms, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            }
            catch
            {
                await ExitSocketAsync(socketID);
                return string.Empty;
            }
        }

        /// <summary>
        /// 主动断开连接
        /// </summary>
        /// <param name="socketID"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task ExitSocketAsync(string socketID, CancellationToken ct = default)
        {
            //保持唯一连接
            if (SocketClients.ContainsKey(socketID))
            {
                SocketClients.TryRemove(socketID, out WebSocket exitSocket);
                if (exitSocket.IsNull()) return;
                //断开
                await exitSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "ExitSocket", ct);
            }
        }
    }
}
