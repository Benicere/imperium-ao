using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ImperiumAO.Common.Database;
using ImperiumAO.Common.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Data;

public class AccountRepository : IAccountRepository
{
    private readonly ImperiumAOContext _context;
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(ImperiumAOContext context, ILogger<AccountRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Account?> GetByUsernameAsync(string username)
    {
        try
        {
            var entity = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == username && a.IsActive);

            if (entity == null)
                return null;

            return new Account(
                Id: entity.Id,
                Username: entity.Username,
                Email: entity.Email,
                PasswordHash: entity.PasswordHash,
                Salt: "",
                DateCreated: entity.CreatedAt,
                LastIp: null,
                DateLastLogin: entity.LastLogin,
                Credits: 0,
                Status: 0
            );
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
            var hash = HashPassword(password, "");
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
