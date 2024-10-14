using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleServerApp
{
    internal class broadcastServer
    {
        private short _port;

        internal broadcastServer(short port) => _port = port;

        internal void run() => Task.Run(() => taskRun());


        void taskRun()
        {
            int bPort = 9080;
            var udpServer = new UdpClient(bPort) { EnableBroadcast = true };
            Console.WriteLine($"Started broadcast UDP server at {bPort} port.");
            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                var data = udpServer.Receive(ref remoteEP); // listen on port 10000
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Broadcast: receive data from " + remoteEP.ToString());
                Console.ResetColor();
                udpServer.Send(BitConverter.GetBytes(_port), 2, remoteEP);
                //udpServer.Send(BitConverter.GetBytes(_port), 2); // reply back
            }



            //var client = listener.AcceptTcpClient();
            //Console.ForegroundColor = ConsoleColor.Green;
            //("Client connected to boarcast server...");
            //Console.ResetColor();
            //using (var stream = client.GetStream())
            //    stream.Write(BitConverter.GetBytes(_port), 0, 2); // отправялем клиенту порт сервера

        }
    }
}
