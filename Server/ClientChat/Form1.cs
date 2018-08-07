using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientChat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TcpClient client;
        Thread thread;
        NetworkStream ns;
        private void Form1_Load(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;
            client = new TcpClient();
            client.Connect(ip, port);
            //Console.WriteLine("client connected!!");
            
            thread = new Thread(o => ReceiveData((TcpClient)o));

            thread.Start(client);

            //string s;
            //while (!string.IsNullOrEmpty((s = Console.ReadLine())))
            //{
            //    byte[] buffer = Encoding.ASCII.GetBytes(s);
            //    ns.Write(buffer, 0, buffer.Length);
            //}

           
           // ns.Close();
            
           // Console.WriteLine("disconnect from server!!");
           // Console.ReadKey();
        }

        void ReceiveData(TcpClient client)
        {
            ns = client.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {                
                listMessages.Text = listMessages.Text.Insert(0, Environment.NewLine + "-->> " + Encoding.UTF8.GetString(receivedBytes, 0, byte_count) + Environment.NewLine + "--------");
            }
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;

            if (keyValue == 13)
            {              
                SendMessage(txtMessage.Text);
                txtMessage.Text = string.Empty;
            }
        }

        private void SendMessage(string message)
        {
            ns = client.GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            ns.Write(buffer, 0, buffer.Length);
            listMessages.Text = listMessages.Text.Insert(0, Environment.NewLine + "<<-- " + message + Environment.NewLine + "--------");
           // ns.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Client.Shutdown(SocketShutdown.Send);
            thread.Join();
            client.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMessage(txtMessage.Text);
            txtMessage.Text = string.Empty;
        }
    }
}
