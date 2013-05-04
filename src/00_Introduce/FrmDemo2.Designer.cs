namespace Introduce
{
    partial class FrmDemo2
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
            this.btnTest0 = new System.Windows.Forms.Button();
            this.btnTest1 = new System.Windows.Forms.Button();
            this.btnTest2 = new System.Windows.Forms.Button();
            this.btnTest3 = new System.Windows.Forms.Button();
            this.btnTest4 = new System.Windows.Forms.Button();
            this.btnTest5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTest0
            // 
            this.btnTest0.Location = new System.Drawing.Point(29, 28);
            this.btnTest0.Name = "btnTest0";
            this.btnTest0.Size = new System.Drawing.Size(212, 74);
            this.btnTest0.TabIndex = 0;
            this.btnTest0.Text = "操作托管堆上的值类型";
            this.btnTest0.UseVisualStyleBackColor = true;
            this.btnTest0.Click += new System.EventHandler(this.btnTest0_Click);
            // 
            // btnTest1
            // 
            this.btnTest1.Location = new System.Drawing.Point(29, 118);
            this.btnTest1.Name = "btnTest1";
            this.btnTest1.Size = new System.Drawing.Size(212, 75);
            this.btnTest1.TabIndex = 1;
            this.btnTest1.Text = "操作栈上的值类型";
            this.btnTest1.UseVisualStyleBackColor = true;
            this.btnTest1.Click += new System.EventHandler(this.btnTest1_Click);
            // 
            // btnTest2
            // 
            this.btnTest2.Location = new System.Drawing.Point(29, 211);
            this.btnTest2.Name = "btnTest2";
            this.btnTest2.Size = new System.Drawing.Size(212, 75);
            this.btnTest2.TabIndex = 2;
            this.btnTest2.Text = "操作非托管堆上的值类型";
            this.btnTest2.UseVisualStyleBackColor = true;
            this.btnTest2.Click += new System.EventHandler(this.btnTest2_Click);
            // 
            // btnTest3
            // 
            this.btnTest3.Location = new System.Drawing.Point(271, 28);
            this.btnTest3.Name = "btnTest3";
            this.btnTest3.Size = new System.Drawing.Size(212, 74);
            this.btnTest3.TabIndex = 3;
            this.btnTest3.Text = "测试Dispose模式";
            this.btnTest3.UseVisualStyleBackColor = true;
            this.btnTest3.Click += new System.EventHandler(this.btnTest3_Click);
            // 
            // btnTest4
            // 
            this.btnTest4.Location = new System.Drawing.Point(271, 118);
            this.btnTest4.Name = "btnTest4";
            this.btnTest4.Size = new System.Drawing.Size(212, 75);
            this.btnTest4.TabIndex = 4;
            this.btnTest4.Text = "测试stackalloc";
            this.btnTest4.UseVisualStyleBackColor = true;
            this.btnTest4.Click += new System.EventHandler(this.btnTest4_Click);
            // 
            // btnTest5
            // 
            this.btnTest5.Location = new System.Drawing.Point(271, 211);
            this.btnTest5.Name = "btnTest5";
            this.btnTest5.Size = new System.Drawing.Size(212, 75);
            this.btnTest5.TabIndex = 5;
            this.btnTest5.Text = "模拟Union";
            this.btnTest5.UseVisualStyleBackColor = true;
            this.btnTest5.Click += new System.EventHandler(this.btnTest5_Click);
            // 
            // FrmDemo2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 353);
            this.Controls.Add(this.btnTest5);
            this.Controls.Add(this.btnTest4);
            this.Controls.Add(this.btnTest3);
            this.Controls.Add(this.btnTest2);
            this.Controls.Add(this.btnTest1);
            this.Controls.Add(this.btnTest0);
            this.Name = "FrmDemo2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "指针使用简介";
            this.Load += new System.EventHandler(this.FrmDemo2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTest0;
        private System.Windows.Forms.Button btnTest1;
        private System.Windows.Forms.Button btnTest2;
        private System.Windows.Forms.Button btnTest3;
        private System.Windows.Forms.Button btnTest4;
        private System.Windows.Forms.Button btnTest5;
    }
}