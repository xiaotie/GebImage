using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Geb.Image
{
    using Geb.Image.WinForm;

    public class GeneratorWarpper
    {
        private Func<IImage> _func;
        public GeneratorWarpper(Func<IImage> func)
        {
            _func = func;
        }

        public Bitmap Generate()
        {
            var img = _func.Invoke();
            return img == null ? null : img.ToBitmap();
        }
    }

    public static class BitmapHelper
    {
        public static System.Windows.Forms.DialogResult ShowDialog(this Bitmap bmp, String title = null)
        {
            if (bmp == null) return System.Windows.Forms.DialogResult.Cancel;

            FrmImage frm = new FrmImage();
            frm.Bitmap = bmp;
            if (title != null) { frm.Text = title; }
            return frm.ShowDialog();
        }

        public static System.Windows.Forms.DialogResult ShowDialog(this IImage image, String title = null)
        {
            if (image == null) return System.Windows.Forms.DialogResult.Cancel;
            return image.ToBitmap().ShowDialog(title);
        }

        public static System.Windows.Forms.DialogResult ShowDialog(this Func<IImage> generator, String title = null)
        {
            return ShowDialog(new GeneratorWarpper(generator).Generate, title);
        }

        public static System.Windows.Forms.DialogResult ShowDialog(this Func<Bitmap> generator, String title = null)
        {
            if (generator == null) return System.Windows.Forms.DialogResult.Cancel;

            FrmImage frm = new FrmImage();
            frm.Generator = generator;
            if (title != null) { frm.Text = title; }
            return frm.ShowDialog();
        }
    }
}
