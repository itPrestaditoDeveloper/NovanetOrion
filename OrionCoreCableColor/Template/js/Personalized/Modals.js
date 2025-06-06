var IdSelectActual;
function VerModalCrearParaSelect(ActionName, IdSelect) {

    $.ajax({
        url: '/Mantenimientos/' + ActionName,
        type: 'GET',
        cache: false,
        success: function (data) {
            IdSelectActual = IdSelect;
            ShowModal("Crear Nuevo", data);
        }
    });
}

//function ShowModalBig(data, scroll = 0
function ShowModalBig(data) {
    $("#MyModalContentBig").html(data);
   // $("#MyModalBig .modal-body").css("height", '80vh');
    $("#MyModalBig").modal("show");

}
function ShowModalBigScroll(data) {
    $("#MyModalContentBig").html(data);
     $("#MyModalBig .modal-body").css("height", '80vh');
    $("#MyModalBig").modal("show");

}
function ShowModalBig2Scroll(data) {
    $("#MyModalContentBig2").html(data);
    $("#MyModalBig2 .modal-body").css("height", '80vh');
    $("#MyModalBig2").modal("show");

}
function ShowModalBigSignalR(data) {
    $("#MyModalContentBigSignalR").html(data);
    // $("#MyModalBig .modal-body").css("height", '80vh');
    $("#MyModalBigSignalR").modal("show");

}


function ShowModalMensajeDirecto(data) {
    $("#MyModalContentMensaje").html(data);
    // $("#MyModalBig .modal-body").css("height", '80vh');
    $("#MyModalMensaje").modal("show");

}

function ShowModalBigScrollSignalR(data) {
    $("#MyModalContentBigSignalR").html(data);
    $("#MyModalBigSignalR .modal-body").css("height", '80vh');
    $("#MyModalBigSignalR").modal("show");

}

function ShowModalSmall(data) {
    $("#MyModalContentSmall").html(data);
    $("#MyModalSm").modal("show");

}

function ShowModal(data) {
    $("#MyModalContent").html(data);
    $("#MyModal").modal("show");

}

function ExpandirModal(boton) {
    $("#MyModalSize").toggleClass("modal-dialog");
}

function CerrarModal() {
    $("#MyModal").modal("hide");
    $("#MyModalBig").modal("hide");
    $("#MyModalSm").modal("hide");
    $("#MyModalBig2").modal("hide");
    $("#modal").modal("hide");
    $("#MyModalBody").empty();
    $("#MyModalBodyBig").empty();
    $("#MyModalBodySm").empty();
    $("#MyModalBig2").empty();
    $("#MyFormModal").empty();
    $("#MyFormModal").modal("hide");
    
}

function CerrarModalSignalR() {
    $("#MyModalBigSignalR").modal("hide");
    $("#MyModalContentBigSignalR").empty();
}


function CerrarMensaje() {
    $("#MyModalMensaje").modal("hide");
    $("#MyModalContentMensaje").empty();
}


function VerModalVistaPrevia(id, url) {
    $.ajax({
        url: url,
        method: "get",
        data: {id:id},
        success: function (resp) {
            ShowModal(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}

function VerModalConObjetoDeParametro(objParameter, url) {
    $.ajax({
        url: url,
        method: "get",
        data: objParameter,
        success: function (resp) {
            ShowModalBigScroll(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}


function VerModalScrollConObjetoDeParametro(objParameter, url) {
    $.ajax({
        url: url,
        method: "get",
        data: objParameter,
        success: function (resp) {
            ShowModalBigScroll(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}
function VerModalScroll2ConObjetoDeParametro(objParameter, url) {
    $.ajax({
        url: url,
        method: "get",
        data: objParameter,
        success: function (resp) {
            ShowModalBig2Scroll(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}


function VerModalSmConObjetoDeParametro(objParameter, url) {
    $.ajax({
        url: url,
        method: "get",
        data: objParameter,
        success: function (resp) {
            ShowModalSmall(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}


function VerModalMdConObjetoDeParametro(objParameter, url) {
    $.ajax({
        url: url,
        method: "get",
        data: objParameter,
        success: function (resp) {
            ShowModal(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}

function VerModalSignalRScrollConObjetoDeParametro(objParameter, url) {
    $.ajax({
        url: url,
        method: "get",
        data: objParameter,
        success: function (resp) {
            ShowModalBigScrollSignalR(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}

function VerModalSignalRConObjetoDeParametro(objParameter, url) {
    $.ajax({
        url: url,
        method: "get",
        data: objParameter,
        success: function (resp) {
            ShowModalBigSignalR(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}

function VerModalMensajeDirectoConObjetoDeParametro(objParameter, url) {
    $.ajax({
        url: url,
        method: "get",
        data: objParameter,
        success: function (resp) {
            ShowModalMensajeDirecto(resp);
        },
        error: function (resp) {
            alert("Error");
        }
    });
}


function SuitAlert(params, callback) {

    // Establecer la función de retorno de llamada
    window.callbackFunc = callback;

    //Vaciar el modal
    $('#overlay').empty();

    // Crear elementos para el modal
    var overlay = document.getElementById('overlay');
    var notification = document.createElement('div');
    notification.id = 'notification';
    var buttonsContainer = document.createElement('div');
    buttonsContainer.id = 'buttons';

    // Configurar contenido
    notification.innerHTML = '<h2>' + params.title + '</h2><p>' + params.content + '</p>';

    // Configurar botones
    for (var i = 0; i < params.buttons.length; i++) {
        let buttonText = params.buttons[i].replace('[', '').replace(']', '');
        let button = document.createElement('button');
        button.onclick = function () { callbackFunc(buttonText); };
        button.innerHTML = buttonText;

        buttonsContainer.appendChild(button);
    }

    // Agregar elementos al DOM
    notification.appendChild(buttonsContainer);
    overlay.appendChild(notification);
    document.body.appendChild(overlay);

    // Mostrar overlay y notificación
    overlay.style.display = 'flex';
    notification.style.display = 'block';

}

function CerrarSuitAlert() {
    var overlay = document.getElementById('overlay');
    overlay.style.display = 'none';
}
