using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using zgcwkj.Util.Log;

#pragma warning disable SYSLIB0014

namespace zgcwkj.Util.Common
{
    /// <summary>
    /// 网络请求工具
    /// </summary>
    public class HttpTool
    {
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/html;charset=UTF-8";
            request.Referer = url;
            request.Method = "GET";

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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.Referer = url;
            request.Method = "POST";

            if (!data.IsNull())
            {
                request.ContentLength = Encoding.UTF8.GetByteCount(data);
                Stream myRequestStream = request.GetRequestStream();
                byte[] postBytes = Encoding.UTF8.GetBytes(data);
                myRequestStream.Write(postBytes, 0, postBytes.Length);
            }

            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns>结果</returns>
        public static string Initiate(HttpWebRequest request)
        {
            return HttpTool.Initiate(request, out _);
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="httpTool">对象</param>
        /// <returns>结果</returns>
        public static string Initiate(HttpWebRequest request, out HttpTool httpTool)
        {
            httpTool = new HttpTool();
            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="filePath">文件地址</param>
        /// <param name="httpTool">对象</param>
        public static void DownloadFile(string url, string filePath, out HttpTool httpTool)
        {
            httpTool = new HttpTool();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Referer = url;

            httpTool.DownloadFile(request, filePath);
        }

        /// <summary>
        /// Cookie 对象
        /// </summary>
        public CookieContainer Cookie { get; set; } = new CookieContainer();

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
        public string InitiateWeb(HttpWebRequest request)
        {
            try
            {
                if (CookieStr.IsNull()) request.CookieContainer = Cookie;
                else request.Headers.Add("Cookie", CookieStr);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = Cookie.GetCookies(response.ResponseUri);

                using Stream stream = response.GetResponseStream();
                using StreamReader myStreamReader = new StreamReader(stream, Encoding);

                string retString = myStreamReader.ReadToEnd();
                return retString;
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
        /// <param name="request">请求对象</param>
        /// <param name="filePath">文件地址</param>
        public void DownloadFile(HttpWebRequest request, string filePath)
        {
            //float percent = 0;//百分比
            try
            {
                if (CookieStr.IsNull()) request.CookieContainer = Cookie;
                else request.Headers.Add("Cookie", CookieStr);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = Cookie.GetCookies(response.ResponseUri);

                using Stream stream = response.GetResponseStream();
                using Stream outStream = new FileStream(filePath, FileMode.Create);

                byte[] by = new byte[1024];
                int osize = stream.Read(by, 0, (int)by.Length);

                //long totalDownloadedByte = 0;
                //long totalBytes = response.ContentLength;
                while (osize > 0)
                {
                    outStream.Write(by, 0, osize);
                    osize = stream.Read(by, 0, (int)by.Length);

                    //totalDownloadedByte = osize + totalDownloadedByte;
                    //percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                    //Console.Write(percent.ToString("00") + " ");
                }
                response.Close();
                stream.Close();
                outStream.Close();
                Logger.Info($"HttpTool Download(OK): {filePath} Url: {request.RequestUri}");
            }
            catch (Exception ex)
            {
                Logger.Error($"HttpTool Download(Error): {ex.Message} Url: {request.RequestUri}");
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
                    return host.AddressList[0].ToStr();
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/html;charset=UTF-8";
            request.Referer = url;
            request.Method = "GET";

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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.Referer = url;
            request.Method = "POST";

            if (!data.IsNull())
            {
                request.ContentLength = Encoding.UTF8.GetByteCount(data);
                Stream myRequestStream = request.GetRequestStream();
                byte[] postBytes = Encoding.UTF8.GetBytes(data);
                myRequestStream.Write(postBytes, 0, postBytes.Length);
            }

            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="httpTool">对象</param>
        /// <param name="request">请求对象</param>
        /// <returns>结果</returns>
        public static string Initiate(this HttpTool httpTool, HttpWebRequest request)
        {
            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="httpTool">对象</param>
        /// <param name="url">请求地址</param>
        /// <param name="filePath">文件地址</param>
        public static void DownloadFile(this HttpTool httpTool, string url, string filePath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Referer = url;

            httpTool.DownloadFile(request, filePath);
        }
    }
}

#pragma warning restore SYSLIB0014
