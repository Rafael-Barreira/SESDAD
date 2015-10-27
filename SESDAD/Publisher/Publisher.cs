﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SESDAD
{
    class Publisher
    {

        internal static BrokerInterface broker;
        internal static string brokerURL;
        internal static string myURL;
        internal static int myPort;
        private string processname;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //TODO remove after PuppetMaster is implemented
            myURL = "tcp://localhost:8088/pub";
            //TODO remove after PuppetMaster is implemented
            myPort = 8088;

            TcpChannel channel = new TcpChannel(myPort);
            ChannelServices.RegisterChannel(channel, false);

            //TODO remove after PuppetMaster is implemented
            brokerURL = "tcp://localhost:8086/broker";

            broker = (BrokerInterface)Activator.GetObject(typeof(BrokerInterface), brokerURL);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemotePublisher),"pub",WellKnownObjectMode.Singleton);

            try
            {
                broker.ConnectPublisher(myURL);
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Broker");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PublisherForm());
        }

        public Publisher(string pubURL, string brkURL, int pubPort, string name)
        {
            myURL = pubURL;
            brokerURL = brkURL;
            myPort = pubPort;
            processname = name;
        }
    }

    class RemotePublisher : MarshalByRefObject, PublisherInterface
    {

        private BrokerInterface broker = Publisher.broker;
        private string myURL = Publisher.myURL;

        public void ChangeTopic(string Topic)
        {
            broker.ChangePublishTopic(myURL, Topic);
        }

        public void SendPublication(string publication)
        {
            broker.ReceivePublication(publication, myURL);
        }
    }
}