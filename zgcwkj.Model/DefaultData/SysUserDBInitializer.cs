using System.Collections.Generic;
using zgcwkj.Model.Models;
using zgcwkj.Util;

namespace zgcwkj.Model.DefaultData
{
    /// <summary>
    /// 用户数据初始化
    /// </summary>
    public class SysUserDBInitializer
    {
        public static List<SysUserModel> GetData
        {
            get
            {
                List<SysUserModel> lists = new List<SysUserModel>();

                lists.Add(new SysUserModel
                {
                    UserID = GlobalConstant.GuidMd5,
                    UserName = "UserName",
                    Password = "Password",
                });

                return lists;
            }
        }
    }
}
