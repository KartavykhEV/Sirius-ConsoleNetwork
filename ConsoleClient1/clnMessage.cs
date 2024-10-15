using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleClient1
{
    /// <summary>
    /// Сообщение клиента серверу
    /// </summary>
    public class clnMessage
    {
        public string message {  get; set; }

        public byte[] ToBytes()
        {
            var json = JsonSerializer.Serialize(this);
            return Encoding.UTF8.GetBytes(json);
        }

        static public clnMessage FromBytes(byte[] source)
        {
            var json = Encoding.UTF8.GetString(source, 0, source.Length);
            return JsonSerializer.Deserialize<clnMessage>(json);   
        }

    }
}
