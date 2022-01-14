var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Trails/GetAllTrail",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "nationalPark.name", "width": "20%" },
            { "data": "name", "width": "20%" },
            { "data": "distance", "width": "20%" },
            { "data": "elevation", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center"> 
                                <a href="/Trails/Upsert/${data}" class="btn btn-success"><i class="far fa-edit"></i></a>
                                <a onclick=Delete("/Trails/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer")><i class="far fa-trash-alt"></i></a>
                            </div>`
                },
                "width": "20%"
            }
        ]

    });
}

function Delete(url) {

    swal({
        title: "Are you sure you want to delete ?",
        text: "You will not able to restore the data !",
        icon: "warning",
        buttons: true,
        dangerMode: true

    }).then((willDelete) => {

        if (willDelete) {
            $.ajax({
                type: "Delete",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });

        } else {
            swal("Your data is safe not deleted");
        }



    });

}