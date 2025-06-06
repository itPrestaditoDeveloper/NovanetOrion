function ToastrSuccess(title, message) {
    toastr["success"](message+" , si usa el app movil, actualize la pagina para ver los cambios", title);
}

function ToastrWarning(title, message) {
    toastr["warning"](message, title);
}

function ToastrError(title, message) {
    toastr["error"](message, title);
}

function ToastrInfo(title, message) {
    toastr["info"](message, title);
}