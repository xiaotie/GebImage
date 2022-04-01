#load "common.csx"

using Geb.Image;

void Save(ImageBgr24 img, String path)
{
    if(File.Exists(path)) File.Delete(path);
    img.SavePng(path);
}

var img = new ImageBgr24("./images/demo.png");
var imgCanvas = new ImageBgr24(800,800);
imgCanvas.Fill(Bgr24.WHITE);

// var m0 = AffineTransMatrix.CreateRotationMatrix(new PointD(0,0), 30);
// imgCanvas.DrawImage(img,m0);

// var m1 = AffineTransMatrix.CreateRotationMatrix(new PointD(200,200), 30) * AffineTransMatrix.CreateResizeMatrix(0.5,0.5);

// imgCanvas.DrawImage(img,m1);

// Save(imgCanvas,"./tmp/demo_draw_rotate1.png");

RotatedRectF rr = new RotatedRectF(new PointF(img.Width/2,img.Height/2), new SizeF(200,200), 10);
var clip = img[rr];
Save(clip,"./tmp/demo_clip.png");


