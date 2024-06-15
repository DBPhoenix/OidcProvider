using IdP.Entities;
using IdP.Enums;
using IdP.Interfaces;
using IdP.Repositories;
using IdP.Services;
using Microsoft.AspNetCore.Mvc;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);

WebApplication app = builder.Build();

SqliteAuthorizationRepository.InitializeDatabase();

app.MapGet("/authorization", (
    [FromHeader(Name = "Authorization")] string rawAuthorizationHeader,
    [FromQuery(Name = "response_type")] string responseType,
    [FromQuery(Name = "scope")] string scope,
    [FromQuery(Name = "client_id")] string clientId,
    [FromQuery(Name = "redirect_uri")] string redirectUri) =>
{
    // TODO: MUST protect against brute force attacks

    OidcRequest oidc = new(responseType, scope, clientId, redirectUri);
    if (!oidc.IsValid())
    {
        // TODO: SHOULD redirect with error information 
        return Results.BadRequest();
    }

    AuthorizationHeader authorizationHeader = new(rawAuthorizationHeader);
    if (authorizationHeader.Type == AuthorizationType.Unknown)
    {
        // TODO: SHOULD redirect with error information 
        return Results.StatusCode(502);
    }

    IAuthorizationRepository repository = new SqliteAuthorizationRepository();

    AuthenticationService authenticator = new(repository);
    if (!authenticator.CanAuthenticate(authorizationHeader))
    {
        // TODO: SHOULD redirect with error information 
        return Results.Unauthorized();
    }

    Guid code = Guid.NewGuid();
    long expiration = DateTimeOffset.Now.AddMinutes(5).ToUnixTimeSeconds();
    repository.ActivateAuthorizationCode(code, expiration);

    return Results.Redirect($"{oidc.RedirectUri!.AbsoluteUri}?code={code}");
});

app.MapPost("/token", () =>
{

});

app.Run();

