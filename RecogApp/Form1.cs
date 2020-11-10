using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Util;
using OpenCvSharp.Extensions;

namespace RecogApp
{
    public partial class Form1 : Form
    {
        //delegate
        public delegate void delegate_buttonstatus(string str);
        public delegate_buttonstatus m_delegate_buttonstatus;
        public delegate void delegate_drawimage(Bitmap bmp);
        public delegate_drawimage m_delegate_drawimage;

        public Form1()
        {
            InitializeComponent();
            //move to center
            CenterToScreen();

            Global.g_frmMain = this;
            //deletegate
            m_delegate_buttonstatus = new delegate_buttonstatus(this.buttonstatus);
            m_delegate_drawimage = new delegate_drawimage(this.draw_image);
        }

        //draw image into picturebox
        public void draw_image(Bitmap bmp)
        {
            try
            {
                picMain.Image = bmp;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
                
        public void my_draw_image(Bitmap bmp)
        {
            try
            {
                Global.g_frmMain.Invoke(Global.g_frmMain.m_delegate_drawimage, bmp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //change the labal of button
        public void buttonstatus(string str)
        {
            try
            {
                btnStart.Text = str;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
                
        public void my_buttonstatus(string str)
        {
            try
            {
                Global.g_frmMain.Invoke(Global.g_frmMain.m_delegate_buttonstatus, str);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //Open File Browser Window
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(f.FileName))
                {
                    Console.WriteLine(f.FileName);
                    txtFile.Text = f.FileName;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Read video file path
            string videofile = txtFile.Text;
            if (string.IsNullOrEmpty(videofile))
                return;

            if (Global.g_bThreadRunning)
            {
                Global.g_bWorkStop = true;
                my_buttonstatus("Stopping");
                return;
            }

            my_buttonstatus("Stop");
            Global.g_bWorkStop = false;
            Global.g_bThreadRunning = true;

            //Run main thread
            Global.g_thrMainThread = new Thread(delegate ()
            {
                main_proc(videofile);
            });
            Global.g_thrMainThread.Start();
        }

        //Convert mat to bitmap
        public static Bitmap MatToBitmap(Mat image)
        {
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
        }

        //Thread Main
        [STAThread]
        public void main_proc(string videofile)
        {
            VideoCapture capture = new VideoCapture(videofile);
            int delay = (int)Math.Round(1000 / capture.Fps);

            using (Mat image = new Mat())
            {
                while (true)
                {
                    //Read frame from video
                    capture.Read(image);
                    if (image.Empty())
                        break;

                    //Put rectangle in frame
                    image.Rectangle(new Rect(10, 10, 80, 80), Scalar.Red, 1);

                    //Put text in frame
                    image.PutText("Recog App", new OpenCvSharp.Point(image.Rows - 100, 50), HersheyFonts.HersheySimplex, 0.7, Scalar.Blue);

                    //Convert mat to bitmap
                    Bitmap bitimg = MatToBitmap(image);

                    //Draw bitmap into picturebox
                    my_draw_image(bitimg);

                    //Delay
                    Cv2.WaitKey(delay);

                    //When click stop button, then exit immediately
                    if (Global.g_bWorkStop)
                        break;
                }
            }

            //initialize
            Global.g_bThreadRunning = false;
            Global.g_bWorkStop = true;
            my_buttonstatus("Start");
            Global.g_thrMainThread = null;
        }
    }
}
