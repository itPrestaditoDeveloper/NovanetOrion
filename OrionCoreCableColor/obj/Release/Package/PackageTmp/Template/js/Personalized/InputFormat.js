


//jQuery.extend(jQuery.validator.messages, {
//    required: "Este campo es obligatorio.",
//    remote: "Por favor, rellena este campo.",
//    email: "Por favor, escribe una dirección de correo válida",
//    url: "Por favor, escribe una URL válida.",
//    date: "Por favor, escribe una fecha válida.",
//    dateISO: "Por favor, escribe una fecha (ISO) válida.",
//    number: "Por favor, escribe un número entero válido.",
//    digits: "Por favor, escribe sólo dígitos.",
//    creditcard: "Por favor, escribe un número de tarjeta válido.",
//    equalTo: "Por favor, escribe el mismo valor de nuevo.",
//    accept: "Por favor, escribe un valor con una extensión aceptada.",
//    maxlength: jQuery.validator.format("Por favor, no escribas más de {0} caracteres."),
//    minlength: jQuery.validator.format("Por favor, no escribas menos de {0} caracteres."),
//    rangelength: jQuery.validator.format("Por favor, escribe un valor entre {0} y {1} caracteres."),
//    range: jQuery.validator.format("Escriba un valor entre {0} y {1}."),
//    max: jQuery.validator.format("Escriba un valor menor o igual a {0}."),
//    min: jQuery.validator.format("Escriba un valor mayor o igual a {0}.")
//});

//$.datepicker.regional['es'] = {
//    closeText: 'Cerrar',
//    prevText: '< Ant',
//    nextText: 'Sig >',
//    currentText: 'Hoy',
//    monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
//    monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'may', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
//    dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
//    dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
//    dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
//    weekHeader: 'Sm',
//    dateFormat: 'dd/mm/yy',
//    firstDay: 1,
//    isRTL: false,
//    showMonthAfterYear: false,
//    yearSuffix: ''
//};
//$.datepicker.setDefaults($.datepicker.regional['es']);

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57)) return false;
    return true;
}



$(document).ready(function () {
    //$('.DateTimeClass').bootstrapMaterialDatePicker({ weekStart: 0, time: false });

    //$(".DateTimeClass").datepicker({
    //    dateFormat: 'dd/MM/yy',
    //    changeMonth: true,
    //    changeYear: true,
    //    prevText: '<i class="fa fa-chevron-left"></i>',
    //    nextText: '<i class="fa fa-chevron-right"></i>',
    //});

    $(document).on('keyup', ".InputPorcentaje", function (e) {
        var valor = $(this).val();
        if (valor > 100) $(this).val(100);
        if (valor < 0) $(this).val(0);
    });

    $(document).on('keypress', "input[type=number]", function (e) {
        var teclaPulsada = window.event ? window.event.keyCode : e.which;
        var valor = $(this).val();
        if ($(this).hasClass("InputDecimal") || $(this).hasClass("InputPorcentaje")) {
            if (teclaPulsada === 46 && valor.indexOf(".") === -1) return true;
            return /\d/.test(String.fromCharCode(teclaPulsada));
        }
        if ($(this).hasClass("InputConNegativos")) {
            if (teclaPulsada === 45 && valor.indexOf("-") === -1) $(this).val("-" + valor);
        }

        if (!isNumberKey(e)) e.preventDefault();
    });



    $(document).on('focus', "input[type=number]", function (e) {
        $(this).on('mousewheel.disableScroll', function (e) {
            e.preventDefault();
        });
    });

    $(document).on('blur', "input[type=number]", function (e) {
        $(this).off('mousewheel.disableScroll');
    });

    $('table').on('focus', 'input[type=number]', function (e) {
        $(this).on('mousewheel.disableScroll', function (e) {
            e.preventDefault();
        });
    });

    $('table').on('blur', 'input[type=number]', function (e) {
        $(this).off('mousewheel.disableScroll');
    });





});