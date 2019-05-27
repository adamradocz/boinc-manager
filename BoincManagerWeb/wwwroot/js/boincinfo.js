"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/boincinfoHub").build();

connection.on("ReceiveMessage", function (message, userNumber) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
    document.getElementById("userNumber").textContent = userNumber;
});

connection.start().then(function () {
    var spanSigalRInfo = document.getElementById("signalRInfo");
    spanSigalRInfo.textContent = "Connected";
    spanSigalRInfo.style = "color:green";
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});