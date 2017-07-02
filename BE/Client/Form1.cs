using StreamLibrary;
using StreamLibrary.UnsafeCodecs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private delegate void Invoky();
       

        private Socket Server;
        public Form1()
        {
            InitializeComponent();
            

            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Server.Bind(new IPEndPoint(0, 4432));
            Server.Listen(10);
            Server.BeginAccept(BeginAccept_Callback, null);
        }

        private void BeginAccept_Callback(IAsyncResult ar)
        {
            try
            {
                Socket sock = Server.EndAccept(ar);
                IUnsafeCodec decoder = new UnsafeStreamCodec(80);
                int FPS = 0;
                Stopwatch sw = Stopwatch.StartNew();
                Stopwatch RenderSW = Stopwatch.StartNew();

                while (sock.Connected)
                {
                    //keep receiving data
                    byte[] Header = ReceiveData(4, sock);
                    if (Header.Length != 4)
                        break;

                    int length = BitConverter.ToInt32(Header, 0);

                    byte[] Payload = ReceiveData(length, sock);
                    if (Payload.Length != length)
                        break;

                    Bitmap decoded = decoder.DecodeData(new MemoryStream(Payload));

                    if (RenderSW.ElapsedMilliseconds >= (1000 / 20))
                    {
                        this.pictureBox1.Image = (Bitmap)decoded.Clone();
                        RenderSW = Stopwatch.StartNew();
                    }

                    FPS++;
                
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            Server.BeginAccept(BeginAccept_Callback, null);
        }

        private byte[] ReceiveData(int Length, Socket socket)
        {
            byte[] data = new byte[Length];
            int offset = 0;

            while (Length > 0)
            {
                int recv = socket.Receive(data, offset, Length, SocketFlags.None);

                if (recv <= 0)
                    return new byte[0];

                offset += recv;
                Length -= recv;
            }
            return data;
        }
    }
}
