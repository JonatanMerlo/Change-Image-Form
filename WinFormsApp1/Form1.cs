using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
        }



        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    

                    string imagen = openFileDialog1.FileName;
                    pictureBox1.Image = Image.FromFile(imagen);
                    checkBox1.Checked = true;
                    checkBox1.Visible = true;
                    checkBox1.Enabled = false;
                    radioButton1.Enabled = true;
                    radioButton2.Enabled = true;
                    textBox1.Enabled = true;
                    button2.Enabled = true;


                }
            }
            catch
            {
                MessageBox.Show("El archivo no es del tipo correcto");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public double anchoFinal(double ancho, double alto, double altoDeseado)
        {

            double constanteDeMultiplicacion = altoDeseado / alto;

            double anchoFinalImagen = constanteDeMultiplicacion * ancho;

            return anchoFinalImagen;

        }

        public double altoFinal(double ancho, double alto, double anchoDeseado)
        {

            double constanteDeMultiplicacion = anchoDeseado / ancho;

            double altoFinalImagen = constanteDeMultiplicacion * alto;

            return altoFinalImagen;

        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if(textBox1.TextLength > 0)
                {

                   double tamañoDeseado = Convert.ToDouble(textBox1.Text);
                    string imagen = openFileDialog1.FileName;
                    double imagenAncho = Convert.ToDouble(Image.FromFile(imagen).Width);
                    double imagenAlto = Convert.ToDouble(Image.FromFile(imagen).Height);


                    if (radioButton1.Checked)
                    {
                        try
                        {
                            Int32 altoDeseado = Convert.ToInt32(textBox1.Text);
                            Int32 anchoFinalDeseado = Convert.ToInt32(anchoFinal(imagenAncho, imagenAlto, altoDeseado));

                            Image img = Image.FromFile(imagen);
                            Image imagenRenderizada = img.ResizeProportional(anchoFinalDeseado,altoDeseado);

                            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Png Image|*.png";
                            saveFileDialog1.Title = "Guardar un archivo de imagen";

                            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                MemoryStream ms = new MemoryStream();
                                imagenRenderizada.Save(saveFileDialog1.FileName);  
                            }

                        }
                        catch
                        {
                            MessageBox.Show("El tipo de dato no es correcto, prueba ingresando un valor numérico entero");
                        }

                    }

                    if (radioButton2.Checked)
                    {
                        try
                        {
                            Int32 anchoDeseado = Convert.ToInt32(textBox1.Text);
                            Int32 altoFinalDeseado = Convert.ToInt32(altoFinal(imagenAlto, imagenAncho, anchoDeseado));

                            Image img = Image.FromFile(imagen);
                            Image imagenRenderizada = img.ResizeProportional(anchoDeseado, altoFinalDeseado);

                            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Png Image|*.png";
                            saveFileDialog1.Title = "Guardar un archivo de imagen";

                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                MemoryStream ms = new MemoryStream();
                                imagenRenderizada.Save(saveFileDialog1.FileName);
                            }

                        }
                        catch
                        {
                            MessageBox.Show("El tipo de dato no es correcto, prueba ingresando un valor numérico entero");
                        }

                    }
                    if(!radioButton1.Checked && !radioButton2.Checked)
                    {
                        MessageBox.Show("Seleccione una opcion");
                    }

                }
                else
                {
                    MessageBox.Show("El campo no puede estar vacio");
                }
            }
            catch
            {
                MessageBox.Show("El campo no puede estar vacio");
            }
        }

    }
        public static class ImageExt
        {
            public static Image Resize(this Image img, int srcX, int srcY, int srcWidth, int srcHeight, int dstWidth, int dstHeight)
            {
                var bmp = new Bitmap(dstWidth, dstHeight);
                using (var graphics = Graphics.FromImage(bmp))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        var destRect = new Rectangle(0, 0, dstWidth, dstHeight);
                        graphics.DrawImage(img, destRect, srcX, srcY, srcWidth, srcHeight, GraphicsUnit.Pixel, wrapMode);
                    }
                }
                return bmp;
            }

            public static Image ResizeProportional(this Image img, int width, int height, bool enlarge = false)
            {
                double ratio = Math.Max(img.Width / (double)width, img.Height / (double)height);
                if (ratio < 1 && !enlarge) return img;
                return img.Resize(0, 0, img.Width, img.Height, Convert.ToInt32(Math.Round(img.Width / ratio)), Convert.ToInt32(Math.Round(img.Height / ratio)));
            }

            public static Image ResizeCropExcess(this Image img, int dstWidth, int dstHeight)
            {
                double srcRatio = img.Width / (double)img.Height;
                double dstRatio = dstWidth / (double)dstHeight;
                int srcX, srcY, cropWidth, cropHeight;

                if (srcRatio < dstRatio) // trim top and bottom
                {
                    cropHeight = dstHeight * img.Width / dstWidth;
                    srcY = (img.Height - cropHeight) / 2;
                    cropWidth = img.Width;
                    srcX = 0;
                }
                else // trim left and right
                {
                    cropWidth = dstWidth * img.Height / dstHeight;
                    srcX = (img.Width - cropWidth) / 2;
                    cropHeight = img.Height;
                    srcY = 0;
                }

                return Resize(img, srcX, srcY, cropWidth, cropHeight, dstWidth, dstHeight);
            }
        }
}
