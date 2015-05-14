#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Urbetrack.Toolkit;

#endregion

namespace SimPumpDC
{
    public partial class Form1 : Form
    {
        private static readonly TcpListener tcpListener = new TcpListener(IPAddress.Any, 1024);
        private readonly Thread task = new Thread(Proc);

        static void Proc()
        {
            tcpListener.Start();
            while (true)
            {
                var socket = tcpListener.AcceptSocket();
                var responseString = string.Format("@lero lero respuesta opaca... {0}@", DateTime.Now);
                var sendBytes = Encoding.ASCII.GetBytes(responseString);
                var b = new byte[1024];
                socket.ReceiveTimeout = 3000;
                socket.Receive(b);
                socket.Send(sendBytes);
                T.TRACE("Message Sent /> : " + responseString);
                Thread.Sleep(2000);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            task.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            task.Abort();
        }
    }
}
