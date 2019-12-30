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

        public static void GenerateFiles(IEnumerable<ClientSocket> clients, IEnumerable<Pipeline> pipelines)
        {
            string indexPage = string.Empty;

            indexPage += @" <!DOCTYPE html>
                            <html>
                                <head>
                                    <meta charset=""utf-8""/>
                                    <title>Devices</title>
                                </head>
                                <body>
    <h1>Clients</h1>";

            foreach (var c in clients)
            {
                if (c.Adapter != null)
                {
                    indexPage += $"<div style=\"border:solid; border-color:black; border-width:2px; padding:2px;\"> \n" +
    $"<h3>{c.Name}</h3> \n" +
    $"{c.UniqueID} \n" +
    $"{c.Adapter.GenerateHTMLCode(c.UniqueID)} \n" +
    $"</div> \n";
                }
            }

            indexPage += @"<div style=""border:solid; border-color:black; border-width:2px; padding:2px;"">
               <h2> Add Pipeline </h2>
                  <form id=""fromform"">
                       <input id=""frommessage"" autocomplete=""off""/>
                      </form>
                      <form id=""toform"">
                           <input id =""tomessage"" autocomplete=""off""/>
                          </form>
                    <form id=""additionalParams"">
             <input id=""additionalParamsmessage"" autocomplete=""off""/>
            </form>";

            indexPage += $"\n <button onclick=\"sendPipeline()\">Add Pipeline</button> \n";

            indexPage += @"<h2> Current Pipelines </h2>
                             <div id=""pipelineContainer"">
            <table>
";

            foreach (var pi in pipelines)
            {
                indexPage += $"<tr><th>{pi.FromId}-->{pi.ToId};;{pi.AdditionalParams}</th><th><button onclick=\"sendDelete('{pi.FromId}-->{pi.ToId}')\">Delete</button></th></tr>\n";
            }

            indexPage += @"</table>
                           </div>
                         </div>";

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
                passToIdFunc += $"\n if(identifier == '{c.UniqueID}'){{ \n" +
                    $"{c.Adapter.GenerateScriptcodeGet(c.UniqueID)} \n" +
                    $"\n}}";
            }

            string sendFromIdFunc = string.Empty;

            foreach (var c in clients)
            {
                sendFromIdFunc += $"\n {c.Adapter.GenerateScriptCodeSet(c.UniqueID)} \n";
            }

            string scripFile = string.Empty;

            scripFile += @"(function () {
    var webSocketProtocol = location.protocol == ""https:"" ? ""wss:"" : ""ws:"";
    var webSocketURI = webSocketProtocol + ""//"" + location.host + ""/bs"";

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
    //@" ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //var form = document.getElementById('form');
    //var message = document.getElementById('message');
    //form.onsubmit = function () {
    //    socket.send(message.value);
    //    message.value = '';
    //    return false;
    //};

    @"var pipesocketURI = webSocketProtocol + ""//"" + location.host + ""/ps"";
    psocket = new WebSocket(pipesocketURI);

            psocket.onopen = function() {
                console.log(""Connected pipeline."");
            };

            psocket.onclose = function(event) {
            if (event.wasClean) {
            console.log('Disconnected pipeline.');
        } else {
            console.log('Connection lost pipeline.'); // for example if server processes is killed
        }
        console.log('Code: ' + event.code + '. Reason: ' + event.reason);
        };

    psocket.onmessage = function(event) {
            console.log(""Received pipeline: \n"" + event.data);
        var msg = event.data;
        var error = msg.split(':')[1];
        console.log(error);
    };

    psocket.onerror = function(error)
    {
        console.log(""Error: "" + error.message);
    };
    })();
    function sendPipeline()
    {
        var frommessage = document.getElementById('frommessage');
        var tomessage = document.getElementById('tomessage');
        var additionalParams = document.getElementById('additionalParamsmessage');
        var str = ""add:"" + frommessage.value + ""-->"" + tomessage.value + "";;"" + additionalParams.value;
        frommessage.value = '';
        tomessage.value = '';
        additionalParams.value = '';
        psocket.send(str);
        setTimeout(function() {
        location.reload();
        }, 500)
    }

    function sendDelete(ids)
    {
        var str = ""delete:"" + ids;
        psocket.send(str);
        setTimeout(function() {
        location.reload();
        }, 500);
    }";
            scripFile += sendFromIdFunc;

            lock (scriptLock)
            {
                File.WriteAllText(ScriptPath, scripFile);
            }
        }
    }
}
