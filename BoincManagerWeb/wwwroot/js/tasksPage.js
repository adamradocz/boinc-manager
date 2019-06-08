"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/boincInfoHub").build();

connection.on("ReceiveTasks", function (tasks) {
    var table = document.getElementById("tasksTable");
    table.getElementsByTagName("tbody")[0].remove();

    var tbody = document.createElement("tbody");

    for (var i = 0; i < tasks.length; i++) {
        var tr = document.createElement("tr");

        var tdHostName = document.createElement("td");
        tdHostName.textContent = tasks[i].hostName;

        var tdProject = document.createElement("td");
        tdProject.textContent = tasks[i].project;

        var tdProgress = document.createElement("td");        
        var divProgress = document.createElement("div");
        divProgress.className = "progress";

        var divProgressBar = document.createElement("div");
        divProgressBar.className = "progress-bar";
        divProgressBar.style = "width:" + tasks[i].progress + "%";
        divProgressBar.textContent = tasks[i].progress + "%";

        divProgress.appendChild(divProgressBar);
        tdProgress.appendChild(divProgress);
        
        var tdStatus = document.createElement("td");
        tdStatus.textContent = tasks[i].status;

        var tdElapsedTime = document.createElement("td");
        tdElapsedTime.textContent = tasks[i].elapsedTime;

        var tdLastCheckpoint = document.createElement("td");
        tdLastCheckpoint.textContent = tasks[i].lastCheckpoint;

        var tdDeadline = document.createElement("td");
        tdDeadline.textContent = tasks[i].deadline;

        var tdApplication = document.createElement("td");
        tdApplication.textContent = tasks[i].application;

        var tdWorkunit = document.createElement("td");
        tdWorkunit.textContent = tasks[i].workunit;        
        
        tr.appendChild(tdHostName);
        tr.appendChild(tdProject);
        tr.appendChild(tdProgress);
        tr.appendChild(tdStatus);
        tr.appendChild(tdElapsedTime);
        tr.appendChild(tdLastCheckpoint);
        tr.appendChild(tdDeadline);
        tr.appendChild(tdApplication);
        tr.appendChild(tdWorkunit);

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
    connection.invoke("GetTasks", textSearchString.value);
}