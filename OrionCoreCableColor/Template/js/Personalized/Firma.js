var estadoFirma = 0;

function DrawCanvasFirma(contenedor, idcanvas, ancho = screen.width, alto = screen.height) {

    var canvasFirma = $(`<div class="row">
                        <div class="col-sm-12">
                            <canvas id="${idcanvas}" class="drawCanvas">
                                No tienes un buen navegador.
                            </canvas>
                            

                        </div>

                        <div class="col-sm-12">
                            <div class="clearfix" style="width:100%;">
            <input type="button" class="btn btn-warning float-right" id="draw-clearBtn-${idcanvas}" value="Borrar" /> </div>
            
        </div>
                        </div>
                    <div class="row">
                        <div class="col-md-12">
                            <input style="display: none;" type="range" id="puntero-${idcanvas}" min="1" default="1" max="5" width="10%" />
                        </div>
                    </div>`);

    $(contenedor).append(canvasFirma);
    stringInicial = document.getElementById("drawCanvas").toDataURL();
    var canvas = document.getElementById(idcanvas);
    canvas.width = ancho;
    canvas.height = alto - (alto * 0.5);

    window.requestAnimFrame = (function (callback) {
        return window.requestAnimationFrame ||
            window.webkitRequestAnimationFrame ||
            window.mozRequestAnimationFrame ||
            window.oRequestAnimationFrame ||
            window.msRequestAnimaitonFrame ||
            function (callback) {
                window.setTimeout(callback, 1000 / 60);

            };
    })();


    var canvas = document.getElementById(idcanvas);
    var ctx = canvas.getContext("2d");

    var drawImage = document.getElementById("draw-image-" + idcanvas);
    var clearBtn = document.getElementById("draw-clearBtn-" + idcanvas);
    clearBtn.addEventListener("click", function (e) {
        
        clearCanvas();
        drawImage.setAttribute("src", "");
    }, false);


    var drawing = false;
    var mousePos = { x: 0, y: 0 };
    var lastPos = mousePos;
    canvas.addEventListener("mousedown", function (e) {

        
        var punta = document.getElementById("puntero-" + idcanvas);
        console.log(e);
        drawing = true;
        lastPos = getMousePos(canvas, e);
    }, false);
    canvas.addEventListener("mouseup", function (e) {
        estadoFirma = 1;
        drawing = false;
    }, false);
    canvas.addEventListener("mousemove", function (e) {
     
        
        mousePos = getMousePos(canvas, e);
    }, false);

    canvas.addEventListener("touchstart", function (e) {
        mousePos = getTouchPos(canvas, e);
        console.log(mousePos);
        e.preventDefault();
        var touch = e.touches[0];
        var mouseEvent = new MouseEvent("mousedown", {
            clientX: touch.clientX,
            clientY: touch.clientY
        });
        canvas.dispatchEvent(mouseEvent);
    }, false);
    canvas.addEventListener("touchend", function (e) {
        e.preventDefault();
        var mouseEvent = new MouseEvent("mouseup", {});
        canvas.dispatchEvent(mouseEvent);
    }, false);
    canvas.addEventListener("touchleave", function (e) {
        e.preventDefault();
        var mouseEvent = new MouseEvent("mouseup", {});
        canvas.dispatchEvent(mouseEvent);
    }, false);
    canvas.addEventListener("touchmove", function (e) {
        e.preventDefault();
        var touch = e.touches[0];
        var mouseEvent = new MouseEvent("mousemove", {
            clientX: touch.clientX,
            clientY: touch.clientY
        });
        canvas.dispatchEvent(mouseEvent);
    }, false);

    function getMousePos(canvasDom, mouseEvent) {
        var rect = canvasDom.getBoundingClientRect();

        return {
            x: mouseEvent.clientX - rect.left,
            y: mouseEvent.clientY - rect.top
        };
    }

    function getTouchPos(canvasDom, touchEvent) {
        var rect = canvasDom.getBoundingClientRect();
        console.log(touchEvent);

        return {
            x: touchEvent.touches[0].clientX - rect.left,
            y: touchEvent.touches[0].clientY - rect.top
        };
    }

    function renderCanvas() {
        if (drawing) {
            var punta = document.getElementById("puntero-" + idcanvas);
            ctx.beginPath();
            ctx.moveTo(lastPos.x, lastPos.y);
            ctx.lineTo(mousePos.x, mousePos.y);
            console.log(punta.value);
            ctx.lineWidth = punta.value;
            ctx.stroke();
            ctx.closePath();
            lastPos = mousePos;
        }
    }

    function clearCanvas() {
        
        estadoFirma = 0;
        canvas.width = canvas.width;
    }

    // Allow for animation
    (function drawLoop() {
        requestAnimFrame(drawLoop);
        renderCanvas();
    })();

}