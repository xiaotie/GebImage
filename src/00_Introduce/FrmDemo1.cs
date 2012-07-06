using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Introduce
{
    using Geb.Utils.WinForm;
    using Geb.Image;

    public partial class FrmDemo1 : Form
    {
        int val;

        public FrmDemo1()
        {
            InitializeComponent();
        }

        private unsafe void btnSubmit_Click(object sender, EventArgs e)
        {
            this.OpenImageFile((String path) =>
                {
                    new ImageArgb32(path).ShowDialog("原始图像")
                        .ToGrayscaleImage().ShowDialog("灰度图像")
                        .ApplyOtsuThreshold().ShowDialog("二值化图像")
                        .ToImageArgb32()
                        .ForEach((Argb32* p) => { if (p->Red == 255) *p = Argb32.RED; })
                        .ShowDialog("染色");
                });
        }
    }
}
