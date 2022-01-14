var dataTable;

$(document).ready(function () {
    laodDataTable();
});

function laodDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/NationalPark/GetAllNationalPark",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "name", "width": "50%" },
            { "data": "state", "width": "25%" },
            {
                "data": "id",
                "render": function (data) {

                    return `<div class="text-center">
                                <a href = "/NationalPark/Upsert/${data}" class="btn btn-success" ><i class="far fa-edit"></i></a >
                                <a onclick=Delete("/NationalPark/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                        <i class="far fa-trash-alt"></i></a>
                            </div >`
                },
                "width": "25%"
            }
        ]
    });

}

function Delete(url) {

    swal({
        title: "Are you sure you want to Delete? ",
        text: "You will not be able to restore the data!",
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
            swal("Your data is safe not deleted.");
        }
    });

}