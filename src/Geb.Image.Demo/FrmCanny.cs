using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Image.Demo
{
    using Geb.Image;
    using Geb.Utils;
    using Geb.Utils.WinForm;

    public partial class FrmCanny : Form
    {
        private ImageU8 Image;

        public FrmCanny()
        {
            InitializeComponent();
        }

        private void FrmCanny_Load(object sender, EventArgs e)
        {

        }

        private void htnOpen_Click(object sender, EventArgs e)
        {
            this.OpenImageFile((path) =>
                {
                    this.tbPath.Text = path;
                    Image = new ImageU8(new Bitmap(path));
                    this.pbMain.Image = Image.ToBitmap();
                });
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Image == null) return;

            ImageU8 tmp = Image.Clone() as ImageU8;
            tmp.ApplyCannyEdgeDetector(trackSigma.Value, trackSize.Value);
            tmp.ShowDialog();
        }
    }
}
