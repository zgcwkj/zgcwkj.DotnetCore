using System.Net;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Text;

namespace zgcwkj.Util.Common
{
    /// <summary>
    /// 网络请求工具
    /// </summary>
    public class HttpTool
    {
        private static readonly Lazy<HttpClient> _sharedClient = new(() => new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        }));

        /// <summary>
        /// 共享的HttpClient实例
        /// </summary>
        public static HttpClient SharedClient => _sharedClient.Value;

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url">请求的路径</param>
        /// <returns>结果</returns>
        public static string Get(string url)
        {
            return HttpTool.Get(url, out _);
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url">请求的路径</param>
        /// <param name="httpTool">对象</param>
        /// <returns>结果</returns>
        public static string Get(string url, out HttpTool httpTool)
        {
            httpTool = new HttpTool();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("ContentType", "text/html;charset=UTF-8");
            request.Headers.Add("Referer", url);

            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <param name="url">提交的路径</param>
        /// <param name="data">提交的数据</param>
        /// <returns>结果</returns>
        public static string Post(string url, string data)
        {
            return HttpTool.Post(url, data, out _);
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <param name="url">提交的路径</param>
        /// <param name="data">提交的数据</param>
        /// <param name="httpTool">对象</param>
        /// <returns>结果</returns>
        public static string Post(string url, string data, out HttpTool httpTool)
        {
            httpTool = new HttpTool();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("ContentType", "application/x-www-form-urlencoded;charset=UTF-8");
            request.Headers.Add("Referer", url);

            if (!data.IsNull())
            {
                request.Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns>结果</returns>
        public static string Initiate(HttpRequestMessage request)
        {
            return HttpTool.Initiate(request, out _);
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="httpTool">对象</param>
        /// <returns>结果</returns>
        public static string Initiate(HttpRequestMessage request, out HttpTool httpTool)
        {
            httpTool = new HttpTool();
            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="filePath">文件地址</param>
        /// <returns>HttpTool对象和成功状态</returns>
        public static async Task<(HttpTool httpTool, bool success)> DownloadFileWithResultAsync(string url, string filePath)
        {
            var httpTool = new HttpTool();
            try
            {
                await httpTool.DownloadFileCoreAsync(url, filePath);
                return (httpTool, true);
            }
            catch
            {
                return (httpTool, false);
            }
        }

        /// <summary>
        /// Cookie 数据字符串
        /// </summary>
        public string CookieStr { get; set; } = "";

        /// <summary>
        /// 编码类型
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 发起网络请求（核心）
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns>结果</returns>
        public string InitiateWeb(HttpRequestMessage request)
        {
            try
            {
                if (!CookieStr.IsNull())
                {
                    request.Headers.Add("Cookie", CookieStr);
                }

                var response = SharedClient.Send(request);
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content;
            }
            catch (Exception ex)
            {
                Logger.Error($"HttpTool Error: {ex.Message} Url: {request.RequestUri}");
                return ex.Message;
            }
        }

        /// <summary>
        /// 下载文件请求（核心）
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="filePath">文件地址</param>
        public async Task DownloadFileCoreAsync(string url, string filePath)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                if (!CookieStr.IsNull())
                {
                    request.Headers.Add("Cookie", CookieStr);
                }

                var response = await SharedClient.SendAsync(request);
                var content = await response.Content.ReadAsByteArrayAsync();

                // 确保目录存在
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllBytesAsync(filePath, content);
                Logger.Info($"HttpTool Download(OK): {filePath} Url: {url}");
            }
            catch (Exception ex)
            {
                Logger.Error($"HttpTool Download(Error): {ex.Message} Url: {url}");
            }
        }
    }

    /// <summary>
    /// 网络请求工具帮助
    /// </summary>
    public static class HttpToolHelp
    {
        /// <summary>
        /// 验证端口是否打开
        /// </summary>
        /// <param name="port">端口</param>
        /// <returns>占用状态</returns>
        public static bool IsPortOpen(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port) return true;
            }
            return false;
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static string GetIP(string url)
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(new Uri(url).Host);
                if (host.AddressList.Length != 0)
                {
                    return host.AddressList[0].To<string>();
                }
            }
            catch { }
            return "";
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="httpTool">对象</param>
        /// <param name="url">请求的路径</param>
        /// <returns>结果</returns>
        public static string Get(this HttpTool httpTool, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("ContentType", "text/html;charset=UTF-8");
            request.Headers.Add("Referer", url);

            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <param name="httpTool">对象</param>
        /// <param name="url">请求的路径</param>
        /// <param name="data">提交的数据</param>
        /// <returns>结果</returns>
        public static string Post(this HttpTool httpTool, string url, string data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("ContentType", "application/x-www-form-urlencoded;charset=UTF-8");
            request.Headers.Add("Referer", url);

            if (!data.IsNull())
            {
                request.Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="httpTool">对象</param>
        /// <param name="request">请求对象</param>
        /// <returns>结果</returns>
        public static string Initiate(this HttpTool httpTool, HttpRequestMessage request)
        {
            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="httpTool">对象</param>
        /// <param name="url">请求地址</param>
        /// <param name="filePath">文件地址</param>
        public static async Task DownloadFileAsync(this HttpTool httpTool, string url, string filePath)
        {
            await httpTool.DownloadFileCoreAsync(url, filePath);
        }
    }
}
