using IdP.Entities;
using IdP.Enums;
using IdP.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IdP.Services;

internal class AuthenticationService(IAuthorizationRepository repository)
{
    public bool CanAuthenticate(AuthorizationHeader authorizationHeader)
    {
        return authorizationHeader.Type switch
        {
            AuthorizationType.Basic => new BasicAuthenticator(repository).CanAuthenticate(new Credentials(authorizationHeader.Value)),
            _ => false,
        };
    }

    private class BasicAuthenticator(IAuthorizationRepository repository)
    {
        private readonly PasswordHasher<string> _hasher = new();

        public bool CanAuthenticate(Credentials credentials)
        {
            string? hashedPassword = repository.GetHashedPassword(credentials);
            if (hashedPassword is null)
            {
                return false;
            }

            return _hasher.VerifyHashedPassword(credentials.UserIdentity, hashedPassword, credentials.Password) switch
            {
                PasswordVerificationResult.Success => true,
                PasswordVerificationResult.SuccessRehashNeeded => RehashCredentials(credentials),
                _ => false,
            };
        }
    
        private bool RehashCredentials(Credentials credentials)
        {
            string passwordHash = _hasher.HashPassword(credentials.UserIdentity, credentials.Password);
            repository.UpdateUserIdentity(credentials.UserIdentity, passwordHash);
            return true;
        }
    }
}
