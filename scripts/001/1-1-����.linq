<Query Kind="Program">
  <Reference Relative="..\..\dll\net4.0\Geb.Utils.dll">E:\MyWorkspace\DotNetWorkspace\01_Public_Geb.Image\dll\net4.0\Geb.Utils.dll</Reference>
  <Reference Relative="..\..\dll\net4.0\Geb.Utils.WinForm.dll">E:\MyWorkspace\DotNetWorkspace\01_Public_Geb.Image\dll\net4.0\Geb.Utils.WinForm.dll</Reference>
  <Reference Relative="..\..\dll\net4.0\Geb.Image.dll">E:\MyWorkspace\DotNetWorkspace\01_Public_Geb.Image\dll\net4.0\Geb.Image.dll</Reference>
  <Namespace>Geb.Image</Namespace>
  <Namespace>Geb.Utils</Namespace>
  <Namespace>Geb.Utils.WinForm</Namespace>
</Query>

String baseDir = "E:\\MyWorkspace\\DotNetWorkspace\\01_Public_Geb.Image\\scripts\\img\\";

unsafe void Main()
{
	ImageRgb24 img = new ImageRgb24(baseDir + "cjk.jpg");
	img.ShowDialog();
	img.ForEach((Rgb24* p)=>{ p->Red = (Byte)(p->Red/2); });
	img.ShowDialog();	
}