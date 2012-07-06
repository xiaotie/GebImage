namespace Introduce
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRun = new System.Windows.Forms.Button();
            this.btnRun2 = new System.Windows.Forms.Button();
            this.btnDemo1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(198, 115);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "运行";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnRun2
            // 
            this.btnRun2.Location = new System.Drawing.Point(198, 178);
            this.btnRun2.Name = "btnRun2";
            this.btnRun2.Size = new System.Drawing.Size(75, 23);
            this.btnRun2.TabIndex = 1;
            this.btnRun2.Text = "运行2";
            this.btnRun2.UseVisualStyleBackColor = true;
            this.btnRun2.Click += new System.EventHandler(this.btnRun2_Click);
            // 
            // btnDemo1
            // 
            this.btnDemo1.Location = new System.Drawing.Point(12, 12);
            this.btnDemo1.Name = "btnDemo1";
            this.btnDemo1.Size = new System.Drawing.Size(180, 23);
            this.btnDemo1.TabIndex = 2;
            this.btnDemo1.Text = "演示1:  聆听代码的歌声";
            this.btnDemo1.UseVisualStyleBackColor = true;
            this.btnDemo1.Click += new System.EventHandler(this.btnDemo1_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 447);
            this.Controls.Add(this.btnDemo1);
            this.Controls.Add(this.btnRun2);
            this.Controls.Add(this.btnRun);
            this.Name = "FrmMain";
            this.Text = "C# 指针编程";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnRun2;
        private System.Windows.Forms.Button btnDemo1;
    }
}

