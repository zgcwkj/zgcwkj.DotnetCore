namespace zgcwkj.Util
{
    /// <summary>
    /// 表对象
    /// </summary>
    internal partial class TableMode
    {
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 排除字段
        /// </summary>
        public bool NotMapped { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type
        {
            get
            {
                return Value.GetType().Name;
            }
        }

        /// <summary>
        /// 字段数据
        /// </summary>
        public string Data
        {
            get
            {
                if (Value.GetType() == typeof(string))
                {
                    return $"'{Value}'";
                }
                else
                {
                    return $"{Value}";
                }
            }
        }
    }
}