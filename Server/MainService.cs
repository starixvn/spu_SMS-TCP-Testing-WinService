using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;

namespace Server
{
    partial class MainService : ServiceBase
    {
        bool blnStartStop;
        ServiceHost host;
        ChatService cs;

        public MainService()
        {
            InitializeComponent();

            cs = new ChatService();
        }

        protected override void OnStart(string[] args)
        {
            host = new ServiceHost(typeof(Server.ChatService));

            if (host.State == CommunicationState.Closed)
            {
                host.Open();
            }
        }

        protected override void OnStop()
        {
            cs.Close();
            host.Close();
        }
    }
}
