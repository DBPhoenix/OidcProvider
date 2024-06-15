using System.Text;

namespace IdP.Entities;

internal struct Credentials
{
    public string UserIdentity { get; init; }
    public string Password { get; init; }

    public Credentials(string base64UserPass)
    {
        string credentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64UserPass));
        int credentialsSeparatorIndex = credentials.IndexOf(':');
        UserIdentity = credentials[..credentialsSeparatorIndex];
        Password = credentials[(credentialsSeparatorIndex + 1)..];
    }
}
