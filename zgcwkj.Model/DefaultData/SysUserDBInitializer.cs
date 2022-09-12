using System.Collections.Generic;
using zgcwkj.Model.Models;
using zgcwkj.Util;
using zgcwkj.Util.Common;

namespace zgcwkj.Model.DefaultData
{
    /// <summary>
    /// 系统用户数据初始化
    /// </summary>
    public class SysUserDBInitializer
    {
        public static List<SysUserModel> GetData
        {
            get
            {
                var lists = new List<SysUserModel>();

                lists.Add(new SysUserModel
                {
                    UserID = GlobalConstant.GuidMd5,
                    UserName = "UserName",
                    Password = "Password",
                    SysID = MD5Tool.GetMd5("zgcwkj"),
                });

                return lists;
            }
        }
    }
}
