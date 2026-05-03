using ImperiumAO.Common.Network;
using System;

namespace ImperiumAO.Server.Network;

public class PacketWriter
{
    public static ByteBuffer CreatePacket(ServerPacketId packetId)
    {
        var buffer = new ByteBuffer();
        buffer.InitializeWriter();
        buffer.PutByte((byte)packetId);
        return buffer;
    }

    public static ByteBuffer Logged()
    {
        return CreatePacket(ServerPacketId.Logged);
    }

    public static ByteBuffer Disconnect()
    {
        return CreatePacket(ServerPacketId.Disconnect);
    }

    public static ByteBuffer ErrorMsg(string message)
    {
        var packet = CreatePacket(ServerPacketId.ErrorMsg);
        packet.PutString(message);
        return packet;
    }

    public static ByteBuffer ChangeMap(byte mapId, byte x, byte y)
    {
        var packet = CreatePacket(ServerPacketId.ChangeMap);
        packet.PutByte(mapId);
        packet.PutByte(x);
        packet.PutByte(y);
        return packet;
    }

    public static ByteBuffer UserIndexInServer(int userId)
    {
        var packet = CreatePacket(ServerPacketId.UserIndexInServer);
        packet.PutLong(userId);
        return packet;
    }

    public static ByteBuffer UserCharIndexInServer(int charId)
    {
        var packet = CreatePacket(ServerPacketId.UserCharIndexInServer);
        packet.PutLong(charId);
        return packet;
    }

    public static ByteBuffer CharacterCreate(int charId, string name, byte body, byte head,
        byte x, byte y, byte heading)
    {
        var packet = CreatePacket(ServerPacketId.CharacterCreate);
        packet.PutLong(charId);
        packet.PutString(name);
        packet.PutByte(body);
        packet.PutByte(head);
        packet.PutByte(x);
        packet.PutByte(y);
        packet.PutByte(heading);
        return packet;
    }

    public static ByteBuffer CharacterMove(int charId, byte x, byte y)
    {
        var packet = CreatePacket(ServerPacketId.CharacterMove);
        packet.PutLong(charId);
        packet.PutByte(x);
        packet.PutByte(y);
        return packet;
    }

    public static ByteBuffer PosUpdate(byte x, byte y)
    {
        var packet = CreatePacket(ServerPacketId.PosUpdate);
        packet.PutByte(x);
        packet.PutByte(y);
        return packet;
    }

    public static ByteBuffer ChatOverHead(int charId, string message)
    {
        var packet = CreatePacket(ServerPacketId.ChatOverHead);
        packet.PutLong(charId);
        packet.PutString(message);
        return packet;
    }

    public static ByteBuffer ConsoleMsg(string message)
    {
        var packet = CreatePacket(ServerPacketId.ConsoleMsg);
        packet.PutString(message);
        return packet;
    }

    public static ByteBuffer UpdateHP(int hp)
    {
        var packet = CreatePacket(ServerPacketId.UpdateHP);
        packet.PutLong(hp);
        return packet;
    }

    public static ByteBuffer UpdateGold(int gold)
    {
        var packet = CreatePacket(ServerPacketId.UpdateGold);
        packet.PutLong(gold);
        return packet;
    }

    public static ByteBuffer UpdateExp(int exp)
    {
        var packet = CreatePacket(ServerPacketId.UpdateExp);
        packet.PutLong(exp);
        return packet;
    }

    public static ByteBuffer UpdateMana(int mana)
    {
        var packet = CreatePacket(ServerPacketId.UpdateMana);
        packet.PutLong(mana);
        return packet;
    }

    public static ByteBuffer Pong()
    {
        return CreatePacket(ServerPacketId.Pong);
    }
}

