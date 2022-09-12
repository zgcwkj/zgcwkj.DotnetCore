using zgcwkj.Util.Log;

namespace zgcwkj.Util.Common
{
    /// <summary>
    /// 内置的嵌入式资源
    /// </summary>
    public class InternalFile
    {
        /// <summary>
        /// 获取内部嵌入资源
        /// </summary>
        /// <param name="name">资源所在的命名空间的名称</param>
        /// <returns></returns>
        public byte[] GetByte(string name)
        {
            Stream stream = GetType().Assembly.GetManifestResourceStream(name);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            return bytes;
        }

        /// <summary>
        /// 输出文件(把文件保存到本地)
        /// </summary>
        /// <param name="name">资源所在的命名空间的名称</param>
        /// <param name="path">路径</param>
        /// <returns>状态</returns>
        public bool OutFile(string name, string path)
        {
            try
            {
                var bytes = GetByte(name);
                //文件所在的文件夹的路径
                string filePath = $"{GlobalConstant.GetRunPath}\\{Path.GetDirectoryName(path)}";
                //防止没有文件夹
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                //文件的路径
                string filePathAndName = $"{GlobalConstant.GetRunPath}\\{path}";
                //文件流输出
                Stream stream = File.OpenWrite(filePathAndName);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return false;
            }
        }
    }
}
