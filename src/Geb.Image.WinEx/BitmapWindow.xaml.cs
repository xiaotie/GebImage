using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Geb.Image.WinEx
{
    /// <summary>
    /// Interaction logic for BitmapWindow.xaml
    /// </summary>
    public partial class BitmapWindow : Window
    {
        internal System.Drawing.Bitmap Bitmap { get; set; }

        public BitmapWindow()
        {
            InitializeComponent();
        }
    }
}
