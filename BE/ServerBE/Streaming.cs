using StreamLibrary.UnsafeCodecs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ServerBE
{
    public partial class Streaming : Form
    {
        Task t;
        public Streaming()
        {

            InitializeComponent();
            this.checkedListBox1.Items.Add("Subscribers");
            this.checkedListBox1.Items.Add("Followers");;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task t=Task.Factory.StartNew(() => Stream());


        }

        private void Stream() {
            int FPS = 0;
            Stopwatch sw = Stopwatch.StartNew();
            Stopwatch RenderSW = Stopwatch.StartNew();

            try
            {

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect("localhost", 4432);
                StreamLibrary.IUnsafeCodec unsafeCodec = new UnsafeStreamCodec(80);



                while (true)
                {
                    Bitmap bmp = CaptureScreen();
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    Size size = new System.Drawing.Size(bmp.Width, bmp.Height);
                    BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                    if (RenderSW.ElapsedMilliseconds >= (1000 / 20))
                    {
                        this.pictureBox1.Image = (Bitmap)bmp.Clone();
                        RenderSW = Stopwatch.StartNew();
                    }

                    FPS++;
                    using (MemoryStream stream = new MemoryStream(10000000))
                    {
                        unsafeCodec.CodeImage(bmpData.Scan0, rect, size, bmp.PixelFormat, stream);

                        if (stream.Length > 0)
                        {


                            socket.Send(BitConverter.GetBytes((int)stream.Length));
                            socket.Send(stream.GetBuffer(), (int)stream.Length, SocketFlags.None);
                        }
                    }
                    bmp.UnlockBits(bmpData);
                    bmp.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
        private static Bitmap CaptureScreen()
        {
            Rectangle rect = Screen.AllScreens[0].WorkingArea;

            try
            {
                Bitmap bmpScreenshot = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
                Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                gfxScreenshot.CopyFromScreen(0, 0, 0, 0, new Size(bmpScreenshot.Width, bmpScreenshot.Height), CopyPixelOperation.SourceCopy);
                gfxScreenshot.Dispose();
                return bmpScreenshot;
            }
            catch { return new Bitmap(rect.Width, rect.Height); }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}

