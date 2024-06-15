using IdP.Entities;
using IdP.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;

namespace IdP.Repositories;

internal class SqliteAuthorizationRepository : IAuthorizationRepository
{
    public static void InitializeDatabase()
    {
        string[] queries =
        [
            "DROP TABLE IF EXISTS [IdentityUser]",
            "DROP TABLE IF EXISTS [AuthorizationCode]",
            "CREATE TABLE [IdentityUser] (UserIdentity TEXT PRIMARY KEY, PasswordHash TEXT NOT NULL)",
            "CREATE TABLE [AuthorizationCode] (Code TEXT PRIMARY KEY, Expiration INT NOT NULL)",
           $"INSERT INTO [IdentityUser] VALUES ('Aladdin', '{new PasswordHasher<string>().HashPassword("Aladdin", "open sesame")}')",
        ];
    
        using SqliteConnection connection = new("Data Source=test.db");
        using SqliteCommand command = new(string.Join(';', queries), connection);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    public void ActivateAuthorizationCode(Guid code, long expiration)
    {
        using SqliteConnection connection = new("Data Source=test.db");
        using SqliteCommand command = new("INSERT INTO [AuthorizationCode] VALUES (@Code, @Expiration)", connection);
        command.Parameters.AddWithValue("@Code", code);
        command.Parameters.AddWithValue("@Expiration", expiration);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    public string? GetHashedPassword(Credentials credentials)
    {
        using SqliteConnection connection = new("Data Source=test.db");
        using SqliteCommand command = new("SELECT PasswordHash FROM [IdentityUser] WHERE UserIdentity = @UserIdentity LIMIT 1", connection);
        command.Parameters.AddWithValue("@UserIdentity", credentials.UserIdentity);
        connection.Open();
        string? hashedPassword = command.ExecuteScalar()?.ToString();
        connection.Close();
        return hashedPassword;
    }

    public void UpdateUserIdentity(string userIdentity, string passwordHash)
    {
        using SqliteConnection connection = new("Data Source=test.db");
        using SqliteCommand command =
            new("UPDATE [IdentityUser] SET PasswordHash = @PasswordHash WHERE UserIdentity = @UserIdentity");
        command.Parameters.AddWithValue("@UserIdentity", userIdentity);
        command.Parameters.AddWithValue("@PasswordHash", passwordHash);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }
}