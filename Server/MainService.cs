using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace Server
{
    partial class MainService : ServiceBase
    {
        //bool blnStartStop;
        //ServiceHost host;
        //ChatService cs;

        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        TcpListener ServerSocket;

        public MainService()
        {
            InitializeComponent();

           // cs = new ChatService();
        }

        protected override void OnStart(string[] args)
        {
            //host = new ServiceHost(typeof(Server.ChatService));

            //if (host.State == CommunicationState.Closed)
            //{
            //    host.Open();
            //}

            Thread serverThread = new Thread(new ThreadStart(StartService));

            serverThread.Start();
        }

        protected override void OnStop()
        {
            //cs.Close();
            //host.Close();
        }

        private void StartService()
        {
            int count = 1;
            ServerSocket = new TcpListener(IPAddress.Any, 1234);
            ServerSocket.Start();

            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                lock (_lock) list_clients.Add(count, client);

                Thread t = new Thread(handle_clients);
                t.Start(count);
                count++;
            }
        }

        public void handle_clients(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock) client = list_clients[id];

            while (true)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int byte_count = stream.Read(buffer, 0, buffer.Length);

                    if (byte_count == 0)
                    {
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, byte_count);
                    broadcast(client, data);
                }
                catch
                {
                    lock (_lock) list_clients.Remove(id);
                    client.Close();
                }
            }

            lock (_lock) list_clients.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        public void broadcast(TcpClient client, string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data + Environment.NewLine);

            lock (_lock)
            {
                foreach (TcpClient c in list_clients.Values)
                {
                    if (client != c)
                    {
                        NetworkStream stream = c.GetStream();

                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }
    }
}
