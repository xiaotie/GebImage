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
            this.btnDistanceTransformDemo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCannyDeno
            // 
            this.btnCannyDeno.Location = new System.Drawing.Point(20, 20);
            this.btnCannyDeno.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCannyDeno.Name = "btnCannyDeno";
            this.btnCannyDeno.Size = new System.Drawing.Size(216, 46);
            this.btnCannyDeno.TabIndex = 0;
            this.btnCannyDeno.Text = "Canny Edge Detector";
            this.btnCannyDeno.UseVisualStyleBackColor = true;
            this.btnCannyDeno.Click += new System.EventHandler(this.btnCannyDeno_Click);
            // 
            // btnPrimaryColor
            // 
            this.btnPrimaryColor.Location = new System.Drawing.Point(20, 75);
            this.btnPrimaryColor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPrimaryColor.Name = "btnPrimaryColor";
            this.btnPrimaryColor.Size = new System.Drawing.Size(216, 46);
            this.btnPrimaryColor.TabIndex = 1;
            this.btnPrimaryColor.Text = "Primary Color";
            this.btnPrimaryColor.UseVisualStyleBackColor = true;
            this.btnPrimaryColor.Click += new System.EventHandler(this.btnPrimaryColor_Click);
            // 
            // btnDistanceTransformDemo
            // 
            this.btnDistanceTransformDemo.Location = new System.Drawing.Point(20, 129);
            this.btnDistanceTransformDemo.Margin = new System.Windows.Forms.Padding(4);
            this.btnDistanceTransformDemo.Name = "btnDistanceTransformDemo";
            this.btnDistanceTransformDemo.Size = new System.Drawing.Size(216, 46);
            this.btnDistanceTransformDemo.TabIndex = 2;
            this.btnDistanceTransformDemo.Text = "Distance Transform";
            this.btnDistanceTransformDemo.UseVisualStyleBackColor = true;
            this.btnDistanceTransformDemo.Click += new System.EventHandler(this.btnDistanceTransformDemo_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 684);
            this.Controls.Add(this.btnDistanceTransformDemo);
            this.Controls.Add(this.btnPrimaryColor);
            this.Controls.Add(this.btnCannyDeno);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCannyDeno;
        private System.Windows.Forms.Button btnPrimaryColor;
        private System.Windows.Forms.Button btnDistanceTransformDemo;
    }
}

