using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ImperiumAO.Common.Database.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace ImperiumAO.Common.Database;

public class AccountRepository : IAccountRepository
{
    private readonly DatabaseConfig _config;
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(IOptions<DatabaseConfig> config, ILogger<AccountRepository> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public async Task<Account?> GetByUsernameAsync(string username)
    {
        try
        {
            using var connection = new MySqlConnection(_config.AccountConnectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, username, email, password, salt, date_created, last_ip,
                       date_last_login, creditos, status
                FROM cuentas
                WHERE username = @username AND status = 0";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            var account = new Account(
                Id: reader.GetInt32(0),
                Username: reader.GetString(1),
                Email: reader.GetString(2),
                PasswordHash: reader.GetString(3),
                Salt: reader.GetString(4),
                DateCreated: reader.GetDateTime(5),
                LastIp: reader.IsDBNull(6) ? null : reader.GetString(6),
                DateLastLogin: reader.GetDateTime(7),
                Credits: reader.GetInt32(8),
                Status: reader.GetByte(9)
            );

            return account;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by username: {Username}", username);
            return null;
        }
    }

    public Task<bool> ValidatePasswordAsync(Account account, string password)
    {
        try
        {
            var hash = HashPassword(password, account.Salt);
            var isValid = hash == account.PasswordHash;
            return Task.FromResult(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating password for account {AccountId}", account.Id);
            return Task.FromResult(false);
        }
    }

    private static string HashPassword(string password, string salt)
    {
        var input = Encoding.UTF8.GetBytes(password + salt);
        var hash = SHA256.HashData(input);
        return Convert.ToHexString(hash).ToLower();
    }
}
