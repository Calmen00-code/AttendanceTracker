$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    var employeeId = document.getElementById("employeeId").value;
    if (employeeId)
    {
        $('#attendanceTable').DataTable({
            "ajax": {
                "url": "/Admin/Admin/GetAllAttendanceRecordsOfEmployee?employeeId=" + employeeId,
                "type": "GET"
            },
            "columns": [
                { data: 'date', width: '30%' },
                { data: 'email', width: '40%' },
                { data: 'totalWorkingHours', width: '30%' },
            ]
        });
    }
    else
    {
        alert("Employee ID is not provided.");
    }
}
