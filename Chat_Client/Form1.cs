using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace Chat_Client
{
    public partial class Form1 : Form
    {
        TcpClient tcpClient;
        string clientUserName;
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StreamWriter sWriter = new StreamWriter(tcpClient.GetStream());
            sWriter.WriteLine("EXITCLIENTCLOSE_" + clientUserName);
        }
        static void SendDataToServer(object obj)
        {
            try
            {
                TcpClient tcpClient = (TcpClient)obj;
                StreamWriter sWriter = new StreamWriter(tcpClient.GetStream());

                while (true)
                {
                    if (tcpClient.Connected)
                    {
                        string input = Console.ReadLine();
                        sWriter.WriteLine(input);
                        sWriter.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }
        static void ReadDataFromServer(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;
            StreamReader sReader = new StreamReader(tcpClient.GetStream());

            while (true)
            {
                try
                {
                    string message = sReader.ReadLine();
                    Console.WriteLine(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs eventArgument)
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 4004);
                StreamWriter sWriter = new StreamWriter(tcpClient.GetStream());
                Console.WriteLine("Connected to server.");
                Console.WriteLine("Please enter a username");
                clientUserName = Regex.Replace(Console.ReadLine(), @"\s", "");
                sWriter.WriteLine(clientUserName);
                Thread thread = new Thread(ReadDataFromServer);
                thread.Start(tcpClient);

                Thread thread2 = new Thread(SendDataToServer);
                thread2.Start(tcpClient);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            Console.ReadKey();
        }
    }
}
