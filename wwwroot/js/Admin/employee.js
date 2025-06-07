$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    $('#employeeTable').DataTable({
        "ajax": {
            "url": "/Admin/Admin/GetAllEmployees",
            "type": "GET"
        },
        "columns": [
            { data: 'employeeId', visible: false },
            { data: 'email', width: '50%' },
            { 
                data: null, 
                render: function(data, type, row) {
                    return `
                        <div class="row">
                            <div class="col-md-6">
                                <button class="btn btn-primary" onclick="viewEmployeeRecord('${row.employeeId}')">View</button>
                            </div>
                            <div class="col-md-6">
                                <button class="btn btn-danger" onclick="deleteEmployeeRecord('${row.employeeId}')">Delete</button>
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

function viewEmployeeRecord(employeeId) {
    window.location.href = `/Admin/Admin/ViewAttendanceRecordsOfEmployee?employeeId=${employeeId}`;
}

function deleteEmployeeRecord(employeeId) {
    window.location.href = `/Admin/Admin/DeleteEmployee?employeeId=${employeeId}`;
}
