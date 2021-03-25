/** @type{EventSource} */
var eSource = null;

function sse_connect() {
    $('#bConnectSSE').addClass("d-none");
    $('#bCloseSSE').removeClass("d-none");

    eSource = new EventSource("/sse/bind");
    eSource.addEventListener("append", (ev) => {
        var jsonData = JSON.parse(ev.data);
        $('#divResult>ul').append(`<li>#${jsonData.id} @ ${jsonData.time} => ${jsonData.payload}</li>`);
    });
}

function sse_close() {
    $('#bConnectSSE').removeClass("d-none");
    $('#bCloseSSE').addClass("d-none");

    eSource.close();
}
