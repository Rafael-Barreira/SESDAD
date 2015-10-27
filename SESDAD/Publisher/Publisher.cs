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
    public class Publisher
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
            //myURL = "tcp://localhost:8088/pub";
            //TODO remove after PuppetMaster is implemented
            //myPort = 8088;

            TcpChannel channel = new TcpChannel(myPort);
            ChannelServices.RegisterChannel(channel, false);

            //TODO remove after PuppetMaster is implemented
            //brokerURL = "tcp://localhost:8086/broker";
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemotePublisher),"pub",WellKnownObjectMode.Singleton);

            broker = (BrokerInterface)Activator.GetObject(typeof(BrokerInterface), brokerURL);

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

        public Publisher(string name, string pubURL, string brkURL)
        {
            myURL = pubURL;
            brokerURL = brkURL;
            myPort = parseURL(pubURL);
            processname = name;
        }

        public int parseURL(string url)
        {
            string[] parsedURL = url.Split(':');  //parsedURL[0] = "tcp"; parsedURL[1]= "//localhost"; parsedURL[2]= "PORT/broker";
            string[] parsedURLv2 = parsedURL[2].Split('/'); //parsedURLv2[0] = "PORT"; parsedURLv2[1]= "broker";
            myPort = int.Parse(parsedURLv2[0]);
            return myPort;
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
