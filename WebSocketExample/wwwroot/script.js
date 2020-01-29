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
 if(identifier == '2c18c358x6db7x477fx9885x02a48ab212a4'){ 
function changeCBValue2c18c358x6db7x477fx9885x02a48ab212a4(msg) { 
    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];    document.getElementById('checkb2c18c358x6db7x477fx9885x02a48ab212a4').checked = (value == '1'); 
 }    changeCBValue2c18c358x6db7x477fx9885x02a48ab212a4(msg); 

}
 if(identifier == '999d3a2cxd0e4x4dc8xba9cx4e943929ad2c'){ 
function changeCBValue999d3a2cxd0e4x4dc8xba9cx4e943929ad2c(msg) { 
    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];    document.getElementById('motionDetDiv999d3a2cxd0e4x4dc8xba9cx4e943929ad2c').innerHTML  =     "<p> Motion Detected: " + value + " </p> ";}    changeCBValue999d3a2cxd0e4x4dc8xba9cx4e943929ad2c(msg); 

}
 if(identifier == 'b983645ax8b9dx4eefx9a65x35b5e46d9293'){ 
function changeCBValueb983645ax8b9dx4eefx9a65x35b5e46d9293(msg) { 
    let value = msg.split(';').filter(part => part.includes('isOn:'))[0].split(':')[1];    document.getElementById('checkbb983645ax8b9dx4eefx9a65x35b5e46d9293').checked = (value == '1'); 
 }    changeCBValueb983645ax8b9dx4eefx9a65x35b5e46d9293(msg); 

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
 function sendOnOff2c18c358x6db7x477fx9885x02a48ab212a4() { 
var str = 'identifier:' + '2c18c358x6db7x477fx9885x02a48ab212a4' + ';;isOn:' + document.getElementById('checkb2c18c358x6db7x477fx9885x02a48ab212a4').checked;socket.send(str); 
 } 

  

 function sendOnOffb983645ax8b9dx4eefx9a65x35b5e46d9293() { 
var str = 'identifier:' + 'b983645ax8b9dx4eefx9a65x35b5e46d9293' + ';;isOn:' + document.getElementById('checkbb983645ax8b9dx4eefx9a65x35b5e46d9293').checked;socket.send(str); 
 } 
