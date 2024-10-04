using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ConsoleServerApp
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
        }

        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 10001);
            listener.Start();
            while (true)
            {
                var client = listener.AcceptTcpClient();

                using (var stream = client.GetStream())
                {
                    byte[] buf = new byte[10];
                    List<byte> bytes = new List<byte>();
                    do
                    {
                        int c = stream.Read(buf, 0, buf.Length);
                        bytes.AddRange(buf.Take(c));
                    }
                    while (stream.DataAvailable);
                    var rString = Encoding.UTF8.GetString(bytes.ToArray());
                    var r = JsonSerializer.Deserialize<request>(rString);
                    response response = new response();
                    if (r.op == operation.div && r.v2 == 0)
                    {
                        response.message = "Делить на 0 нельзя!";
                    }
                    else
                    {
                        string opS = "";
                        double result = 0;
                        switch (r.op)
                        {
                            case operation.plus: result = (r.v1 + r.v2); opS = "+"; break;
                            case operation.minus: result = (r.v1 - r.v2); opS = "-"; break;
                            case operation.mult: result = (r.v1 * r.v2); opS = "*"; break;
                            case operation.div: result = (r.v1 / r.v2); opS = "/"; break;
                        }
                        Console.WriteLine($"{r.v1}{opS}{r.v2} = {result}");
                        response.result = result;
                    }
                    var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                    stream.Write(data, 0, data.Length);
                    //Console.WriteLine("Recieved message: " + Encoding.UTF8.GetString(bytes.ToArray()));
                }

            }
        }
    }
}
