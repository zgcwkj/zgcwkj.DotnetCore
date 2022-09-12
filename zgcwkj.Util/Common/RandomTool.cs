namespace zgcwkj.Util.Common
{
    /// <summary>
    /// 随机字符工具
    /// </summary>
    public class RandomTool
    {
        /// <summary>
        /// 随机数字的内容
        /// </summary>
        public string DigitalStr { get; set; } = "|1|2|3|4|5|6|7|8|9|0";

        /// <summary>
        /// 随机数字的拼接
        /// </summary>
        public bool DigitalOut { get; set; } = false;

        /// <summary>
        /// 随机字符的内容
        /// </summary>
        public string ContentStr { get; set; } = "|q|w|e|r|t|y|u|i|o|p|a|s|d|f|g|h|j|k|l|z|x|c|v|b|n|m";

        /// <summary>
        /// 随机数字的拼接
        /// </summary>
        public bool ContentOut { get; set; } = false;

        /// <summary>
        /// 随机符号的内容
        /// </summary>
        public string SymbolStr { get; set; } = "|,|.|/|;|'|[|]|{|}|;|<|>|?|!|@|#|$|%|^|&|*|(|)|_|-|+|=|~";

        /// <summary>
        /// 随机数字的拼接
        /// </summary>
        public bool SymbolOut { get; set; } = false;

        /// <summary>
        /// 随机字符
        /// </summary>
        private string RandomStr
        {
            get
            {
                string randomStr = "";//输入的随机字符
                if (DigitalOut) randomStr = DigitalStr;//数字
                if (ContentOut) randomStr += ContentStr;//字符
                if (SymbolOut) randomStr += SymbolStr;//符号
                randomStr = randomStr.Substring(1, randomStr.Length - 1);//不要第一个分隔符
                return randomStr;
            }
        }

        /// <summary>
        /// 随机字符工具
        /// </summary>
        /// <param name="digital">数字</param>
        /// <param name="content">字符</param>
        /// <param name="symbol">符号</param>
        public RandomTool(bool digital = true, bool content = false, bool symbol = false)
        {
            this.DigitalOut = digital;
            this.ContentOut = content;
            this.SymbolOut = symbol;
        }

        /// <summary>
        /// 获取随机字符
        /// </summary>
        /// <param name="Length">长度</param>
        /// <returns>随机字符</returns>
        public string GetRandom(int Length)
        {
            string strRandom = "";
            string[] zf = RandomStr.Split('|');
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                int sjs = rd.Next(zf.Length);
                strRandom += zf[sjs];
            }
            return strRandom;
        }

        /// <summary>
        /// 获取随机字符
        /// </summary>
        /// <param name="Length">长度</param>
        /// <param name="RandomStr">随机字符(默认为英文加数字)</param>
        /// <returns>随机字符</returns>
        public static string GetRandomStr(int Length, string RandomStr = "")
        {
            var rTool = new RandomTool(true, true, false);
            if (RandomStr.IsNotNull())
            {
                rTool.DigitalStr = string.Empty;
                rTool.ContentStr = RandomStr.IndexOf("|") > 0 ? RandomStr : string.Join("|", RandomStr.ToCharArray());
                rTool.SymbolStr = string.Empty;
            }
            return rTool.GetRandom(Length);
        }

        /// <summary>
        /// 获取随机数字
        /// </summary>
        /// <param name="Length">长度</param>
        /// <param name="RandomStr">随机字符(默认为英文加数字)</param>
        /// <returns>随机数字</returns>
        public static int GetRandomInt(int Length, string RandomStr = "")
        {
            var rTool = new RandomTool(true, false, false);
            if (RandomStr.IsNotNull())
            {
                rTool.DigitalStr = RandomStr.IndexOf("|") > 0 ? RandomStr : string.Join("|", RandomStr.ToCharArray());
                rTool.ContentStr = string.Empty;
                rTool.SymbolStr = string.Empty;
            }
            return rTool.GetRandom(Length).ToInt();
        }

        /// <summary>
        /// 获取随机码
        /// 把 GoRandom 改成 GetRandom 即可
        /// </summary>
        /// <param name="Length">长度</param>
        /// <returns></returns>
        [Obsolete]
        public string GoRandom(int Length)
        {
            return this.GetRandom(Length);
        }
    }
}
