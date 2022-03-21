#load "common.csx"

using Geb.Image;

var img = new ImageBgr24("./images/demo.png");
var imgCanvas = new ImageBgr24(800,800);
imgCanvas.Fill(Bgr24.WHITE);

var m0 = AffineTransMatrix.CreateRotationMatrix(new PointD(0,0), 30);
imgCanvas.DrawImage(img,m0);

var m1 = AffineTransMatrix.CreateRotationMatrix(new PointD(200,200), 30) * AffineTransMatrix.CreateResizeMatrix(0.5,0.5);

imgCanvas.DrawImage(img,m1);

String outFileName = "./tmp/demo_draw_rotate1.png";
if(File.Exists(outFileName)) File.Delete(outFileName);
imgCanvas.SavePng(outFileName);

