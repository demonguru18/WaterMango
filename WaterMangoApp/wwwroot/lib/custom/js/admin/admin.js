var params = {__RequestVerificationToken: ""};

var plantTable = new Tabulator("#LoadTasksTable", {
    height: "auto",
    layout: "fitColumns",
    paginationSize: 20,
    placeholder: "Loading Scheduled Tasks...",
    columns: [
        { title: "PlantId", field: "id", sorter: "number", align: "center" },
        { title: "Name", field: "name", sorter: "string", align: "left" },
        { title: "Picture", field: "imageUrl", align: "center", formatter:"image", formatterParams:{
                height:"50px",
                width:"50px"
            } },
        {
            title: "Last Run", field: "lastWateredAt", sorter: "date", align: "center", formatter: function (cell) {
                var convertTime = new Date(cell.getValue()).toUTCString();
                return convertTime;
            }
        },
        {
            title: "Next Run", field: "nextWateredAt", sorter: "date", align: "center", formatter: function (cell) {
                var convertTime = new Date(cell.getValue()).toUTCString();
                return convertTime;
            }
        },
        { title: "Active", field: "status", sorter:"boolean", align: "center", formatter:"tickCross" },
        { title: "Task Name", field: "jobId", sorter: "string", align: "center" },
        {title:"Actions", align:"center", formatter:function (cell) {
                var plantId = cell.getData().id;
                var plantJobId = cell.getData().jobId;
                
                var newEditRow = "<div class='btn-group' role='group' aria-label='Perform Actions'>" +                    
                    "<button type='button' name='stop'  class='btn btn-warning btn-sm' onclick='stopWateringPlantById(this)' " +
                    " data-plantjobid='"+plantJobId+"' " +
                    ">" +
                    "<span>" +
                    "<i class='fas fa-tint-slash'>" +
                    "</i>" +
                    "</span>" +
                    "</button>" +
                    "<button type='button' name='start'  class='btn btn-success btn-sm' onclick='startWateringPlantById(this)' " +
                    " data-plantid='"+plantId+"' " +
                    ">" +
                    "<span>" +
                    "<i class='fas fa-tint'>" +
                    "</i>" +
                    "</span>" +
                    "</button>" +                    
                    "</div>";

                return newEditRow;
            }}       
    ]
});
var alertTable = new Tabulator("#LoadTasksStatusTable", {

    height: "auto",
    layout: "fitColumns",
    pagination:"local",
    paginationSize: 5,
    paginationSizeSelector:[3, 6, 8, 10],
    placeholder: "Loading Alerts...",
    columns: [
        { title: "AlertID", field: "id", sorter: "number", align: "center" },
        { title: "Message", field: "message", sorter: "string", align: "left" },       
        { title: "Task", field: "taskName", sorter:"string", align: "center"},
        { title: "Status", field: "status", sorter:"boolean", align: "center", formatter:"tickCross" }        
    ]
});

$(function () {
    //Toastr notification options 
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    
    GetCardInfo();
    GetPlantList();
    GetAlertData();
});

// Global Functions
window.GetCardInfo = function() 
{       
    $.ajax({
        type: 'GET',
        url: '/api/v1/plant/GetCardInfo/',
        dataType: 'json',
        success: function (data) {            
            $("#plantCount").text(data.plantCount);
            $("#alertCount").text(data.alertCount);
            $("#successCount").text(data.successCount);
            $("#userCount").text(data.userCount);
        },
        error: function () {
            Command: toastr["error"]("Could not pre-load data!");
        }
    });
};

window.GetPlantList = function() {
    $.ajax({
        type: 'GET',
        url: '/api/v1/plant/GetListOfPlants/',
        dataType: 'json',
        success: function (data) {
            var obj = JSON.stringify(data);
            plantTable.setData(obj);
        },
        error: function () {
            Command: toastr["error"]("Could not pre-load plant data!");
        }
    });
};

window.GetAlertData = function () {
    $.ajax({
        type: 'GET',
        url: '/api/v1/plant/GetAlertData/',
        dataType: 'json',
        success: function (data) {
            var obj = JSON.stringify(data);
            alertTable.setData(obj);
            alertTable.setSort("id", "desc")
        },
        error: function () {
            Command: toastr["error"]("Could not pre-load plant data!");
        }
    });
};

// Local Functions

function startWateringPlantById(value) {
    let plantId = $(value).data('plantid');
    params.__RequestVerificationToken = $("[name='__RequestVerificationToken']").val();
    
    $.ajax({
        type: 'POST',
        url: '/api/v1/plant/StarWateringPlant/' + plantId,
        data: params,
        success: function (response) {
            GetPlantList();
            Command: toastr["success"]("Watering Task for plant with id : " + plantId + " created.");
        },
        error: function () {
            Command: toastr["error"]("Could not start watering Task!");
        }        
    });
}

function stopWateringPlantById(value) 
{
    let plantId = $(value).data('plantjobid');
    params.__RequestVerificationToken = $("[name='__RequestVerificationToken']").val();
    $.ajax({
        type: 'DELETE',
        url: '/api/v1/plant/StopWateringTask/' + plantId,
        data: params,
        success: function (response) {
            window.GetPlantList();
            Command: toastr["success"]("Watering Task for with id : " + plantId + " stopped.");
        },
        error: function () {
            Command: toastr["error"]("Could not stop watering!");
        }
    });
}