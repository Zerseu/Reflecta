using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RGBAMerge
{
    public partial class Form_Main : Form
    {
        public Form_Main()
        {
            InitializeComponent();
        }

        private void Button_RGB_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog_OpenPicture.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PictureBox_RGB.Image = Image.FromFile(OpenFileDialog_OpenPicture.FileName);
        }
        private void Button_A_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog_OpenPicture.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PictureBox_A.Image = Image.FromFile(OpenFileDialog_OpenPicture.FileName);
        }
        private void Button_Merge_Click(object sender, EventArgs e)
        {
            if (PictureBox_RGB.Image == null || PictureBox_A.Image == null)
            {
                MessageBox.Show("Please select RGB and A (Gloss) images!");
                return;
            }

            Bitmap RGB = PictureBox_RGB.Image as Bitmap;
            Bitmap A = PictureBox_A.Image as Bitmap;

            if (RGB.Width != A.Width || RGB.Height != A.Height)
            {
                MessageBox.Show("Size mismatch!");
                return;
            }

            int width = RGB.Width;
            int height = RGB.Height;
            Bitmap RGBA = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    RGBA.SetPixel(x, y, Color.FromArgb(A.GetPixel(x, y).R, RGB.GetPixel(x, y)));

            if (SaveFileDialog_SavePicture.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                RGBA.Save(SaveFileDialog_SavePicture.FileName, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
