namespace zgcwkj.Util
{
    /// <summary>
    /// SQL实体
    /// </summary>
    internal class SqlModel
    {
        /// <summary>
        /// 脚本
        /// </summary>
        public string Sql { get; set; } = "";

        /// <summary>
        /// 排序
        /// </summary>
        public string OrderBy { get; set; } = "";

        /// <summary>
        /// 分组
        /// </summary>
        public string GroupBy { get; set; } = "";

        /// <summary>
        /// 追加脚本
        /// </summary>
        public string AppendSql { get; set; } = "";

        /// <summary>
        /// 结尾脚本
        /// </summary>
        public string EndSql { get; set; } = "";
    }
}
