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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
        this.SaveFileDialog_SaveMoCap = new System.Windows.Forms.SaveFileDialog();
        this.PictureBox_Working = new System.Windows.Forms.PictureBox();
        this.Button_Record = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Working)).BeginInit();
        this.SuspendLayout();
        // 
        // SaveFileDialog_SaveMoCap
        // 
        this.SaveFileDialog_SaveMoCap.DefaultExt = "mocap";
        this.SaveFileDialog_SaveMoCap.FileName = "MoCap";
        this.SaveFileDialog_SaveMoCap.Filter = "Motion Capture Data Files|*.mocap";
        // 
        // PictureBox_Working
        // 
        this.PictureBox_Working.Location = new System.Drawing.Point(88, 12);
        this.PictureBox_Working.Name = "PictureBox_Working";
        this.PictureBox_Working.Size = new System.Drawing.Size(70, 70);
        this.PictureBox_Working.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.PictureBox_Working.TabIndex = 1;
        this.PictureBox_Working.TabStop = false;
        // 
        // Button_Record
        // 
        this.Button_Record.AutoSize = true;
        this.Button_Record.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.Button_Record.Image = global::Reflecta.Server.UI.Properties.Resources.Record;
        this.Button_Record.Location = new System.Drawing.Point(12, 12);
        this.Button_Record.Name = "Button_Record";
        this.Button_Record.Size = new System.Drawing.Size(70, 70);
        this.Button_Record.TabIndex = 0;
        this.Button_Record.UseVisualStyleBackColor = true;
        this.Button_Record.Click += new System.EventHandler(this.Button_Record_Click);
        // 
        // Form_Main
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoSize = true;
        this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.ClientSize = new System.Drawing.Size(184, 111);
        this.Controls.Add(this.PictureBox_Working);
        this.Controls.Add(this.Button_Record);
        this.DoubleBuffered = true;
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "Form_Main";
        this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        this.Text = "Server";
        this.TopMost = true;
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
        this.Load += new System.EventHandler(this.Form_Main_Load);
        ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Working)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button Button_Record;
    private System.Windows.Forms.SaveFileDialog SaveFileDialog_SaveMoCap;
    private System.Windows.Forms.PictureBox PictureBox_Working;

}

