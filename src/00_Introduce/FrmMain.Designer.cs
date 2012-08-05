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
            this.btnDemo1 = new System.Windows.Forms.Button();
            this.btnDemo2 = new System.Windows.Forms.Button();
            this.btnDemo3 = new System.Windows.Forms.Button();
            this.btnDemo4 = new System.Windows.Forms.Button();
            this.btnDemo5 = new System.Windows.Forms.Button();
            this.btnTest6 = new System.Windows.Forms.Button();
            this.btnDemo7 = new System.Windows.Forms.Button();
            this.lk = new System.Windows.Forms.LinkLabel();
            this.btnDemo8 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDemo1
            // 
            this.btnDemo1.Location = new System.Drawing.Point(239, 51);
            this.btnDemo1.Name = "btnDemo1";
            this.btnDemo1.Size = new System.Drawing.Size(180, 41);
            this.btnDemo1.TabIndex = 2;
            this.btnDemo1.Text = "演示1:  简洁优美的代码";
            this.btnDemo1.UseVisualStyleBackColor = true;
            this.btnDemo1.Click += new System.EventHandler(this.btnDemo1_Click);
            // 
            // btnDemo2
            // 
            this.btnDemo2.Location = new System.Drawing.Point(239, 98);
            this.btnDemo2.Name = "btnDemo2";
            this.btnDemo2.Size = new System.Drawing.Size(180, 41);
            this.btnDemo2.TabIndex = 3;
            this.btnDemo2.Text = "演示2:  指针使用简介";
            this.btnDemo2.UseVisualStyleBackColor = true;
            this.btnDemo2.Click += new System.EventHandler(this.btnDemo2_Click);
            // 
            // btnDemo3
            // 
            this.btnDemo3.Location = new System.Drawing.Point(239, 145);
            this.btnDemo3.Name = "btnDemo3";
            this.btnDemo3.Size = new System.Drawing.Size(180, 41);
            this.btnDemo3.TabIndex = 4;
            this.btnDemo3.Text = "演示3: C# 模板";
            this.btnDemo3.UseVisualStyleBackColor = true;
            this.btnDemo3.Click += new System.EventHandler(this.btnDemo3_Click);
            // 
            // btnDemo4
            // 
            this.btnDemo4.Location = new System.Drawing.Point(239, 192);
            this.btnDemo4.Name = "btnDemo4";
            this.btnDemo4.Size = new System.Drawing.Size(180, 41);
            this.btnDemo4.TabIndex = 5;
            this.btnDemo4.Text = "演示4: 测试迭代器1";
            this.btnDemo4.UseVisualStyleBackColor = true;
            this.btnDemo4.Click += new System.EventHandler(this.btnDemo4_Click);
            // 
            // btnDemo5
            // 
            this.btnDemo5.Location = new System.Drawing.Point(239, 239);
            this.btnDemo5.Name = "btnDemo5";
            this.btnDemo5.Size = new System.Drawing.Size(180, 41);
            this.btnDemo5.TabIndex = 6;
            this.btnDemo5.Text = "演示5: 测试迭代器2";
            this.btnDemo5.UseVisualStyleBackColor = true;
            this.btnDemo5.Click += new System.EventHandler(this.btnDemo5_Click);
            // 
            // btnTest6
            // 
            this.btnTest6.Location = new System.Drawing.Point(239, 286);
            this.btnTest6.Name = "btnTest6";
            this.btnTest6.Size = new System.Drawing.Size(180, 41);
            this.btnTest6.TabIndex = 7;
            this.btnTest6.Text = "演示6: 测试ROI迭代器";
            this.btnTest6.UseVisualStyleBackColor = true;
            this.btnTest6.Click += new System.EventHandler(this.btnTest6_Click);
            // 
            // btnDemo7
            // 
            this.btnDemo7.Location = new System.Drawing.Point(239, 333);
            this.btnDemo7.Name = "btnDemo7";
            this.btnDemo7.Size = new System.Drawing.Size(180, 41);
            this.btnDemo7.TabIndex = 8;
            this.btnDemo7.Text = "演示7: lambda表达式";
            this.btnDemo7.UseVisualStyleBackColor = true;
            this.btnDemo7.Click += new System.EventHandler(this.btnDemo7_Click);
            // 
            // lk
            // 
            this.lk.AutoSize = true;
            this.lk.Location = new System.Drawing.Point(454, 426);
            this.lk.Name = "lk";
            this.lk.Size = new System.Drawing.Size(191, 12);
            this.lk.TabIndex = 9;
            this.lk.TabStop = true;
            this.lk.Text = "By 集异璧实验室(www.geblab.com)";
            this.lk.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lk_LinkClicked);
            // 
            // btnDemo8
            // 
            this.btnDemo8.Location = new System.Drawing.Point(239, 380);
            this.btnDemo8.Name = "btnDemo8";
            this.btnDemo8.Size = new System.Drawing.Size(180, 41);
            this.btnDemo8.TabIndex = 10;
            this.btnDemo8.Text = "演示8: Width";
            this.btnDemo8.UseVisualStyleBackColor = true;
            this.btnDemo8.Click += new System.EventHandler(this.btnDemo8_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 447);
            this.Controls.Add(this.btnDemo8);
            this.Controls.Add(this.lk);
            this.Controls.Add(this.btnDemo7);
            this.Controls.Add(this.btnTest6);
            this.Controls.Add(this.btnDemo5);
            this.Controls.Add(this.btnDemo4);
            this.Controls.Add(this.btnDemo3);
            this.Controls.Add(this.btnDemo2);
            this.Controls.Add(this.btnDemo1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmMain";
            this.Text = "C# 指针编程演示（集异璧实验室，www.geblab.com）";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDemo1;
        private System.Windows.Forms.Button btnDemo2;
        private System.Windows.Forms.Button btnDemo3;
        private System.Windows.Forms.Button btnDemo4;
        private System.Windows.Forms.Button btnDemo5;
        private System.Windows.Forms.Button btnTest6;
        private System.Windows.Forms.Button btnDemo7;
        private System.Windows.Forms.LinkLabel lk;
        private System.Windows.Forms.Button btnDemo8;
    }
}

