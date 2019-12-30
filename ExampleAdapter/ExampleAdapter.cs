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
            this.AdapterName = "exampleadapter";
        }

        public string GenerateHTMLCode(string identifier)
        {

            return @"<h2>TurnOn</h2>
        LightOn: <input type=""checkbox"" id=""checkb" + identifier + @"""onclick=""sendOnOff" + identifier + @"()"">";
        }

        public string GenerateScriptcodeGet(string identifier)
        {
            return $"function changeCBValue{identifier}(msg) {{ \n" +
                $"    let value = msg.split(';').filter(part => part.includes('brightness:'))[0].split(':')[1];" +
                $"    document.getElementById('checkb{identifier}').checked = (value == 'true'); \n }}";
        }

        public string GenerateScriptCodeSet(string identifier)
        {
            return $"function sendOnOff{identifier}() {{ \n" +
                   $"var str = 'identifier:' + {identifier} + ';;isactive:' + document.getElementById('checkb{identifier}').checked;" +
                   $"socket.send(str); \n }}";
        }
    }
}
