using System;
using WebSocketExample;

namespace SoundActuatorAdapter
{
    public class SoundActuatorAdapter : IWebAdapterAble
    {
        public SoundActuatorAdapter()
        {
            this.AdapterName = "soundactuatoradapter";
        }

        public string AdapterName
        {
            get;
            set;
        }

        public string GenerateHTMLCode(string identifier)
        {
            return @"<h2>Sound Actuator " + identifier + "</h2>" +
                @"Sound is on: <input type=""checkbox"" id=""checkb" + identifier + @"""onclick=""sendOnOff" +
                identifier + @"()"">";
                //@"<input type=""submit"" value=""submit"" onclick=""sendOnOff" + identifier + @"()"">";
        }

        public string GenerateScriptcodeGet(string identifier)
        {
            return $"function changeCBValue{identifier}(msg) {{ \n" +
                $"    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];" +
                $"    document.getElementById('checkb{identifier}').checked = (value == '1'); \n }}" +
                $"    changeCBValue{identifier}(msg);";
        }

        public string GenerateScriptCodeSet(string identifier)
        {
            return $"function sendOnOff{identifier}() {{ \n" +
                   $"var str = 'identifier:' + '{identifier}' + ';;isOn:' + document.getElementById('checkb{identifier}').checked;" +
                   $"socket.send(str); \n }}";
        }
    }
}
