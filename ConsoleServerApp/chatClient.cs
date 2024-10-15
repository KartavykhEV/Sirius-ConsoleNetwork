using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleServerApp
{

    public delegate void MessageRecieved(string clnAddress, string message);
    internal delegate void Disconnecting(chatClient client);

    /// <summary>
    /// Клиент 
    /// </summary>
    internal class chatClient
    {
        public TcpClient tcpClient;

        /// <summary>
        /// Событие при поступлении сообщения от клиента
        /// </summary>
        public event MessageRecieved OnMessageRecieved;
        /// <summary>
        /// Событие при отключении клиента от сервера
        /// </summary>
        internal event Disconnecting OnDisconnecting;

        public chatClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} connected");

        }

        public async Task run()
        {
            using (var stream = tcpClient.GetStream())
                while (tcpClient.Connected)
                {
                    byte[] buf = new byte[10];
                    List<byte> bytes = new List<byte>();
                    while (stream.DataAvailable)
                    {
                        int c = await stream.ReadAsync(buf, 0, buf.Length);
                        bytes.AddRange(buf.Take(c));
                    }
                    if (bytes.Count > 0)
                    {
                        var str = Encoding.UTF8.GetString(bytes.ToArray());
                        if (str == "[end]")
                        {
                            Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} disconnecting...");
                            OnDisconnecting?.Invoke(this);
                            break;
                        }
                        else //  прилетело что-то полезное
                        {
                            if (OnMessageRecieved != null)
                                OnMessageRecieved.Invoke(tcpClient.Client.RemoteEndPoint.ToString(), str);
                        }
                    }
                    await Task.Delay(300);
                }
        }

        internal void Send(string message, string fromAddress)
        {
            if (tcpClient.Client.RemoteEndPoint.ToString() != fromAddress)
            {
                var stream = tcpClient.GetStream();
                var bytes = Encoding.UTF8.GetBytes(message);
                stream.Write(bytes, 0, bytes.Length);
            }

        }
    }
}
