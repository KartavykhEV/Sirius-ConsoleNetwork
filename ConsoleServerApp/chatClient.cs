using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleServerApp
{

    public delegate void MessageRecieved(string clnAddress, string message);

    /// <summary>
    /// Клиент 
    /// </summary>
    internal class chatClient
    {
        public TcpClient tcpClient;


        public event MessageRecieved OnMessageRecieved;

        public chatClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} connected");

        }

        public void run()
        {
            using (var stream = tcpClient.GetStream())
                while (true)
                {
                    byte[] buf = new byte[10];
                    List<byte> bytes = new List<byte>();
                    do
                    {
                        int c = stream.Read(buf, 0, buf.Length);
                        bytes.AddRange(buf.Take(c));
                    }
                    while (stream.DataAvailable);
                    var str = Encoding.UTF8.GetString(bytes.ToArray());
                    if (str == "[end]")
                    {
                        Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} disconnecting...");
                        break;
                    }
                    else //  прилетело что-то полезное
                    {
                        if (OnMessageRecieved != null)
                            OnMessageRecieved.Invoke(tcpClient.Client.RemoteEndPoint.ToString(), str);
                    }
                }
        }

    }
}
