using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace COREWEBSOCKET
{
    public class webLinsPeople
    {
        public string Name { get; set; }

        public string Id { get; set; }
        public string Room { get; set; }
        public WebSocket webSocket { get; set; }
    }
}
