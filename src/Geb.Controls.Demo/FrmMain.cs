using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Controls.Demo
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            for(int h = 0; h < 10; h++)
            {
                for (int w = 0; w < 10; w++)
                {
                    Button btn = new Button();
                    btn.Width = 30;
                    btn.Text = h.ToString() + "," + w.ToString();
                    btn.X = w * 40;
                    btn.Y = h * 40;
                    this.container.Add(btn);
                }
            }

            this.container.InitEvents();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
