#load "../common.csx"

using Geb.Image;

ImageBgra32 img = new ImageBgra32(100,100);
Console.WriteLine(img[32]);