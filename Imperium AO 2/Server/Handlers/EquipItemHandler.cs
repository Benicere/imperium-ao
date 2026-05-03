using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Inventory;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers;

public class EquipItemHandler : IPacketHandler
{
    private readonly IEquipmentSystem _equipmentSystem;
    private readonly ILogger<EquipItemHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.EquipItem;

    public EquipItemHandler(IEquipmentSystem equipmentSystem, ILogger<EquipItemHandler> logger)
    {
        _equipmentSystem = equipmentSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var slotId = packet.GetByte();
            var itemId = packet.GetInteger();
            var itemName = packet.GetString();
            var armor = packet.GetInteger();
            var damageBonus = packet.GetInteger();

            if (!Enum.TryParse<EquipmentSlot>(slotId.ToString(), out var slot))
            {
                _logger.LogWarning($"Invalid equipment slot: {slotId}");
                return;
            }

            var item = new EquippedItem
            {
                ItemId = itemId,
                Slot = slot,
                Name = itemName,
                ArmorValue = armor,
                DamageBonus = damageBonus,
                EquippedAt = DateTime.UtcNow,
                MaxDurability = 100,
                Durability = 100
            };

            if (_equipmentSystem.EquipItem(connectionId, item))
            {
                _logger.LogInformation($"Player {connectionId} equipped {itemName}");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling equip item packet");
        }
    }
}
