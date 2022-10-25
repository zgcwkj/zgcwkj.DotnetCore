using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using zgcwkj.Util;

namespace zgcwkj.Model.Models
{
    /// <summary>
    /// 系统信息表
    /// </summary>
    [Table("sys_info")]
    public class SysInfoModel : DbModel
    {
        /// <summary>
        /// 系统ID
        /// </summary>
        [Key, Column("sys_id"), StringLength(32)]
        public string SysID { get; set; } = "";

        /// <summary>
        /// 系统名称
        /// </summary>
        [Column("sys_name")]
        public string SysName { get; set; } = "";

        /// <summary>
        /// 系统IP
        /// </summary>
        [Column("sys_ip")]
        public string SysIP { get; set; } = "";

        /// <summary>
        /// 系统地址
        /// </summary>
        [Column("sys_url")]
        public string SysUrl { get; set; } = "";

        /// <summary>
        /// 系统状态
        /// </summary>
        [Column("sys_status")]
        public bool SysStatus { get; set; } = true;

        /// <summary>
        /// 系统创建时间
        /// </summary>
        [Column("create_time")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
