using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Web.Http;

namespace Dtimu.Utils
{
    public class IPProfile
    {
        /// <summary>
        /// 点分十进制/冒号16进制 IP 地址
        /// </summary>
        public string Adress { get; set; }
        /// <summary>
        /// 前缀长度
        /// </summary>
        public byte PrefixLength { get; set; }
        /// <summary>
        /// 网关
        /// </summary>
        public string GateWay { get; set; }
    }
    public static class RemoteUtil
    {
        public static byte[] GetAddressBytes(this IPProfile ip)
        {
            if (string.IsNullOrWhiteSpace(ip?.Adress))
                throw new ArgumentException("IP 地址不能为空");

            if (!IPAddress.TryParse(ip.Adress, out var address))
                throw new ArgumentException("IP 地址格式错误");

            return address.GetAddressBytes();
        }

        public static string GetBroadcast(this IPProfile ip)
        {
            if (string.IsNullOrWhiteSpace(ip?.Adress))
                throw new ArgumentException("IP 地址不能为空");

            if (!IPAddress.TryParse(ip.Adress, out var address))
                throw new ArgumentException("IP 地址格式错误");

            // 只支持 IPv4
            if (address.AddressFamily != AddressFamily.InterNetwork)
                throw new NotSupportedException("IPv6 没有广播地址");

            var ipBytes = address.GetAddressBytes();

            uint ipUint =
                ((uint)ipBytes[0] << 24) |
                ((uint)ipBytes[1] << 16) |
                ((uint)ipBytes[2] << 8) |
                ipBytes[3];

            uint mask = PrefixToMask(ip.PrefixLength);

            uint broadcast = ipUint | ~mask;

            var bytes = new byte[]
            {
            (byte)(broadcast >> 24),
            (byte)(broadcast >> 16),
            (byte)(broadcast >> 8),
            (byte)(broadcast)
            };

            return new IPAddress(bytes).ToString();
        }

        private static uint PrefixToMask(int prefixLength)
        {
            if (prefixLength < 0 || prefixLength > 32)
                throw new ArgumentOutOfRangeException(nameof(prefixLength));

            return prefixLength == 0
                ? 0
                : uint.MaxValue << (32 - prefixLength);
        }
        public static void StartFindServers()
        {
            var hostnames = Windows.Networking.Connectivity.NetworkInformation.GetHostNames();
            var res = new List<IPEndPoint>();
            foreach (var item in hostnames)
            {
                if (item.IPInformation != null)
                {
                    if(item.Type == HostNameType.Ipv6)
                    {
                        continue;
                    }
                    if (item.DisplayName.StartsWith("169"))
                    {
                        continue;
                    }
                    var ip = item.DisplayName;
                    var pl = item.IPInformation.PrefixLength;
                    var ipp = new IPProfile();
                    ipp.Adress = ip;
                    ipp.PrefixLength = (byte)pl;
                    var bc = ipp.GetBroadcast();

                    var discovery = new BroadCastDiscover(bc, 0);
                    discovery.DiscoverAsync();

                }
            }
        }
    }
}
