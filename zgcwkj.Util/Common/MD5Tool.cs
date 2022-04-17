using System;
using System.Security.Cryptography;
using System.Text;

namespace zgcwkj.Util.Common
{
    /// <summary>
    /// MD5加密类
    /// </summary>
    public class MD5Tool
    {
        /// <summary>
        /// 获得MD5加密
        /// </summary>
        /// <param name="content">要加密的文本</param>
        /// <param name="isUpper">是否大写</param>
        /// <returns>返回加密获得文件</returns>
        public static string GetMd5(string content, bool isUpper = false)
        {
            using var md5 = MD5.Create();
            //16
            var toByte16 = Encoding.UTF8.GetBytes(content);
            var toByteHash16 = md5.ComputeHash(toByte16);
            var strResult16 = BitConverter.ToString(toByteHash16, 4, 8);
            strResult16 = strResult16.Replace("-", "");
            //32
            var toByte32 = Encoding.UTF8.GetBytes(strResult16);
            var toByteHash32 = md5.ComputeHash(toByte32);
            var strResult32 = BitConverter.ToString(toByteHash32);
            strResult32 = strResult32.Replace("-", "");
            //大小写转换
            var toMd5 = isUpper ? strResult32.ToUpper() : strResult32.ToLower();
            //返回
            return toMd5;
        }

        /// <summary>
        /// 获得MD5加密(16位)
        /// </summary>
        /// <param name="ConvertString">内容</param>
        /// <returns></returns>
        [Obsolete]
        public static string GetMd5_16(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        /// <summary>
        /// 获得MD5加密(32位)
        /// </summary>
        /// <param name="s">字符内容</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns></returns>
        [Obsolete]
        public static string GetMD5_32(string s, string _input_charset)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));//计算该数组的哈希值
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }
}