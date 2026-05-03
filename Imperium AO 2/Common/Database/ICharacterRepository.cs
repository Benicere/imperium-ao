using System.Collections.Generic;
using System.Threading.Tasks;
using ImperiumAO.Common.Database.Models;
using ImperiumAO.Common.Entities;

namespace ImperiumAO.Common.Database;

public interface ICharacterRepository
{
    Task<IReadOnlyList<CharacterSummary>> GetCharactersByAccountAsync(int accountId);
    Task<Player?> LoadCharacterAsync(int characterId);
    Task SaveCharacterAsync(Player character);
}
