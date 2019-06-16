"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/boincInfoHub").build();

connection.on("ReceiveVisitors", function (userNumber) {
    document.getElementById("userNumber").textContent = userNumber;
});

connection.start().then(function () {
    var spanSigalRInfo = document.getElementById("signalRInfo");
    spanSigalRInfo.textContent = "Connected";
    spanSigalRInfo.style = "color:green";

    connection.invoke("GetVisitors");

    setInterval(updateLoop, 1000);
}).catch(function (err) {
    return console.error(err.toString());
});

function updateLoop() {
    connection.invoke("GetVisitors");
}