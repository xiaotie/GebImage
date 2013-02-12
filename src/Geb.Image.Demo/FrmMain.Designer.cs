namespace Geb.Image.Demo
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
            this.btnCannyDeno = new System.Windows.Forms.Button();
            this.btnPrimaryColor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCannyDeno
            // 
            this.btnCannyDeno.Location = new System.Drawing.Point(13, 13);
            this.btnCannyDeno.Name = "btnCannyDeno";
            this.btnCannyDeno.Size = new System.Drawing.Size(144, 31);
            this.btnCannyDeno.TabIndex = 0;
            this.btnCannyDeno.Text = "Canny Edge Detector";
            this.btnCannyDeno.UseVisualStyleBackColor = true;
            this.btnCannyDeno.Click += new System.EventHandler(this.btnCannyDeno_Click);
            // 
            // btnPrimaryColor
            // 
            this.btnPrimaryColor.Location = new System.Drawing.Point(13, 50);
            this.btnPrimaryColor.Name = "btnPrimaryColor";
            this.btnPrimaryColor.Size = new System.Drawing.Size(144, 31);
            this.btnPrimaryColor.TabIndex = 1;
            this.btnPrimaryColor.Text = "主色调测试";
            this.btnPrimaryColor.UseVisualStyleBackColor = true;
            this.btnPrimaryColor.Click += new System.EventHandler(this.btnPrimaryColor_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 456);
            this.Controls.Add(this.btnPrimaryColor);
            this.Controls.Add(this.btnCannyDeno);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCannyDeno;
        private System.Windows.Forms.Button btnPrimaryColor;
    }
}

