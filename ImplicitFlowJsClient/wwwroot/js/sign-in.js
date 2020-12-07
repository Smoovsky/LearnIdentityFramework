var signIn = function()
{
    var redirectUri = 'https://localhost:11001/signin';
    var responseType = 'id_token token';
    

    var authUrl = `/connect/authorize/callback?client_id=implicitFlowJsClient?redirect_uri=${encodeURIComponent(redirectUri)}?response_type=${encodeURIComponent(responseType)}`;

    var returnUrl = encodeURI();

    console.log(url);
}