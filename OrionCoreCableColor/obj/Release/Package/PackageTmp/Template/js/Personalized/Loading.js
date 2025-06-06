function LoadWaitNotification() {
    $("#modalWaitingTime").modal({ backdrop: 'static', keyboard: false }, "show");
    $('#statusID').fadeIn();
}

function UnloadWaitNotification() {
    $("#modalWaitingTime").modal("hide");
    $('#statusID').hide();
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