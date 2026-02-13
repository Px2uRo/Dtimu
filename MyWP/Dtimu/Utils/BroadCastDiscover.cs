using Dtimu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Dtimu.Utils
{
    public class BroadCastDiscover
    {
        private readonly string _broadcastIp;
        private readonly int _port;

        public BroadCastDiscover(string broadcastIp = "255.255.255.255", int port = 26212)
        {
            _broadcastIp = broadcastIp;
            _port = port;
        }

        public async Task DiscoverAsync()
        {
            using (var udp = new UdpClient())
            {
                udp.EnableBroadcast = true;

                var endpoint = new IPEndPoint(IPAddress.Parse(_broadcastIp), 26212);

                byte[] data = Encoding.UTF8.GetBytes("DISCOVER_MY_APP");
                await udp.SendAsync(data, data.Length, endpoint);

                while (true)
                {
                    var receiveTask = udp.ReceiveAsync();
                    var timeoutTask = Task.Delay(2000);

                    var completed = await Task.WhenAny(receiveTask, timeoutTask);

                    if (completed == timeoutTask)
                    {
                        break;  
                    }

                    var result = receiveTask.Result;
                    var msg = Encoding.UTF8.GetString(result.Buffer);

                    if (JsonObject.TryParse(msg, out var root))
                    {
                        IPEndPoint resI = new IPEndPoint(0,0);
                        var ins = new ServerInstance();
                        foreach (var jO in root)
                        {
                            if (jO.Key == "IP")
                            {
                                resI.Address = IPAddress.Parse(jO.Value.GetString());
                            }
                            if(jO.Key == "HttpPort")
                            {
                                resI.Port = (int)jO.Value.GetNumber();
                            }
                            if (jO.Key == "MachineName")
                            {
                                ins.HostName = jO.Value.GetString();
                            }
                        }

                        ins.IPAddress = $"{resI.Address}:{resI.Port}";

                        GlobalConfigs.ServerInstances.Add(ins);
                    }
                }
            }
        }
    }
}
