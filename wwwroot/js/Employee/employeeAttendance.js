$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    var employeeId = document.getElementById("employeeId").value;
    if (employeeId)
    {
        $('#employeeAttendanceTable').DataTable({
            "ajax": {
                "url": "/Employee/Employee/GetEmployeeAttendances?employeeId=" + employeeId,
                "type": "GET"
            },
            "columns": [
                { data: 'date', width: '50%' },
                { data: 'totalWorkingHours', width: '50%' },
            ]
        });
    }
    else
    {
        alert("Employee ID is not provided.");
    }
}
