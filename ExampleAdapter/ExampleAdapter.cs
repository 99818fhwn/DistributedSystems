using System;
using WebSocketExample;

namespace ExampleAdapter
{
    public class ExampleAdapter : IWebAdapterAble
    {
        public string AdapterName
        {
            get;
            set;
        }

        public ExampleAdapter()
        {
            this.AdapterName = "ExampleAdapter";
        }

        public string GenerateHTMLCode(string identifier)
        {
            return null;
        }

        public string GenerateScripcodeGet(string identifier)
        {
            return null;
        }

        public string GenerateScripCodeSet(string identifier)
        {
            return null;
        }
    }
}
