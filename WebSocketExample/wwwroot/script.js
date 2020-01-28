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
 if(identifier == '8980f064xebd2x4995xab86x84165f0f00c0'){ 
function changeCBValue8980f064xebd2x4995xab86x84165f0f00c0(msg) { 
    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];    document.getElementById('checkb8980f064xebd2x4995xab86x84165f0f00c0').checked = (value == '1'); 
 }    changeCBValue8980f064xebd2x4995xab86x84165f0f00c0(msg); 

}
 if(identifier == '3cdfe73axb3cex4ab4xb5e0xbd8c4377714b'){ 
function changeCBValue3cdfe73axb3cex4ab4xb5e0xbd8c4377714b(msg) { 
    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];    document.getElementById('motionDetDiv3cdfe73axb3cex4ab4xb5e0xbd8c4377714b').innerHTML  =     "<p> Motion Detected: " + value + " </p> ";}    changeCBValue3cdfe73axb3cex4ab4xb5e0xbd8c4377714b(msg); 

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
 function sendOnOff8980f064xebd2x4995xab86x84165f0f00c0() { 
var str = 'identifier:' + '8980f064xebd2x4995xab86x84165f0f00c0' + ';;isOn:' + document.getElementById('checkb8980f064xebd2x4995xab86x84165f0f00c0').checked;socket.send(str); 
 } 

  
