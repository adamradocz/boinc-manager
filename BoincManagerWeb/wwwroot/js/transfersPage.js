"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/boincInfoHub").build();

connection.on("ReceiveTransfers", function (transfers) {
    var table = document.getElementById("transfersTable");
    table.getElementsByTagName("tbody")[0].remove();

    var tbody = document.createElement("tbody");

    for (var i = 0; i < transfers.length; i++) {
        var tr = document.createElement("tr");
        
        var tdHostName = document.createElement("td");
        tdHostName.textContent = tasks[i].hostName;

        var tdProject = document.createElement("td");
        tdProject.textContent = tasks[i].project;

        var tdFilename = document.createElement("td");
        tdFilename.textContent = tasks[i].fileName;

        var tdProgress = document.createElement("td");
        var divProgress = document.createElement("div");
        divProgress.className = "progress";

        var divProgressBar = document.createElement("div");
        divProgressBar.className = "progress-bar";
        divProgressBar.style = "width:" + tasks[i].progress + "%";
        divProgressBar.textContent = tasks[i].progress + "%";

        divProgress.appendChild(divProgressBar);
        tdProgress.appendChild(divProgress);

        var tdFileSize = document.createElement("td");
        tdFileSize.textContent = tasks[i].fileSize;

        var tdTransferRate = document.createElement("td");
        tdTransferRate.textContent = tasks[i].transferRate;

        var tdElapsedTime = document.createElement("td");
        tdElapsedTime.textContent = tasks[i].elapsedTime;

        var tdTimeRemaining = document.createElement("td");
        tdTimeRemaining.textContent = tasks[i].timeRemaining;

        var tdStatus = document.createElement("td");
        tdStatus.textContent = tasks[i].status;

        tr.appendChild(tdHostName);
        tr.appendChild(tdProject);
        tr.appendChild(tdFilename);
        tr.appendChild(tdProgress);
        tr.appendChild(tdFileSize);
        tr.appendChild(tdTransferRate);
        tr.appendChild(tdElapsedTime);
        tr.appendChild(tdTimeRemaining);
        tr.appendChild(tdStatus);

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
    connection.invoke("GetTransfers", textSearchString.value);
}