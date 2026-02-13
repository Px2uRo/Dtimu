using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtimu.Models
{
    public class ServerInstance
    {
        public string IPAddress { get; set; }
        public Version Version { get; set; }
        public string HostName { get; set; }
    }
}
