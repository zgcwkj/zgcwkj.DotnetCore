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
    [Table("sys_user")]
    public class SysUserModel : DbModel
    {
        [Key, Column("user_id"), StringLength(32)]
        public string UserID { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("password")]
        public string Password { get; set; }
    }
}
