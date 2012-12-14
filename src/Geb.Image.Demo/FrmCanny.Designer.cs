namespace Geb.Image.Demo
{
    partial class FrmCanny
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
            this.htnOpen = new System.Windows.Forms.Button();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.pbMain = new System.Windows.Forms.PictureBox();
            this.trackSigma = new System.Windows.Forms.TrackBar();
            this.trackSize = new System.Windows.Forms.TrackBar();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSigma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSize)).BeginInit();
            this.SuspendLayout();
            // 
            // htnOpen
            // 
            this.htnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.htnOpen.Location = new System.Drawing.Point(488, 12);
            this.htnOpen.Name = "htnOpen";
            this.htnOpen.Size = new System.Drawing.Size(75, 23);
            this.htnOpen.TabIndex = 0;
            this.htnOpen.Text = "Open ...";
            this.htnOpen.UseVisualStyleBackColor = true;
            this.htnOpen.Click += new System.EventHandler(this.htnOpen_Click);
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(13, 13);
            this.tbPath.Name = "tbPath";
            this.tbPath.ReadOnly = true;
            this.tbPath.Size = new System.Drawing.Size(469, 21);
            this.tbPath.TabIndex = 1;
            this.tbPath.TabStop = false;
            // 
            // pbMain
            // 
            this.pbMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbMain.Location = new System.Drawing.Point(13, 41);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(469, 326);
            this.pbMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbMain.TabIndex = 2;
            this.pbMain.TabStop = false;
            // 
            // trackSigma
            // 
            this.trackSigma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.trackSigma.Location = new System.Drawing.Point(47, 373);
            this.trackSigma.Maximum = 5;
            this.trackSigma.Minimum = 1;
            this.trackSigma.Name = "trackSigma";
            this.trackSigma.Size = new System.Drawing.Size(180, 45);
            this.trackSigma.TabIndex = 3;
            this.trackSigma.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackSigma.Value = 2;
            // 
            // trackSize
            // 
            this.trackSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.trackSize.Location = new System.Drawing.Point(309, 373);
            this.trackSize.Maximum = 21;
            this.trackSize.Minimum = 3;
            this.trackSize.Name = "trackSize";
            this.trackSize.Size = new System.Drawing.Size(161, 45);
            this.trackSize.TabIndex = 4;
            this.trackSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackSize.Value = 3;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(488, 376);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 5;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 376);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Sigma:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(277, 375);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "Size:";
            // 
            // FrmCanny
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 411);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.trackSize);
            this.Controls.Add(this.trackSigma);
            this.Controls.Add(this.pbMain);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.htnOpen);
            this.Name = "FrmCanny";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Canny Edge Detector";
            this.Load += new System.EventHandler(this.FrmCanny_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSigma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button htnOpen;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.PictureBox pbMain;
        private System.Windows.Forms.TrackBar trackSigma;
        private System.Windows.Forms.TrackBar trackSize;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}