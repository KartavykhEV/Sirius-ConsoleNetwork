using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ConsoleClient1
{
    internal class Program
    {
        public enum operation
        {
            plus,
            minus,
            mult,
            div
        }

        class request
        {
            public double v1 { get; set; }
            public double v2 { get; set; }
            public operation op { get; set; }
        }
        class response
        {
            public double result { get; set; }
            public string message { get; set; }
            public bool isError => !String.IsNullOrEmpty(message);
        }


        static void Main(string[] args)
        {
            Console.Write("Enter value 1: ");
            double v1i, v2i;
            if (!Double.TryParse(Console.ReadLine(), out v1i))
            {
                Console.WriteLine("Wrong number format!!!");
                return;
            }
            Console.Write("Enter value 2: ");
            if (!Double.TryParse(Console.ReadLine(), out v2i))
            {
                Console.WriteLine("Wrong number format!!!");
                return;
            }
            Console.Write("Enter operation: ");
            var operationS = Console.ReadLine();
            operation opi = operation.plus;
            switch (operationS)
            {
                case "+": opi = operation.plus; break;
                case "-": opi = operation.minus; break;
                case "/": opi = operation.div; break;
                case "*": opi = operation.mult; break;
                default: Console.WriteLine($"Wrong operation: {operationS}"); return;
            }

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 10001);
            using (var stream = tcpClient.GetStream())
            {
                // формируем запрос на сервере
                request r = new request() { v1 = v1i, v2 = v2i, op = opi };
                var rString = JsonSerializer.Serialize(r);
                var bytes = Encoding.UTF8.GetBytes(rString);

                stream.Write(bytes, 0, bytes.Length); // отправляем запрос
                stream.Flush();
                // читаем ответ сервера
                byte[] buf = new byte[512];
                List<byte> inBytes = new List<byte>();
                do
                {
                    int c = stream.Read(buf, 0, buf.Length);
                    inBytes.AddRange(buf.Take(c));
                }
                while (stream.DataAvailable);
                // разбираем что нам прислал сервер
                var resp = JsonSerializer.Deserialize<response>(Encoding.UTF8.GetString(inBytes.ToArray()));

                // выводим ответ
                Console.WriteLine($"Answer is: {resp.result}");
            }
            tcpClient.Close();
        }
    }
}
