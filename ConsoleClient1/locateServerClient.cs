using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient1
{
    internal class locateServerClient
    {
        Int16 port = -1;

        /// <summary>
        /// Найти сервер
        /// </summary>
        /// <returns></returns>
        internal IPEndPoint locateServer()
        {
            var udpClient = new UdpClient() { EnableBroadcast = true };
            foreach (var address in Dns.GetHostAddresses(Dns.GetHostName()))
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    udpClient.Client.Bind(new IPEndPoint(address, 0));
                    var serverEp = new IPEndPoint(IPAddress.Any, 0);
                    int bPort = 9080;
                    Console.WriteLine($"Sending broadcast to {bPort} port over {address} network...");
                    udpClient.Send(new byte[] { 55 }, 1, new IPEndPoint(IPAddress.Broadcast, bPort));
                    try
                    {
                        udpClient.Client.ReceiveTimeout = 2000; // 2 seconds
                        var data = udpClient.Receive(ref serverEp);
                        udpClient.Close();
                        Console.WriteLine($"Server located at {serverEp.Address}:{BitConverter.ToInt16(data.Take(2).ToArray())}");
                        return new IPEndPoint(serverEp.Address, BitConverter.ToInt16(data.Take(2).ToArray()));
                    }
                    catch { }
                }
            return null;
        }













        private void DataReceived(IAsyncResult ar)
        {
            UdpClient c = (UdpClient)ar.AsyncState;
            IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Byte[] receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.ff tt") + " (" + receivedBytes.Length + " bytes)");
            port = 1;
            c.BeginReceive(DataReceived, ar.AsyncState);
        }

    }
}
