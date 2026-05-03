using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperiumAO.Common.Database;
using ImperiumAO.Common.Database.Models;
using ImperiumAO.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Data;

public class CharacterRepository : ICharacterRepository
{
    private readonly ImperiumAOContext _context;
    private readonly ILogger<CharacterRepository> _logger;

    public CharacterRepository(ImperiumAOContext context, ILogger<CharacterRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CharacterSummary>> GetCharactersByAccountAsync(int accountId)
    {
        try
        {
            var players = await _context.Players
                .Where(p => p.AccountId == accountId)
                .Take(5)
                .ToListAsync();

            var summaries = players.Select(p => new CharacterSummary(
                Id: p.Id,
                AccountId: p.AccountId,
                Name: p.Name,
                Level: p.Level,
                Experience: p.Experience,
                RaceId: (byte)p.Race,
                ClassId: (byte)p.Class,
                GenreId: (byte)p.Gender,
                BodyId: 0,
                HeadId: 0,
                WeaponId: 0,
                HelmetId: 0,
                ShieldId: 0,
                Heading: 0,
                PosMap: p.Map,
                PosX: (byte)p.X,
                PosY: (byte)p.Y,
                Gold: p.Gold,
                Health: p.Health,
                MaxHealth: p.MaxHealth,
                Mana: p.Mana,
                MaxMana: p.MaxMana,
                Stamina: p.Stamina,
                MaxStamina: p.MaxStamina
            )).ToList();

            return summaries.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting characters for account {AccountId}", accountId);
            return new List<CharacterSummary>();
        }
    }

    public async Task<Player?> LoadCharacterAsync(int characterId)
    {
        try
        {
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.Id == characterId);

            return player;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading character {CharacterId}", characterId);
            return null;
        }
    }

    public async Task SaveCharacterAsync(Player character)
    {
        try
        {
            _context.Players.Update(character);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving character {CharacterId}", character.Id);
        }
    }
}
