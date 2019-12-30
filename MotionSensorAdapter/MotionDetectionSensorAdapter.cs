using System;
using WebSocketExample;

namespace MotionSensorAdapter
{
    public class MotionDetectionSensorAdapter : IWebAdapterAble
    {
        public MotionDetectionSensorAdapter()
        {
            this.AdapterName = "motionsensoradapter";
        }

        public string AdapterName 
        { 
            get; 
            set; 
        }

        public string GenerateHTMLCode(string identifier)
        {

            return @"<h2>Motion Detection Sensor  " + identifier + "</h2>" +
                @"1 - motion detected; 0 - no motion: <div id=""motionDetDiv" + identifier + @"""></div>";
        }

        public string GenerateScriptcodeGet(string identifier)
        {
            return $"function changeCBValue{identifier}(msg) {{ \n" +
                $"    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];" +
                $"    document.getElementById('motionDetDiv{identifier}').innerHTML  = "+ 
                @"    ""<p> Motion Detected: "" + value + "" </p> "";}" +
                $"    changeCBValue{identifier}(msg);";
            // @"""<p>Motion Detected: "" + value + ""</p>""
        }

        public string GenerateScriptCodeSet(string identifier)
        {
            return "";
        }
    }
}
