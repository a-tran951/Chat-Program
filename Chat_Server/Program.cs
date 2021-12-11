using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace Server
{
    class Program
    {
        private static TcpListener server;
        private static List<TcpClient> tcpClientsList = new List<TcpClient>();

        static void Main(string[] args)
        {
            ServerListener();
        }

        public static void ServerListener()
        {
            server = new TcpListener(IPAddress.Any, 4004);
            server.Start();

            Console.WriteLine("Server started");

            while (true)
            {
                TcpClient tcpClient = server.AcceptTcpClient();//each time connection established it will be stored in a list
                tcpClientsList.Add(tcpClient);

                Thread thread = new Thread(ClientListener);//each client connection need to run in parallel 
                thread.Start(tcpClient);

            }
        }

        public static void ClientListener(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;
            StreamReader reader = new StreamReader(tcpClient.GetStream());
            Regex regex = new Regex(@"^EXITCLIENTCLOSE_[a-zA-Z0-9]{1,}");
            string message = reader.ReadLine();
            Console.WriteLine("Client is connected");
            bool isDone = false;
            foreach (TcpClient client in tcpClientsList)
            {
                StreamWriter sWriter = new StreamWriter(client.GetStream());
                sWriter.WriteLine(message + " has joined the chat");
                sWriter.Flush();
            }
            while (!isDone)
            {
                message = reader.ReadLine();
                if (regex.IsMatch(message))
                {
                    string userName = message.Split("_")[1];
                    tcpClientsList.Remove(tcpClient);
                    foreach (TcpClient client in tcpClientsList)
                    {
                        StreamWriter sWriter = new StreamWriter(client.GetStream());
                        sWriter.WriteLine(userName + " has left the chat");
                        sWriter.Flush();
                    }
                    isDone = true;
                }
                //Broadcast message to all client
                foreach (TcpClient client in tcpClientsList)
                {
                    StreamWriter sWriter = new StreamWriter(client.GetStream());
                    sWriter.WriteLine(message);
                    sWriter.Flush();

                }
                Console.WriteLine(message);
            }
        }


    }
}