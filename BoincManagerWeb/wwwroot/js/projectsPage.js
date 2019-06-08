"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/boincInfoHub").build();

connection.on("ReceiveProjects", function (projects) {
    var table = document.getElementById("projectsTable");
    table.getElementsByTagName("tbody")[0].remove();

    var tbody = document.createElement("tbody");

    for (var i = 0; i < projects.length; i++) {
        var tr = document.createElement("tr");

        var tdHostName = document.createElement("td");
        tdHostName.textContent = projects[i].hostName;

        var tdName = document.createElement("td");
        tdName.textContent = projects[i].name;

        var tdUsername = document.createElement("td");
        tdUsername.textContent = projects[i].username;

        var tdTeam = document.createElement("td");
        tdTeam.textContent = projects[i].team;

        var tdCredit = document.createElement("td");
        tdCredit.textContent = projects[i].credit;

        var tdAverageCredit = document.createElement("td");
        tdAverageCredit.textContent = projects[i].averageCredit;

        var tdStatus = document.createElement("td");
        tdStatus.textContent = projects[i].status;

        tr.appendChild(tdHostName);
        tr.appendChild(tdName);
        tr.appendChild(tdUsername);
        tr.appendChild(tdTeam);
        tr.appendChild(tdCredit);
        tr.appendChild(tdAverageCredit);        
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
    connection.invoke("GetProjects", textSearchString.value);
}