using System.Threading.Tasks;
using ImperiumAO.Common.Database.Models;

namespace ImperiumAO.Common.Database;

public interface IAccountRepository
{
    Task<Account?> GetByUsernameAsync(string username);
    Task<bool> ValidatePasswordAsync(Account account, string password);
}
