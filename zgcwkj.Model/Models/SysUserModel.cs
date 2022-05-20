using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using zgcwkj.Util;

namespace zgcwkj.Model.Models
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    [Table("sys_user")]
    public class SysUserModel : DbModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Key, Column("user_id"), StringLength(32)]
        public string UserID { get; set; } = "";

        /// <summary>
        /// 系统ID
        /// </summary>
        [Column("sys_id")]
        public string SysID { get; set; } = "";

        /// <summary>
        /// 用户名称
        /// </summary>
        [Column("user_name")]
        public string UserName { get; set; } = "";

        /// <summary>
        /// 用户密码
        /// </summary>
        [Column("password")]
        public string Password { get; set; } = "";

        /// <summary>
        /// 用户私密
        /// </summary>
        [NotMapped]
        public string Privacy { get; set; } = "";
    }
}
