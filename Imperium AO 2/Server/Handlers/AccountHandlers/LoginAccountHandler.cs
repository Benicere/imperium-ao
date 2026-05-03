using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Database;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Core;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers.AccountHandlers;

public class LoginAccountHandler : IPacketHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<LoginAccountHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.LoginExistingAccount;

    public LoginAccountHandler(
        IAccountRepository accountRepository,
        IConnectionManager connectionManager,
        ILogger<LoginAccountHandler> logger)
    {
        _accountRepository = accountRepository;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            packet.GetByte(); // Consume packet ID
            var username = packet.GetString();
            var password = packet.GetString();

            _logger.LogDebug("Connection {ConnectionId}: Login attempt for username: {Username}",
                connectionId, username);

            var account = await _accountRepository.GetByUsernameAsync(username);
            if (account == null)
            {
                _logger.LogWarning("Connection {ConnectionId}: Account not found: {Username}",
                    connectionId, username);
                var errorPacket = PacketWriter.ErrorMsg("Usuario o contraseña incorrectos");
                await connection.SendAsync(errorPacket, default);
                return;
            }

            var isValidPassword = await _accountRepository.ValidatePasswordAsync(account, password);
            if (!isValidPassword)
            {
                _logger.LogWarning("Connection {ConnectionId}: Invalid password for account: {AccountId}",
                    connectionId, account.Id);
                var errorPacket = PacketWriter.ErrorMsg("Usuario o contraseña incorrectos");
                await connection.SendAsync(errorPacket, default);
                return;
            }

            _logger.LogInformation("Connection {ConnectionId}: Account {AccountId} logged in successfully",
                connectionId, account.Id);

            var successPacket = PacketWriter.Logged();
            await connection.SendAsync(successPacket, default);

            // TODO: Store account info in connection context for later use
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoginAccountHandler");
            var errorPacket = PacketWriter.ErrorMsg("Error en el servidor");
            await connection.SendAsync(errorPacket, default);
        }
    }
}
