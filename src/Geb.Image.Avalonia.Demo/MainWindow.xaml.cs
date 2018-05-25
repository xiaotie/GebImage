// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Geb.Image.Avalonia.Demo
{
    public class MainWindow : Window
    {
        public MainWindow()
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
            context.FillRectangle(new SolidColorBrush(Colors.Blue), new Rect(0,0,ClientSize.Width,ClientSize.Height));
            
        }
    }
}
