"use strict";

window.RequestId = -1;

document.getElementById("sendButton").disabled = true;
document.getElementById("getButton").disabled = true;
document.getElementById("request_name").textContent = "---";
document.getElementById("request_text").textContent = "---";
document.getElementById("request_id").textContent = "---";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/SupportHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();
connection.serverTimeoutInMilliseconds = 1200000;

//
connection.on("ReceiveRequest", function (request) {
    RequestId = request.id;
    document.getElementById("sendButton").disabled = false;
    document.getElementById("getButton").disabled = true;
    document.getElementById("request_name").textContent = request.head;
    document.getElementById("request_text").textContent = request.text;
    document.getElementById("request_id").textContent = request.id;
});
//

async function start() {
    try {
        document.getElementById("sendButton").disabled = true;
        document.getElementById("getButton").disabled = true;
        document.getElementById("disconnectcard").hidden = false;
        await connection.start().then(function () {
            console.log("SignalR Connected.");
            document.getElementById("disconnectcard").hidden = true;
            document.getElementById("sendButton").disabled = true;
            document.getElementById("getButton").disabled = false;
            connection.invoke("GetCurrentRequest").catch(function (err) {
                return console.error(err.toString());
            });

        }).catch(function (err) {
            return console.error(err.toString());
            setTimeout(start, 500);
        });
    } catch (err) {
        console.log(err);
        setTimeout(start, 500);
    }
    
}

connection.onclose(start);

document.getElementById("getButton").addEventListener("click", function (event) {
    document.getElementById("getButton").disabled = true;
    connection.invoke("GetRequest").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    document.getElementById("sendButton").disabled = true;
    document.getElementById("getButton").disabled = false;
    document.getElementById("request_name").textContent = "---";
    document.getElementById("request_text").textContent = "---";
    document.getElementById("request_id").textContent = "---";
    connection.invoke("SendDoneRequest", RequestId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

// Start the connection.
start();