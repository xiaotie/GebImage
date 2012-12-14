using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Image.Demo
{
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
    }
}
