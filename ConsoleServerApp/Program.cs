using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ConsoleServerApp
{
    internal class Program
    {

        /// <summary>
        ///  Клиенты сервера
        /// </summary>
        static List<chatClient> chatClients = new List<chatClient>();

        static void runClient(TcpClient tcpClient)
        {
            chatClient client = new chatClient(tcpClient);
            lock (chatClients)
                chatClients.Add(client);
            client.OnMessageRecieved += Client_OnMessageRecieved;
            client.run();
        }

        private static void Client_OnMessageRecieved(string clnAddress, string message)
        {
            Console.WriteLine($"{clnAddress} said: {message}");
        }

        static async Task Main(string[] args)
        {
            Int16 port = 9082;
            broadcastServer bServer = new broadcastServer(port);
            bServer.run(); // запуск сервера

            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, port);
            Console.WriteLine($"Started TCP server at {port} port.");

            listener.Start();
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                runClient(client); 


            }
        }
    }
}
