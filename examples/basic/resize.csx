#load "../common.csx"

using Geb.Image;

ImageBgr24 img = new ImageBgr24(80,100);
for(int h = 0; h < img.Height; h++)
{
    for(int w = 0; w < img.Width; w++)
    {
        int g = (h + w)*100; 
        Bgr24 val = new Bgr24(g,g,g);
        img[h,w] = val;
    }
}

ImageBgr24 img2 = img.Resize(60,60,InterpolationMode.Bilinear);
Console.WriteLine(img[1,1]);
Console.WriteLine(img[1,2]);
Console.WriteLine(img[2,1]);
Console.WriteLine(img[2,2]);
Console.WriteLine(img2[1,1]);

ImageBgr24 img3 = img.Resize(120,180,InterpolationMode.Bilinear);

double ax = 0.3333334;
double ay = 0.666666; 
double bx = 1 -ax;
double by = 1 - ay;

double v = 200*bx*by + 44 * ax * by + 44 * bx*ay + 144 * ax * ay;
Console.WriteLine(v);

// img.Save("1.png");
// img2.Save("2.png");
// img3.Save("3.png");

