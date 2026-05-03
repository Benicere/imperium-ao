using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ImperiumAO.Common.Network;
using ImperiumAO.Client.Network;
using ImperiumAO.Client.GameState;
using ImperiumAO.Client.UI;

namespace ImperiumAO.Client.Screens;

public class LoginScreen : IScreen
{
    private readonly GameClient _gameClient;
    private readonly ClientGameState _gameState;
    private readonly ClientPacketHandler _packetHandler;
    private readonly UIManager _uiManager;

    private string _username = "";
    private string _password = "";
    private bool _isConnecting = false;
    private string _statusMessage = "Ingresa username y password";
    private KeyboardState _previousKeyboardState;

    public LoginScreen(GameClient gameClient, ClientGameState gameState, UIManager uiManager)
    {
        _gameClient = gameClient;
        _gameState = gameState;
        _uiManager = uiManager;
        _packetHandler = new ClientPacketHandler(gameState);
        _previousKeyboardState = Keyboard.GetState();

        _gameClient.PacketReceived += (sender, packet) => _packetHandler.HandlePacket(packet);
    }

    public void Update(float deltaTime)
    {
        var keyState = Keyboard.GetState();

        if (keyState.IsKeyDown(Keys.Escape))
        {
            _previousKeyboardState = keyState;
            return;
        }

        HandleInput(keyState);
        _previousKeyboardState = keyState;
    }

    public void Draw(SpriteBatch spriteBatch, UIManager? uiManager = null)
    {
        var ui = uiManager ?? _uiManager;

        int centerX = 512;
        int startY = 150;

        ui.DrawText(spriteBatch, "=== IMPERIUM AO ===", new Vector2(centerX - 100, startY), Color.Gold);
        ui.DrawText(spriteBatch, "LOGIN", new Vector2(centerX - 40, startY + 40), Color.White);

        ui.DrawText(spriteBatch, "Usuario:", new Vector2(centerX - 150, startY + 100), Color.White);
        ui.DrawBox(spriteBatch, new Rectangle(centerX - 150, startY + 130, 300, 30), Color.DarkGray, 1);
        ui.DrawText(spriteBatch, _username, new Vector2(centerX - 140, startY + 135), Color.White);

        ui.DrawText(spriteBatch, "Contraseña:", new Vector2(centerX - 150, startY + 180), Color.White);
        ui.DrawBox(spriteBatch, new Rectangle(centerX - 150, startY + 210, 300, 30), Color.DarkGray, 1);
        ui.DrawText(spriteBatch, new string('*', _password.Length), new Vector2(centerX - 140, startY + 215), Color.White);

        var statusColor = _statusMessage.Contains("Error") ? Color.Red :
                         _statusMessage.Contains("exitoso") ? Color.LimeGreen :
                         Color.Yellow;
        ui.DrawText(spriteBatch, _statusMessage, new Vector2(centerX - 200, startY + 280), statusColor);

        if (_isConnecting)
        {
            ui.DrawText(spriteBatch, "Conectando...", new Vector2(centerX - 80, startY + 320), Color.Cyan);
        }

        ui.DrawButton(spriteBatch, new Rectangle(centerX - 100, startY + 360, 200, 40), "Conectar", Color.DarkSlateBlue, Color.White);

        ui.DrawText(spriteBatch, "ESC para salir | ENTER para conectar", new Vector2(centerX - 200, startY + 450), Color.Gray);
    }

    private void HandleInput(KeyboardState keyState)
    {
        // Register key for characters
        foreach (var key in keyState.GetPressedKeys())
        {
            if (_previousKeyboardState.IsKeyUp(key))
            {
                if (key == Keys.Enter && !_isConnecting)
                {
                    _ = TryLoginAsync();
                }
                else if (key == Keys.Back && !string.IsNullOrEmpty(_username))
                {
                    _username = _username[..^1];
                }
                else if (key >= Keys.A && key <= Keys.Z)
                {
                    char c = (char)(key - Keys.A + 'a');
                    if (keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift))
                        c = (char)(key - Keys.A + 'A');
                    _username += c;
                }
                else if (key >= Keys.D0 && key <= Keys.D9)
                {
                    _username += (char)(key - Keys.D0 + '0');
                }
                else if (key == Keys.Space)
                {
                    _username += ' ';
                }
            }
        }
    }

    private async Task TryLoginAsync()
    {
        if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
        {
            _statusMessage = "Ingresa usuario y contraseña";
            return;
        }

        _isConnecting = true;
        _statusMessage = "Conectando al servidor...";

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
            _statusMessage = "Aguardando respuesta del servidor...";

            await Task.Delay(2000);

            if (_gameState.IsLoggedIn)
            {
                _statusMessage = "¡Login exitoso!";
            }
            else if (!string.IsNullOrEmpty(_gameState.LastError))
            {
                _statusMessage = $"Error: {_gameState.LastError}";
            }
            else
            {
                _statusMessage = "No se recibió respuesta del servidor";
            }
        }
        catch (Exception ex)
        {
            _statusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            _isConnecting = false;
        }
    }
}
