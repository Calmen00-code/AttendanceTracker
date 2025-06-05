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
                { 
                    data: null, 
                    render: function (data, type, row) {
                        return `
                            <div class="row">
                                <div class="col-md-12">
                                    <button class="btn btn-primary" onclick="editEmployeeRecord('${employeeId}', '${row.date}')">Edit</button>
                                </div>
                            </div>
                            `;
                    }, 
                    orderable: true, 
                    searchable: true,
                    width: '50%'
                }
            ]
        });
    }
    else
    {
        alert("Employee ID is not provided.");
    }
}

function editEmployeeRecord(employeeId, date)
{
    window.location.href = `/Admin/Admin/EditAttendanceRecord?employeeId=${employeeId}&date=${date}`;
}
