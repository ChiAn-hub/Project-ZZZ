using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Project_ZZZ.Net
{
    public class ClientSocket
    {
        public Socket Socket { get; set; }
        public long lastReceiveTime { get; set; }
        public int UserId { get; set; }
        public ByteArray bytes = new ByteArray();
    }
}
