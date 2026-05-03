using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Database;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Core;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers.AccountHandlers;

public class SelectCharacterHandler : IPacketHandler
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IPlayerManager _playerManager;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<SelectCharacterHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.LoginExistingChar;

    public SelectCharacterHandler(
        ICharacterRepository characterRepository,
        IPlayerManager playerManager,
        IConnectionManager connectionManager,
        ILogger<SelectCharacterHandler> logger)
    {
        _characterRepository = characterRepository;
        _playerManager = playerManager;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            packet.GetByte(); // Consume packet ID
            var characterIndex = packet.GetByte();

            _logger.LogDebug("Connection {ConnectionId}: Select character request - index: {CharacterIndex}",
                connectionId, characterIndex);

            // TODO: Get account ID from connection context
            // For now, use a placeholder
            var accountId = 1;

            var characters = await _characterRepository.GetCharactersByAccountAsync(accountId);
            if (characterIndex >= characters.Count)
            {
                _logger.LogWarning("Connection {ConnectionId}: Invalid character index: {CharacterIndex}",
                    connectionId, characterIndex);
                var errorPacket = PacketWriter.ErrorMsg("Personaje inválido");
                await connection.SendAsync(errorPacket, default);
                return;
            }

            var characterSummary = characters[characterIndex];
            var character = await _characterRepository.LoadCharacterAsync(characterSummary.Id);
            if (character == null)
            {
                _logger.LogWarning("Connection {ConnectionId}: Failed to load character: {CharacterId}",
                    connectionId, characterSummary.Id);
                var errorPacket = PacketWriter.ErrorMsg("Error al cargar el personaje");
                await connection.SendAsync(errorPacket, default);
                return;
            }

            _logger.LogInformation("Connection {ConnectionId}: Character {CharacterId} selected",
                connectionId, character.Id);

            // Send character info
            await connection.SendAsync(PacketWriter.UserCharIndexInServer(character.Id), default);
            await connection.SendAsync(PacketWriter.UserIndexInServer(character.Id), default);

            // Send map and position
            await connection.SendAsync(
                PacketWriter.ChangeMap((byte)character.Map, (byte)character.X, (byte)character.Y),
                default);

            // Create character in world
            await connection.SendAsync(
                PacketWriter.CharacterCreate(character.Id, character.Name, 0, 0,
                    (byte)character.X, (byte)character.Y, 3),
                default);

            // Send stats
            await connection.SendAsync(PacketWriter.UpdateHP(character.Health), default);
            await connection.SendAsync(PacketWriter.UpdateMana(character.Mana), default);
            await connection.SendAsync(PacketWriter.UpdateGold(character.Gold), default);

            // TODO: Store character in connection context for later use
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SelectCharacterHandler");
            var errorPacket = PacketWriter.ErrorMsg("Error en el servidor");
            await connection.SendAsync(errorPacket, default);
        }
    }
}
