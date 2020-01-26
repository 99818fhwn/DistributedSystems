(function () {
    var webSocketProtocol = location.protocol == "https:" ? "wss:" : "ws:";
    var webSocketURI = webSocketProtocol + "//" + location.host + "/bs";

            socket = new WebSocket(webSocketURI);

            socket.onopen = function() {
                console.log("Connected.");
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
            console.log("Received: \n" + event.data);
        var msg = event.data;
        var identifier = msg.split(';').filter(part => part.includes('identifier:'))[0].split(':')[1];
        console.log(identifier); 
 if(identifier == '3734306dx0592x4ef0x88bax75b8f1ef959d'){ 
function changeCBValue3734306dx0592x4ef0x88bax75b8f1ef959d(msg) { 
    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];    document.getElementById('motionDetDiv3734306dx0592x4ef0x88bax75b8f1ef959d').innerHTML  =     "<p> Motion Detected: " + value + " </p> ";}    changeCBValue3734306dx0592x4ef0x88bax75b8f1ef959d(msg); 

}
 if(identifier == '40f1071cxf9ffx4524xbb3dxa3a38ef092cc'){ 
function changeCBValue40f1071cxf9ffx4524xbb3dxa3a38ef092cc(msg) { 
    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];    document.getElementById('checkb40f1071cxf9ffx4524xbb3dxa3a38ef092cc').checked = (value == '1'); 
 }    changeCBValue40f1071cxf9ffx4524xbb3dxa3a38ef092cc(msg); 

}
 if(identifier == 'b72a6e92x2104x4c5bx88c3x8dbc966eb000'){ 
function changeCBValueb72a6e92x2104x4c5bx88c3x8dbc966eb000(msg) { 
    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];    document.getElementById('motionDetDivb72a6e92x2104x4c5bx88c3x8dbc966eb000').innerHTML  =     "<p> Motion Detected: " + value + " </p> ";}    changeCBValueb72a6e92x2104x4c5bx88c3x8dbc966eb000(msg); 

}};

    socket.onerror = function(error)
    {
        console.log("Error: " + error.message);
    };var pipesocketURI = webSocketProtocol + "//" + location.host + "/ps";
    psocket = new WebSocket(pipesocketURI);

            psocket.onopen = function() {
                console.log("Connected pipeline.");
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
            console.log("Received pipeline: \n" + event.data);
        var msg = event.data;
        var error = msg.split(':')[1];
        console.log(error);
    };

    psocket.onerror = function(error)
    {
        console.log("Error: " + error.message);
    };
    })();
    function sendPipeline()
    {
        var frommessage = document.getElementById('frommessage');
        var tomessage = document.getElementById('tomessage');
        var additionalParams = document.getElementById('additionalParamsmessage');
        var str = "add:" + frommessage.value + "-->" + tomessage.value + ";;" + additionalParams.value;
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
        var str = "delete:" + ids;
        psocket.send(str);
        setTimeout(function() {
        location.reload();
        }, 500);
    }
  

 function sendOnOff40f1071cxf9ffx4524xbb3dxa3a38ef092cc() { 
var str = 'identifier:' + '40f1071cxf9ffx4524xbb3dxa3a38ef092cc' + ';;isOn:' + document.getElementById('checkb40f1071cxf9ffx4524xbb3dxa3a38ef092cc').checked;socket.send(str); 
 } 

  
