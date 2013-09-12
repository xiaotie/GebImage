<Query Kind="Program">
  <Reference Relative="..\..\dll\net4.0\Geb.Utils.dll">D:\MyWorkspace\DotNetWorkspace\01_Public_Geb.Image\dll\net4.0\Geb.Utils.dll</Reference>
  <Reference Relative="..\..\dll\net4.0\Geb.Utils.WinForm.dll">D:\MyWorkspace\DotNetWorkspace\01_Public_Geb.Image\dll\net4.0\Geb.Utils.WinForm.dll</Reference>
  <Reference Relative="..\..\dll\net4.0\Geb.Image.dll">D:\MyWorkspace\DotNetWorkspace\01_Public_Geb.Image\dll\net4.0\Geb.Image.dll</Reference>
  <Namespace>Geb.Image</Namespace>
  <Namespace>Geb.Utils</Namespace>
  <Namespace>Geb.Utils.WinForm</Namespace>
</Query>

String imgDir = Path.GetDirectoryName(LINQPad.Util.CurrentQueryPath) + "\\..\\img\\";

void Main()
{
	ImageRgb24 img = new ImageRgb24(imgDir + "cjk.jpg");
	img.ShowDialog();
}