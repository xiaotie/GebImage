using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Gui.Controls
{
    using Avalonia;
    using Avalonia.Media;
    using Avalonia.Controls;
    using Geb.Image;

    public class ImageWindow : Window
    {
        public ImageWindow()
        {
            this.InitializeComponent();
            this.AttachDevTools();
        }

        private void InitializeComponent()
        {
            this.Width = 600;
            this.Height = 400;
            this.Background = new SolidColorBrush(Colors.Blue);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            context.FillRectangle(new SolidColorBrush(Colors.Blue), new Avalonia.Rect(0, 0, ClientSize.Width, ClientSize.Height));
        }
    }
}
