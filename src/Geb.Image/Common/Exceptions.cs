using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geb.Image
{
    /// <summary>
    /// 文件解码异常
    /// </summary>
    public class FileDecodeException:Exception
    {
        public FileDecodeException(String msg) : base(msg)
        {
        }
    }

    /// <summary>
    /// 文件编码异常
    /// </summary>
    public class FileEncodeException : Exception
    {
        public FileEncodeException(String msg) : base(msg)
        {
        }
    }
}
