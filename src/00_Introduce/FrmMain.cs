using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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

        private void btnDemo2_Click(object sender, EventArgs e)
        {
            FrmDemo2 demo2 = new FrmDemo2();
            demo2.ShowDialog();
        }

        private unsafe void btnDemo3_Click(object sender, EventArgs e)
        {
            Cat cat = new Cat();
            int age = 20;
            cat.SetAge(&age);
            MessageBox.Show(cat.Age.ToString());
        }

        private unsafe void btnDemo4_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            using (ImageArgb32 img = new ImageArgb32(1000, 1000))
            {
                sw.Start();

                for (int i = 0; i < 100; i++)
                {
                    Argb32* p;
                    ItArgb32Old itor = img.CreateItorOld();
                    while ((p = itor.Next()) != null)
                    {
                        p->Red = 200;
                    }
                }

                sw.Stop();
                long ms0 = sw.ElapsedMilliseconds;

                ImageRgb24 imgBig = new ImageRgb24(3000, 4000);

                sw.Reset();
                sw.Start();

                for (int i = 0; i < 100; i++)
                {
                    Argb32* pStart = img.Start;
                    Argb32* pEnd = img.Start + img.Length;
                    while (pStart < pEnd)
                    {
                        pStart->Red = 200;
                        pStart++;
                    }
                }

                sw.Stop();
                long ms1 = sw.ElapsedMilliseconds;
                MessageBox.Show(String.Format("Itor:{0} ms / Normal: {1} ms", ms0, ms1));
            }
        }

        private unsafe void btnDemo5_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            using (ImageArgb32 img = new ImageArgb32(1000, 1000))
            {
                sw.Start();

                for (int i = 0; i < 100; i++)
                {
                    ItArgb32 itor = img.CreateItor();
                    for (Argb32* p = itor.Start; p < itor.End; p+= itor.Step(p))
                    {
                        p->Red = 200;
                    }
                }

                sw.Stop();
                long ms0 = sw.ElapsedMilliseconds;

                sw.Reset();
                sw.Start();
                for (int i = 0; i < 100; i++)
                {
                    Argb32* pStart = img.Start;
                    Argb32* pEnd = img.Start + img.Length;
                    while (pStart < pEnd)
                    {
                        pStart->Red = 200;
                        pStart++;
                    }
                }

                sw.Stop();
                long ms1 = sw.ElapsedMilliseconds;
                MessageBox.Show(String.Format("Itor:{0} ms / Normal: {1} ms", ms0, ms1));
            }
        }

        private unsafe void btnTest6_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            using (ImageArgb32 img = new ImageArgb32(1000, 1000))
            {
                sw.Start();

                for (int i = 0; i < 100; i++)
                {
                    ItRoiArgb32 itRoi = img.CreateRoiItor(1, 1, img.Width - 2, img.Height - 2);

                    for (Argb32* line = itRoi.Start; line < itRoi.End; line += itRoi.Step(line))
                    {
                        ItArgb32 it = itRoi.Itor(line);
                        for (Argb32* p = it.Start; p < it.End; p += it.Step(p))
                        {
                            p->Red = 200;
                        }
                    }
                }

                sw.Stop();
                long ms0 = sw.ElapsedMilliseconds;

                sw.Reset();
                sw.Start();
                for (int i = 0; i < 100; i++)
                {
                    int width = img.Width;
                    int height = img.Height;

                    for (int h = 1; h < height - 1; h++)
                    {
                        Argb32* line = img.Start + h * width;
                        for (int w = 1; w < width - 1; w++)
                        {
                            line[w].Red = 255;
                        }
                    }
                }

                sw.Stop();
                long ms1 = sw.ElapsedMilliseconds;
                MessageBox.Show(String.Format("Itor:{0} ms / Normal: {1} ms", ms0, ms1));
            }
        }

        private unsafe void btnDemo7_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            using (ImageArgb32 img = new ImageArgb32(1000, 1000))
            {
                sw.Start();

                for (int i = 0; i < 100; i++)
                {
                    img.ForEach((Argb32* p) => { p->Red = 200; });
                }

                sw.Stop();
                long ms0 = sw.ElapsedMilliseconds;

                sw.Reset();
                sw.Start();
                for (int i = 0; i < 100; i++)
                {
                    Argb32* pStart = img.Start;
                    Argb32* pEnd = img.Start + img.Length;
                    while (pStart < pEnd)
                    {
                        pStart->Red = 200;
                        pStart++;
                    }
                }

                sw.Stop();
                long ms1 = sw.ElapsedMilliseconds;
                MessageBox.Show(String.Format("Itor:{0} ms / Normal: {1} ms", ms0, ms1));
            }
        }

        private void lk_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.geblab.com");
        }

        private unsafe void btnDemo8_Click(object sender, EventArgs e)
        {
            Argb32* p = stackalloc Argb32[10];
            uint val = (uint)p;
            val = val % 4;
            Console.WriteLine(val);
            MessageBox.Show(val.ToString());
            Argb32* heap = (Argb32*)System.Runtime.InteropServices.Marshal.AllocHGlobal(40);

            ImageArgb32 img2 = new ImageArgb32(3000, 3000);
            Console.Write(img2);
            Stopwatch sw = new Stopwatch();
            using (ImageArgb32 img = new ImageArgb32(10000, 5000))
            {
                sw.Start();
                int width = img.Width;
                int height = img.Height;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        p[0] = img[y, x];
                        //heap[0] = img[y, x];
                    }
                }

                sw.Stop();
                long ms0 = sw.ElapsedMilliseconds;

                sw.Reset();
                sw.Start();

                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                    }
                }

                sw.Stop();
                Console.Write(p[0]);
                Console.Write(heap[0]);
                long ms1 = sw.ElapsedMilliseconds;
                MessageBox.Show(String.Format("Itor:{0} ms / Normal: {1} ms", ms0, ms1));
            }
        }
    }
}
