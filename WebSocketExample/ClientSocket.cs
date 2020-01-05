using System;
using System.Net.WebSockets;

namespace WebSocketExample
{
    public class ClientSocket
    {
        public ClientSocket(WebSocket socket, string uniqueID)
        {
            this.Socket = socket;
            this.UniqueID = uniqueID;
            this.LastTimeStamp = DateTime.UtcNow;
        }

        public WebSocket Socket { get; set; }

        public string UniqueID { get; set; }

        public IWebAdapterAble Adapter { get; set; }

        public string Name { get; set; }

        public ProtocollObject LastProtoObj { get; set; }

        public DateTime LastTimeStamp { get; set; }

    }
}