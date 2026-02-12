using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace MVCDtimu
{
    public class BroadcastServer
    {
        public void Start()
        {
            Task.Run(async () =>
            {
                using var udp = new UdpClient(26212);

                Console.WriteLine($"UDP Discovery Server listening on port 26212...");

                while (true)
                {
                    var result = await udp.ReceiveAsync();
                    var msg = Encoding.UTF8.GetString(result.Buffer);

                    if (msg == "DISCOVER_MY_APP")
                    {
                        var response = JsonSerializer.Serialize(new
                        {
                            MachineName = Environment.MachineName,
                            HttpPort = 26131
                        });

                        var bytes = Encoding.UTF8.GetBytes(response);
                        await udp.SendAsync(bytes, bytes.Length, result.RemoteEndPoint);

                    }
                }
            });
        }
    }
}
