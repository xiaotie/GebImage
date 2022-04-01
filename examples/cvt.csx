#load "common.csx"

using Geb.Image;

String CheckFile(String filePath)
{
    if(File.Exists(filePath) && filePath.EndsWith(".png")) File.Delete(filePath);
    return filePath;
}

var img = new ImageBgr24("./images/demo.png");
var img8 = img.ToGrayscaleImage();
var img32 = img.ToImageBgr32();
var imgClip = img[new Rect(0,img.Height - 200,img.Width,200)];

img.SavePng(CheckFile("./tmp/demo_png24.png"));
img8.SavePng(CheckFile("./tmp/demo_png8.png"));
img32.SavePng(CheckFile("./tmp/demo_png32.png"));
imgClip.SavePng(CheckFile("./tmp/demo_png24_clip.png"));