#load "../common.csx"

using Geb.Image;
using System.Drawing;

Bitmap bitmap = new Bitmap(100,100);
ImageU8 img = new ImageU8(bitmap);
Console.WriteLine(img[3,3]);
for(int i = 0; i < img.Length; i++)
    img[i] = (Byte)(i % 255);
bitmap = img.ToBitmap();
Console.WriteLine(bitmap.GetPixel(3,3));
img.ApplyGaussianBlur();
Console.WriteLine(img[3,3]);
