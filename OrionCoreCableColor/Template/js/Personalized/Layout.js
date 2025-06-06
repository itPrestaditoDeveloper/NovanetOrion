function MostrarCrearRegistro() {
    $.ajax({
        url: "/Registro/CrearRegistro",
        method: "get",
        //data: { id: dataRow.Pk_IdPrestamo },
        success: function (resp) {
            
            ShowModal(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}

function MostrarCrearCuenta() {
    $.ajax({
        url: "/Cuenta/CrearCuenta",
        method: "get",
        success: function (resp) {
            console.log(resp);
            ShowModal(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}

function MostrarEditarRegistro(id) {
    $.ajax({
        url: "/Registro/EditarRegistro",
        method: "get",
        data: { id: id },
        success: function (resp) {
            ShowModal(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}

