var createSession = function () {
    return 'sessionFLSKDFJKLSDLFLK';
}

var createNounce = function () {
    return 'nounceJJDFEIUGJIRKJLSJS';
}


var signIn = function () {
    var redirectUri = 'https://localhost:11001/signin';
    var responseType = 'id_token token';
    var scope = 'openid ApiOne';

    var authUrl = `/connect/authorize/callback?client_id=implicitFlowJsClient&redirect_uri=${encodeURIComponent(redirectUri)}&response_type=${encodeURIComponent(responseType)}&scope=${scope}&nonce=${createNounce()}&state=${createSession()}`;

    var returnUrl = encodeURI(authUrl);

    console.log(returnUrl);

    window.location.href = 'https://localhost:6001/auth/login?ReturnUrl=' + returnUrl;
}