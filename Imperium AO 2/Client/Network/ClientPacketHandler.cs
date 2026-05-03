using ImperiumAO.Common.Network;
using ImperiumAO.Client.GameState;

namespace ImperiumAO.Client.Network;

public class ClientPacketHandler
{
    private readonly ClientGameState _gameState;

    public ClientPacketHandler(ClientGameState gameState)
    {
        _gameState = gameState;
    }

    public void HandlePacket(ByteBuffer packet)
    {
        if (packet.GetLastPos() < 0)
            return;

        packet.GetVoid(-(int)packet.GetCurrentPos());
        var packetId = (ServerPacketId)packet.GetByte();

        switch (packetId)
        {
            case ServerPacketId.Logged:
                HandleLogged();
                break;
            case ServerPacketId.ChangeMap:
                HandleChangeMap(packet);
                break;
            case ServerPacketId.CharacterCreate:
                HandleCharacterCreate(packet);
                break;
            case ServerPacketId.ChatOverHead:
                HandleChatOverHead(packet);
                break;
            case ServerPacketId.PosUpdate:
                HandlePosUpdate(packet);
                break;
            case ServerPacketId.UpdateHP:
                HandleUpdateHP(packet);
                break;
            case ServerPacketId.UpdateGold:
                HandleUpdateGold(packet);
                break;
            case ServerPacketId.ErrorMsg:
                HandleErrorMsg(packet);
                break;
            case ServerPacketId.Pong:
                HandlePong();
                break;
        }
    }

    private void HandleLogged()
    {
        _gameState.IsLoggedIn = true;
    }

    private void HandleChangeMap(ByteBuffer packet)
    {
        var mapId = packet.GetByte();
        var x = packet.GetByte();
        var y = packet.GetByte();

        _gameState.CurrentMap = mapId;
        _gameState.PlayerX = x;
        _gameState.PlayerY = y;
    }

    private void HandleCharacterCreate(ByteBuffer packet)
    {
        var charId = packet.GetLong();
        var name = packet.GetString();
        var body = packet.GetByte();
        var head = packet.GetByte();
        var x = packet.GetByte();
        var y = packet.GetByte();
        var heading = packet.GetByte();

        _gameState.PlayerId = (int)charId;
        _gameState.PlayerName = name;
        _gameState.PlayerX = x;
        _gameState.PlayerY = y;
    }

    private void HandleChatOverHead(ByteBuffer packet)
    {
        var charId = packet.GetLong();
        var message = packet.GetString();

        _gameState.AddChatMessage($"{(int)charId}: {message}");
    }

    private void HandlePosUpdate(ByteBuffer packet)
    {
        var x = packet.GetByte();
        var y = packet.GetByte();

        _gameState.PlayerX = x;
        _gameState.PlayerY = y;
    }

    private void HandleUpdateHP(ByteBuffer packet)
    {
        var hp = packet.GetLong();
        _gameState.PlayerHealth = (int)hp;
    }

    private void HandleUpdateGold(ByteBuffer packet)
    {
        var gold = packet.GetLong();
        _gameState.PlayerGold = (int)gold;
    }

    private void HandleErrorMsg(ByteBuffer packet)
    {
        var message = packet.GetString();
        _gameState.LastError = message;
    }

    private void HandlePong()
    {
        _gameState.LastPongTime = System.DateTime.UtcNow;
    }
}
