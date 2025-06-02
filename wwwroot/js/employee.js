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
                    return `<button class="btn btn-primary" onclick="viewEmployeeRecord('${row.employeeId}')">View</button>`;
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