namespace RGBAMerge
{
    partial class Form_Main
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
            this.Button_RGB = new System.Windows.Forms.Button();
            this.PictureBox_RGB = new System.Windows.Forms.PictureBox();
            this.OpenFileDialog_OpenPicture = new System.Windows.Forms.OpenFileDialog();
            this.PictureBox_A = new System.Windows.Forms.PictureBox();
            this.Button_A = new System.Windows.Forms.Button();
            this.Button_Merge = new System.Windows.Forms.Button();
            this.SaveFileDialog_SavePicture = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_RGB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_A)).BeginInit();
            this.SuspendLayout();
            // 
            // Button_RGB
            // 
            this.Button_RGB.AutoSize = true;
            this.Button_RGB.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Button_RGB.Location = new System.Drawing.Point(12, 12);
            this.Button_RGB.Name = "Button_RGB";
            this.Button_RGB.Size = new System.Drawing.Size(40, 23);
            this.Button_RGB.TabIndex = 0;
            this.Button_RGB.Text = "RGB";
            this.Button_RGB.UseVisualStyleBackColor = true;
            this.Button_RGB.Click += new System.EventHandler(this.Button_RGB_Click);
            // 
            // PictureBox_RGB
            // 
            this.PictureBox_RGB.Location = new System.Drawing.Point(12, 41);
            this.PictureBox_RGB.Name = "PictureBox_RGB";
            this.PictureBox_RGB.Size = new System.Drawing.Size(300, 300);
            this.PictureBox_RGB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox_RGB.TabIndex = 1;
            this.PictureBox_RGB.TabStop = false;
            // 
            // OpenFileDialog_OpenPicture
            // 
            this.OpenFileDialog_OpenPicture.DefaultExt = "png";
            this.OpenFileDialog_OpenPicture.FileName = "Image";
            this.OpenFileDialog_OpenPicture.Filter = "Image Files|*.jpg;*.png;*.bmp;*.gif";
            // 
            // PictureBox_A
            // 
            this.PictureBox_A.Location = new System.Drawing.Point(318, 41);
            this.PictureBox_A.Name = "PictureBox_A";
            this.PictureBox_A.Size = new System.Drawing.Size(300, 300);
            this.PictureBox_A.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox_A.TabIndex = 3;
            this.PictureBox_A.TabStop = false;
            // 
            // Button_A
            // 
            this.Button_A.AutoSize = true;
            this.Button_A.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Button_A.Location = new System.Drawing.Point(318, 12);
            this.Button_A.Name = "Button_A";
            this.Button_A.Size = new System.Drawing.Size(24, 23);
            this.Button_A.TabIndex = 2;
            this.Button_A.Text = "A";
            this.Button_A.UseVisualStyleBackColor = true;
            this.Button_A.Click += new System.EventHandler(this.Button_A_Click);
            // 
            // Button_Merge
            // 
            this.Button_Merge.AutoSize = true;
            this.Button_Merge.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Button_Merge.Location = new System.Drawing.Point(625, 376);
            this.Button_Merge.Name = "Button_Merge";
            this.Button_Merge.Size = new System.Drawing.Size(47, 23);
            this.Button_Merge.TabIndex = 4;
            this.Button_Merge.Text = "Merge";
            this.Button_Merge.UseVisualStyleBackColor = true;
            this.Button_Merge.Click += new System.EventHandler(this.Button_Merge_Click);
            // 
            // SaveFileDialog_SavePicture
            // 
            this.SaveFileDialog_SavePicture.DefaultExt = "png";
            this.SaveFileDialog_SavePicture.FileName = "Merged";
            this.SaveFileDialog_SavePicture.Filter = "Image Files|*.jpg;*.png;*.bmp;*.gif";
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 411);
            this.Controls.Add(this.Button_Merge);
            this.Controls.Add(this.PictureBox_A);
            this.Controls.Add(this.Button_A);
            this.Controls.Add(this.PictureBox_RGB);
            this.Controls.Add(this.Button_RGB);
            this.Name = "Form_Main";
            this.Text = "RGBA Merge";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_RGB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_A)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_RGB;
        private System.Windows.Forms.PictureBox PictureBox_RGB;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog_OpenPicture;
        private System.Windows.Forms.PictureBox PictureBox_A;
        private System.Windows.Forms.Button Button_A;
        private System.Windows.Forms.Button Button_Merge;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog_SavePicture;
    }
}

