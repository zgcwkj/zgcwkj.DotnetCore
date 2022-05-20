using zgcwkj.Util.Data.Cache;
using zgcwkj.Util.Data.DataBase;

namespace zgcwkj.Util
{
    /// <summary>
    /// 数据工厂
    /// </summary>
    public class DataFactory
    {
        /// <summary>
        /// 数据库对象
        /// </summary>
        public static IDataBase Db
        {
            get
            {
                return DbFactory.Db;
            }
        }

        /// <summary>
        /// 缓存对象
        /// </summary>
        public static ICache Cache
        {
            get
            {
                return CacheFactory.Cache;
            }
        }
    }
}
