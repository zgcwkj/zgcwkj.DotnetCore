using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using zgcwkj.Util.DbUtil;

namespace zgcwkj.Model.Models
{
    /// <summary>
    /// 用户表
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
