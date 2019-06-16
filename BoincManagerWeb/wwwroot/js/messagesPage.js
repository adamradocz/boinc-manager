"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/boincInfoHub").build();

connection.on("ReceiveMessages", function (messages) {
    var table = document.getElementById("messagesTable");
    table.getElementsByTagName("tbody")[0].remove();

    var tbody = document.createElement("tbody");

    for (var i = 0; i < messages.length; i++) {
        var tr = document.createElement("tr");
        
        var tdHostName = document.createElement("td");
        tdHostName.textContent = messages[i].hostName;

        var tdProject = document.createElement("td");
        tdProject.textContent = messages[i].project;

        var tdDate = document.createElement("td");
        tdDate.textContent = messages[i].date;

        var tdMessageBody = document.createElement("td");
        tdMessageBody.textContent = messages[i].messageBody;

        var tdPriority = document.createElement("td");
        tdPriority.textContent = messages[i].priority;

        tr.appendChild(tdHostName);
        tr.appendChild(tdProject);
        tr.appendChild(tdDate);
        tr.appendChild(tdMessageBody);
        tr.appendChild(tdPriority);

        tbody.appendChild(tr);
    }

    table.appendChild(tbody);
});

connection.start().then(function () {
    setInterval(updateLoop, 1000);
}).catch(function (err) {
    return console.error(err.toString());
});

function updateLoop() {
    var textSearchString = document.getElementById("searchString");
    connection.invoke("GetMessages", textSearchString.value);
}