using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geb.Image.WinForm
{
    public partial class FrmImage : Form
    {
        public Bitmap Bitmap { get; set; }

        public Func<Bitmap> Generator { get; set; }

        public FrmImage()
        {
            InitializeComponent();
        }

        private void FrmImage_Load(object sender, EventArgs e)
        {
            if (Bitmap != null)
            {
                this.imageBox.Image = Bitmap;
            }

            if(Generator != null)
            {
                this.backgroundWorker.RunWorkerAsync();
            }
        }

        delegate void InvokeUpdate();

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                var bmp = Generator.Invoke();
                if (bmp == null) break;
                this.Invoke(new InvokeUpdate(()=> { this.ShowBitmap(bmp); }));
            }
        }

        private void ShowBitmap(Bitmap bmp)
        {
            Bitmap oldBitmap = this.imageBox.Image as Bitmap;
            this.imageBox.Image = bmp;
            if (oldBitmap != null) oldBitmap.Dispose();
        }
    }
}
