using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ImperiumAO.Common.Network;
using ImperiumAO.Client.Network;
using ImperiumAO.Client.GameState;
using ImperiumAO.Client.UI;

namespace ImperiumAO.Client.Screens;

public class GameScreen : IScreen
{
    private readonly GameClient _gameClient;
    private readonly ClientGameState _gameState;
    private readonly ClientPacketHandler _packetHandler;

    private float _pingTimer = 0;

    public GameScreen(GameClient gameClient, ClientGameState gameState)
    {
        _gameClient = gameClient;
        _gameState = gameState;
        _packetHandler = new ClientPacketHandler(gameState);

        _gameClient.PacketReceived += (sender, packet) => _packetHandler.HandlePacket(packet);
    }

    public void Update(float deltaTime)
    {
        var keyState = Keyboard.GetState();

        // Movement
        if (keyState.IsKeyDown(Keys.W))
            SendWalkPacket(0);
        if (keyState.IsKeyDown(Keys.A))
            SendWalkPacket(3);
        if (keyState.IsKeyDown(Keys.S))
            SendWalkPacket(2);
        if (keyState.IsKeyDown(Keys.D))
            SendWalkPacket(1);

        // Chat
        if (keyState.IsKeyDown(Keys.Enter))
        {
            // TODO: Implement chat input
        }

        // Ping
        _pingTimer += deltaTime;
        if (_pingTimer > 30)
        {
            SendPing();
            _pingTimer = 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch, UIManager? uiManager = null)
    {
        if (uiManager == null) return;

        uiManager.DrawText(spriteBatch, "=== GAME WORLD ===", new Vector2(10, 10), Color.Gold);

        uiManager.DrawText(spriteBatch, $"Player: {_gameState.PlayerName} (ID: {_gameState.PlayerId})", new Vector2(10, 40), Color.White);
        uiManager.DrawText(spriteBatch, $"Position: ({_gameState.PlayerX}, {_gameState.PlayerY})", new Vector2(10, 70), Color.White);
        uiManager.DrawText(spriteBatch, $"HP: {_gameState.PlayerHealth}/{_gameState.PlayerMaxHealth}", new Vector2(10, 100), Color.LimeGreen);
        uiManager.DrawText(spriteBatch, $"Gold: {_gameState.PlayerGold}", new Vector2(10, 130), Color.Yellow);

        int chatY = 400;
        uiManager.DrawText(spriteBatch, "=== CHAT ===", new Vector2(10, chatY), Color.LimeGreen);

        chatY += 30;
        foreach (var message in _gameState.ChatMessages)
        {
            uiManager.DrawText(spriteBatch, message, new Vector2(10, chatY), Color.White);
            chatY += 20;
        }

        uiManager.DrawText(spriteBatch, "Controles: WASD=Mover, ENTER=Chat, ESC=Salir", new Vector2(10, 550), Color.Gray);
    }

    private void SendWalkPacket(byte direction)
    {
        try
        {
            var packet = new ByteBuffer();
            packet.InitializeWriter();
            packet.PutByte((byte)ClientPacketId.Walk);
            packet.PutByte(direction);

            _ = _gameClient.SendPacketAsync(packet);
        }
        catch { }
    }

    private void SendPing()
    {
        try
        {
            var packet = new ByteBuffer();
            packet.InitializeWriter();
            packet.PutByte((byte)ClientPacketId.Ping);

            _ = _gameClient.SendPacketAsync(packet);
        }
        catch { }
    }
}
