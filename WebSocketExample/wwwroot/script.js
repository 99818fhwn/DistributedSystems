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
 };

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
  

 function sendOnOff8b8cb4acx4693x47f6x81a2xe92062877e81() { 
var str = 'identifier:' + '8b8cb4acx4693x47f6x81a2xe92062877e81' + ';;isOn:' + document.getElementById('checkb8b8cb4acx4693x47f6x81a2xe92062877e81').checked;socket.send(str); 
 } 

  

 function sendOnOff0fb4efadxe3c9x43cdx8d65x85d7106013df() { 
var str = 'identifier:' + '0fb4efadxe3c9x43cdx8d65x85d7106013df' + ';;isOn:' + document.getElementById('checkb0fb4efadxe3c9x43cdx8d65x85d7106013df').checked;socket.send(str); 
 } 

  

 function sendOnOff6d47cf18x6b49x4f3fxbcadx8956ca677fb5() { 
var str = 'identifier:' + '6d47cf18x6b49x4f3fxbcadx8956ca677fb5' + ';;isOn:' + document.getElementById('checkb6d47cf18x6b49x4f3fxbcadx8956ca677fb5').checked;socket.send(str); 
 } 

  

  

 function sendOnOff40dc92f3x76eax4ddbxaed2xb8844307821d() { 
var str = 'identifier:' + '40dc92f3x76eax4ddbxaed2xb8844307821d' + ';;isOn:' + document.getElementById('checkb40dc92f3x76eax4ddbxaed2xb8844307821d').checked;socket.send(str); 
 } 

  

 function sendOnOff270bf397xadebx48dcxaef6xaf00bb3ce2ae() { 
var str = 'identifier:' + '270bf397xadebx48dcxaef6xaf00bb3ce2ae' + ';;isOn:' + document.getElementById('checkb270bf397xadebx48dcxaef6xaf00bb3ce2ae').checked;socket.send(str); 
 } 

  

 function sendOnOffc2b6ad6fxf48cx45d5xbf79x07fc505467bd() { 
var str = 'identifier:' + 'c2b6ad6fxf48cx45d5xbf79x07fc505467bd' + ';;isOn:' + document.getElementById('checkbc2b6ad6fxf48cx45d5xbf79x07fc505467bd').checked;socket.send(str); 
 } 

  

 function sendOnOff176df71cx78d5x49f4xa799xc0216373c820() { 
var str = 'identifier:' + '176df71cx78d5x49f4xa799xc0216373c820' + ';;isOn:' + document.getElementById('checkb176df71cx78d5x49f4xa799xc0216373c820').checked;socket.send(str); 
 } 

  

 function sendOnOffb1a460c5xf31bx4454xa25dxaaa5e1d48b80() { 
var str = 'identifier:' + 'b1a460c5xf31bx4454xa25dxaaa5e1d48b80' + ';;isOn:' + document.getElementById('checkbb1a460c5xf31bx4454xa25dxaaa5e1d48b80').checked;socket.send(str); 
 } 

 function sendOnOff6c603b43x9407x49eexbe7cx6e7ff1850507() { 
var str = 'identifier:' + '6c603b43x9407x49eexbe7cx6e7ff1850507' + ';;isOn:' + document.getElementById('checkb6c603b43x9407x49eexbe7cx6e7ff1850507').checked;socket.send(str); 
 } 

 function sendOnOfff643db52xd2c0x4473xb780xc88faad04661() { 
var str = 'identifier:' + 'f643db52xd2c0x4473xb780xc88faad04661' + ';;isOn:' + document.getElementById('checkbf643db52xd2c0x4473xb780xc88faad04661').checked;socket.send(str); 
 } 

  
