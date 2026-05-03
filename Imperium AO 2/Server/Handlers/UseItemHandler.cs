using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Inventory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ImperiumAO.Server.Handlers;

public class UseItemHandler : IPacketHandler
{
    private readonly IInventorySystem _inventorySystem;
    private readonly ILogger<UseItemHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.UseItem;

    public UseItemHandler(
        IInventorySystem inventorySystem,
        ILogger<UseItemHandler> logger)
    {
        _inventorySystem = inventorySystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var itemId = packet.GetInteger();
            var success = _inventorySystem.UseItem(connectionId, itemId);
            _logger.LogInformation($"Player {connectionId} used item {itemId}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling use item packet");
        }
    }
}

