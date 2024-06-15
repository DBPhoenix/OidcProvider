namespace IdP.Entities;

internal class OidcRequest
{
    public string ResponseType { get; init; }
    public string[] Scopes { get; init; }
    public string ClientId { get; init; }
    public Uri? RedirectUri => _redirectUri;

    private readonly Uri? _redirectUri;

    public OidcRequest(string responseType, string scope, string clientId, string rawRedirectUri)
    {
        ResponseType = responseType;
        Scopes = scope.Split("%20");
        ClientId = clientId;
        Uri.TryCreate(rawRedirectUri, UriKind.Absolute, out _redirectUri);
    }

    public bool IsValid()
    {
        // TODO: MUST check if client id is trusted
        return ResponseType == "code" && Scopes.Contains("openid") && RedirectUri is not null;
    }
}
