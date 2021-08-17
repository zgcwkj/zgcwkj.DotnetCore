using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace zgcwkj.Util.Common
{
    /// <summary>
    /// 网络请求工具
    /// </summary>
    public class HttpTool
    {
        /// <summary>
        /// Cookie 对象
        /// </summary>
        public CookieContainer cookie = new CookieContainer();

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url">请求的路径</param>
        /// <returns>返回结果</returns>
        public static string Get(string url)
        {
            return HttpTool.Get(url,  out _);
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url">请求的路径</param>
        /// <param name="httpTool">对象</param>
        /// <returns>返回结果</returns>
        public static string Get(string url, out HttpTool httpTool)
        {
            httpTool = new HttpTool();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/html;charset=UTF-8";
            request.CookieContainer = httpTool.cookie;
            request.Method = "GET";

            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <param name="url">提交的路径</param>
        /// <param name="data">提交的数据</param>
        /// <returns>返回结果</returns>
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
        /// <returns>返回结果</returns>
        public static string Post(string url, string data, out HttpTool httpTool)
        {
            httpTool = new HttpTool();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.CookieContainer = httpTool.cookie;
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
        /// <returns></returns>
        public static string Initiate(HttpWebRequest request)
        {
            return HttpTool.Initiate(request, out _);
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="httpTool">对象</param>
        /// <returns></returns>
        public static string Initiate(HttpWebRequest request, out HttpTool httpTool)
        {
            httpTool = new HttpTool();
            return httpTool.InitiateWeb(request);
        }

        /// <summary>
        /// 发起网络请求（核心）
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public string InitiateWeb(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = cookie.GetCookies(response.ResponseUri);

                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();

                return retString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 设置证书策略
        /// </summary>
        public static void SetCertificatePolicy()
        {
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        }

        /// <summary>
        /// 远程证书验证
        /// </summary>
        private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            // 信任任何证书
            System.Console.WriteLine("Warning, trust any certificate");
            return true;
        }
    }
}
