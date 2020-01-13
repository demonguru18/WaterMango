"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/quartzHub").build();

connection.on("JobEndMessage", function (jobName, message) {
    Command: toastr["info"](message);
    window.GetCardInfo();
    window.GetPlantList();
    window.GetAlertData();
});

connection.start().then(function () {
    console.log("connection started");
}).catch(function (err) {
    return console.error(err.toString());
});
