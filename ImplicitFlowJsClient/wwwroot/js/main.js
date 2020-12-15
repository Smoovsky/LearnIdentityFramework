let config = {
    authority: 'https://localhost:6001/',
    client_id: 'implicitFlowJsClient',
    response_type: 'id_token token',
    redirect_uri: 'https://localhost:11001/home/signin',
    scope: 'openid ApiOne'
};

let userManager = new Oidc.UserManager(config);