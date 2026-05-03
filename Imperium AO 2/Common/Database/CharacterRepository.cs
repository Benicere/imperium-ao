using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperiumAO.Common.Database.Models;
using ImperiumAO.Common.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace ImperiumAO.Common.Database;

public class CharacterRepository : ICharacterRepository
{
    private readonly DatabaseConfig _config;
    private readonly ILogger<CharacterRepository> _logger;

    public CharacterRepository(IOptions<DatabaseConfig> config, ILogger<CharacterRepository> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CharacterSummary>> GetCharactersByAccountAsync(int accountId)
    {
        try
        {
            using var connection = new MySqlConnection(_config.CharacterConnectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, cuenta_id, name, level, exp, race_id, class_id, genre_id,
                       body_id, head_id, weapon_id, helmet_id, shield_id, heading,
                       pos_map, pos_x, pos_y, gold, max_hp, max_man, max_sta
                FROM personaje
                WHERE cuenta_id = @accountId AND deleted = 0
                LIMIT 5";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@accountId", accountId);

            var characters = new List<CharacterSummary>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                characters.Add(new CharacterSummary(
                    Id: reader.GetInt32(0),
                    AccountId: reader.GetInt32(1),
                    Name: reader.GetString(2),
                    Level: reader.GetInt16(3),
                    Experience: reader.GetInt32(4),
                    RaceId: reader.GetByte(5),
                    ClassId: reader.GetByte(6),
                    GenreId: reader.GetByte(7),
                    BodyId: reader.GetInt32(8),
                    HeadId: reader.GetInt32(9),
                    WeaponId: reader.GetInt32(10),
                    HelmetId: reader.GetInt32(11),
                    ShieldId: reader.GetInt32(12),
                    Heading: reader.GetByte(13),
                    PosMap: reader.GetInt16(14),
                    PosX: reader.GetByte(15),
                    PosY: reader.GetByte(16),
                    Gold: reader.GetInt32(17),
                    Health: reader.GetInt16(18),
                    MaxHealth: reader.GetInt16(18),
                    Mana: reader.GetInt16(19),
                    MaxMana: reader.GetInt16(19),
                    Stamina: reader.GetInt16(20),
                    MaxStamina: reader.GetInt16(20)
                ));
            }

            return characters.AsReadOnly();
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
            using var connection = new MySqlConnection(_config.CharacterConnectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, cuenta_id, name, level, exp, race_id, class_id, genre_id,
                       body_id, head_id, weapon_id, helmet_id, shield_id, heading,
                       pos_map, pos_x, pos_y, gold, max_hp, max_man, max_sta
                FROM personaje
                WHERE id = @characterId AND deleted = 0";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@characterId", characterId);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            var player = new Player
            {
                Id = reader.GetInt32(0),
                AccountId = reader.GetInt32(1),
                Name = reader.GetString(2),
                Level = reader.GetInt16(3),
                Experience = reader.GetInt32(4),
                Race = reader.GetByte(5),
                Class = reader.GetByte(6),
                Gender = reader.GetByte(7),
                X = reader.GetByte(15),
                Y = reader.GetByte(16),
                Map = reader.GetInt16(14),
                MaxHealth = reader.GetInt16(18),
                Health = reader.GetInt16(18),
                MaxMana = reader.GetInt16(19),
                Mana = reader.GetInt16(19),
                MaxStamina = reader.GetInt16(20),
                Stamina = reader.GetInt16(20),
                Gold = reader.GetInt32(17)
            };

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
            using var connection = new MySqlConnection(_config.CharacterConnectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE personaje
                SET pos_map = @map, pos_x = @x, pos_y = @y, gold = @gold,
                    max_hp = @maxHp, max_man = @maxMana, max_sta = @maxStamina
                WHERE id = @characterId";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@map", character.Map);
            command.Parameters.AddWithValue("@x", character.X);
            command.Parameters.AddWithValue("@y", character.Y);
            command.Parameters.AddWithValue("@gold", character.Gold);
            command.Parameters.AddWithValue("@maxHp", character.MaxHealth);
            command.Parameters.AddWithValue("@maxMana", character.MaxMana);
            command.Parameters.AddWithValue("@maxStamina", character.MaxStamina);
            command.Parameters.AddWithValue("@characterId", character.Id);

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving character {CharacterId}", character.Id);
        }
    }
}
