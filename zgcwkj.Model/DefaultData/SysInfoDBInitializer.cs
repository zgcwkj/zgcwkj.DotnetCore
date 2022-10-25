using System.Collections.Generic;
using zgcwkj.Model.Models;
using zgcwkj.Util.Common;

namespace zgcwkj.Model.DefaultData
{
    /// <summary>
    /// 系统信息数据初始化
    /// </summary>
    public class SysInfoDBInitializer
    {
        public static List<SysInfoModel> GetData
        {
            get
            {
                var lists = new List<SysInfoModel>();

                lists.Add(new SysInfoModel
                {
                    SysID = MD5Tool.GetMd5("zgcwkj"),
                    SysName = "SysName",
                    SysIP = "127.0.0.1",
                    SysUrl = "http://127.0.0.1",
                    SysStatus = true,
                    CreateTime = DateTime.Now,
                });

                return lists;
            }
        }
    }
}
