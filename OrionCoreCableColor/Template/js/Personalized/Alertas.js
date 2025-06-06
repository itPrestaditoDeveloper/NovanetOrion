
function AlertaAjax(data) {
    swal.fire(data.Titulo, data.Mensaje, data.Estado ? 'success' : 'error');
}

function AlertaTempData(Estado, Titulo, Mensaje) {
    swal.fire(Titulo, Mensaje, Estado ? 'success' : 'error');
}

function AlertaError() {
    swal.fire("Error de red", "", 'error');
}

function AlertaErrorMensaje(Mensaje) {
    swal.fire(Mensaje, "", 'error');
}


function AlertaCrear(data, urlRetorno) {
    if (data.Estado) {
        swal.fire({
            html: "<hr>",
            title: '<span class="text-success">' + data.Titulo + '</span> <br> <small>' + data.Mensaje + '</small>',
            text: "Que desea hacer a continuacion?",
            type: "success",
            showCancelButton: true,
            confirmButtonText: 'Go to List',
            cancelButtonColor: '#739e73',
            cancelButtonText: 'Back',
            allowOutsideClick: false
        }).then(function () {
            window.location.href = urlRetorno;

        },
            function (dismiss) {
                if (dismiss === 'cancel') {
                    location.reload();


                }
            });
    } else {
        AlertaAjax(data);
    }
}




function DeshabilitarRegistro(Id, boton, url) {
    swal.fire({
        html: "<hr>",
        title: '<span class="text-danger">Deshabilitar Registro</span>',
        type: "warning",
        showCancelButton: true,
        confirmButtonText: 'Si',
        confirmButtonColor: '#d33',
        cancelButtonText: 'No',
        allowOutsideClick: false
    }).then(function () {
        $.ajax({
            url: url,
            type: "post",
            data: { id: Id },
            success: function (data) {
                if (data.Estado) {
                    $.fn.dataTable.Api($(boton).closest("table")).ajax.reload().draw(false);
                }
                AlertaAjax(data);

            },
            error: function (data) {

                swal('Error de red', '', 'error');

            },
        });
    });
}


function AlertaMensaje(mensaje) {
    swal.fire("", mensaje, 'warning');
}



function AlertaCrearGoToIndex(data, urlRetorno) {
    if (data.Estado) {
        swal.fire({
            html: "<hr>",
            title: '<span class="text-success">' + data.Titulo + '</span> <br> <small>' + data.Mensaje + '</small>',
            text: "Que desea hacer a continuacion?",
            type: "success",
            showCancelButton: false,
            confirmButtonText: 'Go to List',
            cancelButtonColor: '#739e73',
            cancelButtonText: 'Go Back',
            allowOutsideClick: false
        }).then(function () {
            window.location.href = urlRetorno;

        },
            function (dismiss) {
                if (dismiss === 'cancel') {
                    location.reload();


                }
            });
    } else {
        AlertaAjax(data);
    }
}