#load "common.csx"

using Geb.Image;
using Geb.Image.Analysis;

int width = 600;
int height = 600;
ImageBgr24 image = new ImageBgr24(width,height);
image.Fill(Bgr24.WHITE);

Random r = new Random();
// 随机生成点
PointF[] points = new PointF[12];
for(int i = 0; i < points.Length; i++)
{
    int x = r.Next(100, width - 100);
    int y = r.Next(100, height - 100);
    points[i] = new PointF(x,y);
    image.FillCircle(x,y,Bgr24.RED,2);
}

// 计算凸包点
List<PointF> list = new List<PointF>();
list.AddRange(points);
var chPoints = ConvexHull.CreateConvexHull(list).ToArray();

// 绘制凸包点
int num = 0;
foreach(var item in chPoints)
{
    num ++;
    image.DrawCircle(item.X,item.Y,Bgr24.BLUE,4);
    Console.WriteLine($"({item.X},{item.Y})");
    image.DrawText(num.ToString(), Bgr24.BLUE, new PointS((short)item.X - 20, (short)item.Y - 20));
}

// 计算 MinAreaRect
RotatedRectF rect = ConvexHull.MinAreaRect(chPoints,true);
// 绘制旋转矩阵
PointF[] rps = rect.Points();
for(int i = 0; i < rps.Length; i++)
{
    PointF p0 = rps[i];
    PointF p1 = rps[(i+1)%rps.Length];
    image.DrawLine(p0,p1,Bgr24.RED);
}

// 绘制矩阵中心
image.DrawCircle(rect.Center.X, rect.Center.Y, Bgr24.RED, 3);

image.Save("./tmp/image.png");
