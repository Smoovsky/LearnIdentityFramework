

var extractToken = function () {
    var url = window.location.href;

    var returnValue = url.split('#')[1];

    var values = returnValue.split('&');

    for (var value of values) {
        var kv = value.split('=');

        localStorage.setItem(kv[0], kv[1]);
    }
}