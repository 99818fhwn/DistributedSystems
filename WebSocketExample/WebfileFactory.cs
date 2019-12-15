using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketExample
{
    public static class WebfileFactory
    {
        private readonly static object indexLock = new object();

        private readonly static object scriptLock = new object();
        public static string IndexPath { get; set; }

        public static string ScriptPath { get; set; }

        public static void GenerateFiles(IEnumerable<ClientSocket> clients)
        {
            string indexPage = string.Empty;

            indexPage += @" <!DOCTYPE html>
                            <html>
                                <head>
                                    <meta charset=""utf-8""/>
                                    <title>Devices</title>
                                </head>
                                <body>";

            foreach (var c in clients)
            {
                indexPage += $"<div style=\"border: solid; border - color:black; border - width:2px; padding: 2px;\"> \n" +
                    $"<h3>{c.Name}</h3> \n" +
                    $"{c.UniqueID} \n" +
                    $"{c.Adapter.GenerateHTMLCode(c.UniqueID)} \n" +
                    $"</div> \n";
            }

            indexPage += @"<script src=""script.js""></script>
                           </body>
                           </html>";

            lock (indexLock)
            {
                File.WriteAllText(IndexPath, indexPage);
            }

            string passToIdFunc = string.Empty;

            foreach (var c in clients)
            {
                passToIdFunc += $"\n if(identifier == {c.UniqueID}){{ \n" +
                    $"{c.Adapter.GenerateScripcodeGet(c.UniqueID)} \n" +
                    $"\n}}";
            }

            string sendFromIdFunc = string.Empty;

            foreach (var c in clients)
            {
                sendFromIdFunc += $"\n {c.Adapter.GenerateScripCodeSet(c.UniqueID)} \n";
            }

            string scripFile = string.Empty;

            scripFile += @"(function () {
    var webSocketProtocol = location.protocol == ""https: "" ? ""wss: "" : ""ws: "";
    var webSocketURI = webSocketProtocol + ""//"" + location.host + ""/ws"";

            socket = new WebSocket(webSocketURI);

            socket.onopen = function() {
                console.log(""Connected."");
            };

            socket.onclose = function(event) {
            if (event.wasClean) {
            console.log('Disconnected.');
        } else {
            console.log('Connection lost.'); // for example if server processes is killed
        }
        console.log('Code: ' + event.code + '. Reason: ' + event.reason);
        };

    socket.onmessage = function(event) {
            console.log(""Received: \n"" + event.data);
        var msg = event.data;
        var identifier = msg.split(';').filter(part => part.includes('identifier:'))[0].split(':')[1];
        console.log(identifier); " +
        passToIdFunc
    + @"};

    socket.onerror = function(error)
    {
        console.log(""Error: "" + error.message);
    };" +
    sendFromIdFunc +
    @" 
    var form = document.getElementById('form');
    var message = document.getElementById('message');
    form.onsubmit = function () {
        socket.send(message.value);
        message.value = '';
        return false;
    };
    })();";

            lock (scriptLock)
            {
                File.WriteAllText(ScriptPath, scripFile);
            }
        }
    }
}
