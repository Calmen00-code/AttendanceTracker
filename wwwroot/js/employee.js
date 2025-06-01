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
            { data: 'userName', width: '50%' },
            { 
                data: null, 
                render: function(data, type, row) {
                    return `<button class="btn btn-primary" onclick="viewEmployeeRecord('${row.id}')">View</button>`;
                }, 
                orderable: true, 
                searchable: true,
                width: '50%'
            }
        ]
    });
}
