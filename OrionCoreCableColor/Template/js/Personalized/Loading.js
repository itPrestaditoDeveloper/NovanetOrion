function UnloadWaitNotification() {
    $("#modalWaitingTime").modal("hide");
    $('#statusID').hide();
}

function LoadWaitNotification() {
    $("#modalWaitingTime").modal({ backdrop: 'static', keyboard: false }, "show");
    $('#statusID').fadeIn();
    UnloadWaitNotification();

}



$(document).bind("ajaxSend", function () {
    LoadWaitNotification();

}).bind("ajaxComplete", function () {
    UnloadWaitNotification();
});


$(document).ajaxSend(function () {
    LoadWaitNotification();
});

$(document).ajaxComplete(function () {
    UnloadWaitNotification();
});

window.onbeforeunload = function () {
    LoadWaitNotification();
};

window.onload = function () {
    UnloadWaitNotification();
};