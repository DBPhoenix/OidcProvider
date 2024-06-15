using IdP.Entities;

namespace IdP.Interfaces;

internal interface IAuthorizationRepository
{
    public void ActivateAuthorizationCode(Guid code, long expiration);
    public string? GetHashedPassword(Credentials credentials);
    public void UpdateUserIdentity(string userIdentity, string passwordHash);
}
