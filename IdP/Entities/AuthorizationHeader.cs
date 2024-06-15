using IdP.Enums;

namespace IdP.Entities;

internal struct AuthorizationHeader
{
    public AuthorizationType Type { get; init; }
    public string Value { get; init; }

    internal AuthorizationHeader(string rawAuthorizationHeader)
    {
        int authorizationHeaderSeparatorIndex = rawAuthorizationHeader.IndexOf(' ');
        Type = rawAuthorizationHeader[..authorizationHeaderSeparatorIndex] switch
        {
            "Basic" => AuthorizationType.Basic,
            _ => AuthorizationType.Unknown,
        };
        Value = rawAuthorizationHeader[(authorizationHeaderSeparatorIndex + 1)..];
    }
}
