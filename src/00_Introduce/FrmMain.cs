using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Introduce
{
    using Geb.Utils;
    using Geb.Image;

    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private unsafe void FrmMain_Load(object sender, EventArgs e)
        {
        }

        private unsafe void btnRun_Click(object sender, EventArgs e)
        {

            //ImageArgb32 img = new ImageArgb32(1000, 1000);
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //int count = 0;
            //for (int i = 0; i < 100; i++)
            //{
            //    {
            //        ItArgb32 it = img.Itor();
            //        for (Argb32* p = it.Start; p < it.End; p += it.Step(p))
            //        {
            //            p->Red = 200;
            //        }
            //    }

            //    {
            //        ItRoiArgb32 itRoi = img.RoiItor();
            //        for (Argb32* line = itRoi.Start; line < itRoi.End; line += itRoi.Step(line))
            //        {
            //            ItArgb32 it = itRoi.Itor(line);
            //            for (Argb32* p = it.Start; p < it.End; p += it.Step(p))
            //            {
            //                p->Red = 200;
            //                count++;
            //            }
            //        }
            //    }

            //    //Argb32* end = it.End;
            //    //Argb32* start = it.Ptr;
            //    //while (it.Ptr < it.End)
            //    //{
            //    //    it.Ptr++;
            //    //    //it.Ptr++;
            //    //    // it.Ptr->Red = 255;
            //    //}
            //}
            //sw.Stop();

            //MessageBox.Show(sw.ElapsedMilliseconds.ToString() + "ms, " + count.ToString());
        }

        private unsafe void btnRun2_Click(object sender, EventArgs e)
        {
            //ImageArgb32 img = new ImageArgb32(1000, 1000);
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //int count = 0;
            //for (int i = 0; i < 100; i++)
            //{
            //    Argb32* pStart = img.Start;
            //    Argb32* pEnd = img.Start + img.Length;
            //    while (pStart < pEnd)
            //    {
            //        pStart->Red = 255;
            //        pStart++;
            //    }

            //    int width = img.Width;
            //    int height = img.Height;

            //    for (int h = 1; h < height - 1; h++)
            //    {
            //        Argb32* line = img.Start + h * width;
            //        for (int w = 1; w < width - 1; w++)
            //        {
            //            line[w].Red = 255;
            //            count++;
            //        }
            //    }
            //}

            //sw.Stop();

            //MessageBox.Show(sw.ElapsedMilliseconds.ToString() + "ms'," + count.ToString());
        }

        private void btnDemo1_Click(object sender, EventArgs e)
        {
            FrmDemo1 demo1 = new FrmDemo1();
            demo1.ShowDialog();
        }
    }
}
