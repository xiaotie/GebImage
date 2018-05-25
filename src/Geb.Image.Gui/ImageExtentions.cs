using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public static class ImageExtentions
    {
        public static void ShowDialog(this ImageArgb32 image)
        {
            Gui.Controls.ImageWindow window = new Gui.Controls.ImageWindow();
            window.ShowDialog();
        }
    }
}
