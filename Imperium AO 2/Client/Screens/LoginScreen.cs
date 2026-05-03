using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ImperiumAO.Common.Network;
using ImperiumAO.Client.Network;
using ImperiumAO.Client.GameState;

namespace ImperiumAO.Client.Screens;

public class LoginScreen : IScreen
{
    private readonly GameClient _gameClient;
    private readonly ClientGameState _gameState;
    private readonly ClientPacketHandler _packetHandler;

    private string _username = "";
    private string _password = "";
    private bool _isConnecting = false;
    private string _statusMessage = "Ingresa username y password";

    public LoginScreen(GameClient gameClient, ClientGameState gameState)
    {
        _gameClient = gameClient;
        _gameState = gameState;
        _packetHandler = new ClientPacketHandler(gameState);

        _gameClient.PacketReceived += (sender, packet) => _packetHandler.HandlePacket(packet);
    }

    public void Update(float deltaTime)
    {
        var keyState = Keyboard.GetState();

        if (keyState.IsKeyDown(Keys.Escape))
            return;

        if (keyState.IsKeyDown(Keys.Enter) && !_isConnecting)
        {
            _ = TryLoginAsync();
        }

        // Simple input handling (in real implementation, use text input event)
        if (keyState.IsKeyDown(Keys.Back) && !string.IsNullOrEmpty(_username))
        {
            _username = _username[..^1];
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            null,
            "=== LOGIN ===",
            new Vector2(300, 100),
            Color.White);

        spriteBatch.DrawString(
            null,
            $"Username: {_username}",
            new Vector2(300, 150),
            Color.White);

        spriteBatch.DrawString(
            null,
            $"Password: {new string('*', _password.Length)}",
            new Vector2(300, 200),
            Color.White);

        spriteBatch.DrawString(
            null,
            _statusMessage,
            new Vector2(300, 250),
            Color.Yellow);

        if (_isConnecting)
        {
            spriteBatch.DrawString(
                null,
                "Conectando...",
                new Vector2(300, 300),
                Color.Cyan);
        }

        spriteBatch.DrawString(
            null,
            "Presiona ENTER para conectar",
            new Vector2(300, 400),
            Color.Gray);
    }

    private async Task TryLoginAsync()
    {
        if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
        {
            _statusMessage = "Ingresa username y password";
            return;
        }

        _isConnecting = true;
        _statusMessage = "Conectando...";

        try
        {
            await _gameClient.ConnectAsync("localhost", 7666);
            _statusMessage = "Conectado. Enviando credenciales...";

            var loginPacket = new ByteBuffer();
            loginPacket.InitializeWriter();
            loginPacket.PutByte((byte)ClientPacketId.LoginExistingAccount);
            loginPacket.PutString(_username);
            loginPacket.PutString(_password);

            await _gameClient.SendPacketAsync(loginPacket);
            _statusMessage = "Aguardando respuesta...";

            // Wait a bit for response
            await Task.Delay(1000);

            if (_gameState.IsLoggedIn)
            {
                _statusMessage = "Login exitoso!";
            }
            else if (!string.IsNullOrEmpty(_gameState.LastError))
            {
                _statusMessage = $"Error: {_gameState.LastError}";
            }
        }
        catch (Exception ex)
        {
            _statusMessage = $"Error de conexión: {ex.Message}";
        }
        finally
        {
            _isConnecting = false;
        }
    }
}
