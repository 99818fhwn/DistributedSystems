using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketExample
{
    public class Pipeline
    {
        public Pipeline(string fromId, string toId, string additionalParams)
        {
            this.FromId = fromId;
            this.ToId = toId;
            this.AdditionalParams = additionalParams;
        }

        public string FromId { get; set; }

        public string ToId { get; set; }

        public string AdditionalParams { get; set; }
    }
}
