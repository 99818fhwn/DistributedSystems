using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketExample
{
    public class Pipeline
    {
        public Pipeline(string fromId, string toId)
        {
            this.FromId = fromId;
            this.ToId = toId;
        }

        public string FromId { get; set; }

        public string ToId { get; set; }
    }
}
