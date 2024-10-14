using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace ConsoleClient1
{
    internal class Program
    {

        static bool quit = false;

        static async Task readConsole(NetworkStream stream)
        {
            while (!quit)
            {
                Console.Write("Enter text: ");
                var str = Console.ReadLine();
                if (str == "/q") { str = "[end]"; quit = true; }
                var bytes = Encoding.UTF8.GetBytes(str);
                stream.Write(bytes, 0, bytes.Length); // отправляем запрос
                await Task.Delay(100);
            }
        }

        static async Task Main(string[] args)
        {

            var bClient = new locateServerClient();
            var endpoint = bClient.locateServer();
            if (endpoint == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No servers found, exiting...");
                Console.ResetColor();   
                return;
            }

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(endpoint);
            Console.WriteLine("Connected to server");
            using (var stream = tcpClient.GetStream())
            {
                Task.Run(async () => readConsole(stream)); // создаем поток для чтения сведений с консоли
                while (!quit)
                {
                    byte[] buf = new byte[512];
                    List<byte> bytes = new List<byte>();
                    while (stream.DataAvailable) // проверяем есть ли что-то в потоке
                    { 
                        int c = stream.Read(buf, 0, buf.Length); // читаем поток
                        bytes.AddRange(buf.Take(c));
                    }
                    if (bytes.Count > 0) // если что-то пришло
                    {
                        var str = Encoding.UTF8.GetString(bytes.ToArray()); // преобразуем в строку
                        Console.WriteLine(str);
                    }
                    await Task.Delay(300);
                }
            }
            tcpClient.Close();
        }
    }
}
