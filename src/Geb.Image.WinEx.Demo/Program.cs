using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geb.Image.WinEx.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bmp = new Bitmap(@"D:\测试数据\demo.png");
            ImageU8 img = bmp.ToImageU8();
            Console.WriteLine(img[5, 5]);
            Console.WriteLine(bmp.GetPixel(5, 5));
            Console.Read();
        }
    }
}
