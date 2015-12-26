using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Image.Demo
{
    using Geb.Utils.WinForm;

    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnCannyDeno_Click(object sender, EventArgs e)
        {
            new FrmCanny().ShowDialog();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private unsafe void btnPrimaryColor_Click(object sender, EventArgs e)
        {
            Rgb24[] colors = new Rgb24[]{Rgb24.BLACK,Rgb24.BLUE,Rgb24.GREEN,Rgb24.PINK,Rgb24.RED};
            int[] idxes = new int[colors.Length];
            int[] map = new int[512];

            for (int r = 0; r < 8; r++)
            {
                for (int g = 0; g < 8; g++)
                {
                    for (int b = 0; b < 8;  b++)
                    {
                    }
                }
            }

            this.OpenImageFile(path =>
                {
                    ImageRgb24 img = new ImageRgb24(path);
                });
        }

        private void btnDistanceTransformDemo_Click(object sender, EventArgs e)
        {
            new Tests.DistanceTransformTest().Run();
        }
    }
}
