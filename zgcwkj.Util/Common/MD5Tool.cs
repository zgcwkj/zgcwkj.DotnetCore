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
        /// <param name="str">要加密的文本</param>
        /// <returns>返回加密获得文件</returns>
        public static string GetMd5(string str)
        {
            return GetMD5_32(GetMd5_16(str), "UTF-8");
        }

        /// <summary>
        /// 获得MD5加密(16位)
        /// </summary>
        /// <param name="ConvertString">内容</param>
        /// <returns></returns>
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