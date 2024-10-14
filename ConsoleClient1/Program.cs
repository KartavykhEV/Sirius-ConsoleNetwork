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

        static void readConsole(NetworkStream stream)
        {
            while (!quit)
            {
                var str = Console.ReadLine();
                if (str == "/q") { str = "[end]"; quit = true; }
                var bytes = Encoding.UTF8.GetBytes(str);
                stream.Write(bytes, 0, bytes.Length); // отправляем запрос
            }
        }

        static void Main(string[] args)
        {

            var bClient = new locateServerClient();
            var endpoint = bClient.locateServer();


            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(endpoint);
            using (var stream = tcpClient.GetStream())
            {
                Task.Run(() => readConsole(stream)); // создаем поток для чтения сведений с консоли
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
                    Task.Delay(300);
                }
            }
            tcpClient.Close();
        }
    }
}
