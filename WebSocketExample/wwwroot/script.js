(function () {
    var webSocketProtocol = location.protocol == "https:" ? "wss:" : "ws:";
    var webSocketURI = webSocketProtocol + "//" + location.host + "/bs";

    socket = new WebSocket(webSocketURI);

    socket.onopen = function () {
        console.log("Connected.");
    };

    socket.onclose = function (event) {
        if (event.wasClean) {
            console.log('Disconnected.');
        } else {
            console.log('Connection lost.'); // for example if server processes is killed
        }
        console.log('Code: ' + event.code + '. Reason: ' + event.reason);
    };

    socket.onmessage = function (event) {
        console.log("Received: \n" + event.data);
        var msg = event.data;
        var identifier = msg.split(';').filter(part => part.includes('identifier:'))[0].split(':')[1];
        console.log(identifier);
    };

    socket.onerror = function (error) {
        console.log("Error: " + error.message);
    }; var pipesocketURI = webSocketProtocol + "//" + location.host + "/ps";
    psocket = new WebSocket(pipesocketURI);

    psocket.onopen = function () {
        console.log("Connected pipeline.");
    };

    psocket.onclose = function (event) {
        if (event.wasClean) {
            console.log('Disconnected pipeline.');
        } else {
            console.log('Connection lost pipeline.'); // for example if server processes is killed
        }
        console.log('Code: ' + event.code + '. Reason: ' + event.reason);
    };

    psocket.onmessage = function (event) {
        console.log("Received pipeline: \n" + event.data);
        var msg = event.data;
        var error = msg.split(':')[1];
        console.log(error);
    };

    psocket.onerror = function (error) {
        console.log("Error: " + error.message);
    };
})();
function sendPipeline() {
    var frommessage = document.getElementById('frommessage');
    var tomessage = document.getElementById('tomessage');
    var additionalParams = document.getElementById('additionalParamsmessage');
    var str = "add:" + frommessage.value + "-->" + tomessage.value + ";;" + additionalParams.value;
    frommessage.value = '';
    tomessage.value = '';
    additionalParams.value = '';
    psocket.send(str);
    setTimeout(function () {
        location.reload();
    }, 500)
}

function sendDelete(ids) {
    var str = "delete:" + ids;
    psocket.send(str);
    setTimeout(function () {
        location.reload();
    }, 500);
}