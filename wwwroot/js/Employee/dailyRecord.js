$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    var employeeId = document.getElementById("employeeId").value;
    if (employeeId)
    {
        $('#dailyRecordTable').DataTable({
            "ajax": {
                "url": "/QR/Home/GetUserDailyRecord?employeeId=" + employeeId,
                "type": "GET"
            },
            "columns": [
                { data: 'checkIn', width: '30%' },
                { data: 'checkOut', width: '40%' },
            ]
        });
    }
    else
    {
        alert("Employee ID is not provided.");
    }
}
