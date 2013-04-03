<Query Kind="Program">
  <Reference Relative="..\..\dll\net4.0\Geb.Image.dll">E:\MyWorkspace\DotNetWorkspace\01_Public_Geb.Image\dll\net4.0\Geb.Image.dll</Reference>
  <Reference Relative="..\..\dll\net4.0\Geb.Utils.dll">E:\MyWorkspace\DotNetWorkspace\01_Public_Geb.Image\dll\net4.0\Geb.Utils.dll</Reference>
  <Reference Relative="..\..\dll\net4.0\Geb.Utils.WinForm.dll">E:\MyWorkspace\DotNetWorkspace\01_Public_Geb.Image\dll\net4.0\Geb.Utils.WinForm.dll</Reference>
  <Namespace>Geb.Image</Namespace>
  <Namespace>Geb.Utils</Namespace>
  <Namespace>Geb.Utils.WinForm</Namespace>
</Query>

String baseDir = "E:\\MyWorkspace\\DotNetWorkspace\\01_Public_Geb.Image\\scripts\\img\\";

unsafe void Main()
{
	ImageRgb24 img = new ImageRgb24(baseDir + "cjk.jpg");
	img.ShowDialog("img");
	
	// 将图像看作连续的内存，通过偏移量来访问
	ImageRgb24 img2 = img.Clone();
	for(int i = 0; i < img2.Length; i++)
	{
		Rgb24 p= img2[i];
		p.Red = (Byte)(p.Red/2);
		img2[i] = p;
	}
	img2.ShowDialog("img2");
	
	// 将图像看作一个二维“表格”，通过行和列坐标来访问
	ImageRgb24 img3 = img.Clone();
	for(int row = 0; row < img3.Height; row++)
	{
		for(int col = 0; col < img3.Width; col ++)
		{
			Rgb24 p= img3[row,col];
			p.Red = (Byte)(p.Red/2);
			img3[row,col] = p;
		}
	}
	img3.ShowDialog("img3");
	
	// 直接通过指针访问
	ImageRgb24 img4 = img.Clone();
	{
		Rgb24* p = img4.Start;
		Rgb24* pEnd = p + img4.Length;
		while(p != pEnd)
		{
			p->Red = (Byte)(p->Red/2);
			p++;
		}
	}
	img4.ShowDialog("img4");
	
	// 通过lambda表达式访问
	ImageRgb24 img5 = img.Clone();
	img5.ForEach((Rgb24* p)=>{p->Red = (Byte)(p->Red/2);});
	img5.ShowDialog("img5");
	
	// Dispose 可以释放图像所占非管理内存。如果不Dispose，当GC销毁对象时，会释放非管理内存。
	// img.Dispose();
	// img2.Dispose();
	// img3.Dispose();
	// img4.Dispose();
	// img5.Dispose();
}