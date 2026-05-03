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
    private string _statusMessage = "Ingresa usuario y contraseña";
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;
    private int _focusedField = 0; // 0=username, 1=password

    private Rectangle _usernameFieldRect;
    private Rectangle _passwordFieldRect;
    private Rectangle _connectButtonRect;

    public LoginScreen(GameClient gameClient, ClientGameState gameState, UIManager uiManager)
    {
        _gameClient = gameClient;
        _gameState = gameState;
        _uiManager = uiManager;
        _packetHandler = new ClientPacketHandler(gameState);
        _previousKeyboardState = Keyboard.GetState();
        _previousMouseState = Mouse.GetState();

        _gameClient.PacketReceived += (sender, packet) => _packetHandler.HandlePacket(packet);
    }

    public void Update(float deltaTime)
    {
        var keyState = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        if (keyState.IsKeyDown(Keys.Escape))
        {
            _previousKeyboardState = keyState;
            return;
        }

        HandleMouseInput(mouseState);
        HandleKeyboardInput(keyState);

        _previousKeyboardState = keyState;
        _previousMouseState = mouseState;
    }

    public void Draw(SpriteBatch spriteBatch, UIManager? uiManager = null)
    {
        var ui = uiManager ?? _uiManager;

        int centerX = 512;
        int startY = 100;

        // Title
        ui.DrawText(spriteBatch, "IMPERIUM AO", new Vector2(centerX - 80, startY), Color.Gold);
        ui.DrawText(spriteBatch, "LOGIN", new Vector2(centerX - 35, startY + 50), Color.LimeGreen);

        // Username label
        ui.DrawText(spriteBatch, "USUARIO:", new Vector2(centerX - 200, startY + 130), Color.White);
        _usernameFieldRect = new Rectangle(centerX - 200, startY + 160, 400, 40);
        DrawInputField(spriteBatch, ui, _usernameFieldRect, _username, _focusedField == 0);
        ui.DrawText(spriteBatch, _username, new Vector2(centerX - 190, startY + 170), Color.Yellow);

        // Password label
        ui.DrawText(spriteBatch, "CONTRASEÑA:", new Vector2(centerX - 200, startY + 230), Color.White);
        _passwordFieldRect = new Rectangle(centerX - 200, startY + 260, 400, 40);
        DrawInputField(spriteBatch, ui, _passwordFieldRect, new string('*', _password.Length), _focusedField == 1);
        ui.DrawText(spriteBatch, new string('*', _password.Length), new Vector2(centerX - 190, startY + 270), Color.Yellow);

        // Status message
        var statusColor = _statusMessage.Contains("Error") ? Color.Red :
                         _statusMessage.Contains("exitoso") ? Color.LimeGreen :
                         Color.Cyan;
        ui.DrawText(spriteBatch, _statusMessage, new Vector2(centerX - 250, startY + 330), statusColor);

        // Connect button
        _connectButtonRect = new Rectangle(centerX - 120, startY + 380, 240, 50);
        bool isButtonHovered = _connectButtonRect.Contains(Mouse.GetState().Position);
        DrawButton(spriteBatch, ui, _connectButtonRect, "CONECTAR", isButtonHovered);

        // Instructions
        ui.DrawText(spriteBatch, "TAB=Cambiar campo | ENTER=Conectar | ESC=Salir",
                   new Vector2(centerX - 280, startY + 480), Color.Gray);

        if (_isConnecting)
        {
            ui.DrawText(spriteBatch, "CONECTANDO...", new Vector2(centerX - 90, startY + 550), Color.Cyan);
        }
    }

    private void DrawInputField(SpriteBatch spriteBatch, UIManager ui, Rectangle rect, string text, bool focused)
    {
        var borderColor = focused ? Color.LimeGreen : Color.DarkGray;
        ui.DrawBox(spriteBatch, rect, borderColor, 2);

        if (focused)
        {
            // Blinking cursor effect
            var cursorX = rect.X + 10 + (text.Length * 8);
            ui.DrawBox(spriteBatch, new Rectangle(cursorX, rect.Y + 5, 2, rect.Height - 10), Color.White, 1);
        }
    }

    private void DrawButton(SpriteBatch spriteBatch, UIManager ui, Rectangle rect, string text, bool hovered)
    {
        var bgColor = hovered ? Color.DarkSlateBlue : Color.MidnightBlue;
        var textColor = hovered ? Color.White : Color.LightGray;
        ui.DrawButton(spriteBatch, rect, text, bgColor, textColor, hovered);
    }

    private void HandleMouseInput(MouseState mouseState)
    {
        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            var mousePos = mouseState.Position;

            if (_usernameFieldRect.Contains(mousePos))
                _focusedField = 0;
            else if (_passwordFieldRect.Contains(mousePos))
                _focusedField = 1;
            else if (_connectButtonRect.Contains(mousePos) && !_isConnecting)
                _ = TryLoginAsync();
        }
    }

    private void HandleKeyboardInput(KeyboardState keyState)
    {
        foreach (var key in keyState.GetPressedKeys())
        {
            if (_previousKeyboardState.IsKeyUp(key))
            {
                if (key == Keys.Tab)
                {
                    _focusedField = 1 - _focusedField;
                }
                else if (key == Keys.Enter && !_isConnecting)
                {
                    _ = TryLoginAsync();
                }
                else if (key == Keys.Back)
                {
                    if (_focusedField == 0 && _username.Length > 0)
                        _username = _username[..^1];
                    else if (_focusedField == 1 && _password.Length > 0)
                        _password = _password[..^1];
                }
                else
                {
                    char? c = KeyToChar(key, keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift));
                    if (c.HasValue)
                    {
                        if (_focusedField == 0)
                            _username += c.Value;
                        else
                            _password += c.Value;
                    }
                }
            }
        }
    }

    private char? KeyToChar(Keys key, bool shift)
    {
        if (key >= Keys.A && key <= Keys.Z)
        {
            char c = (char)(key - Keys.A + (shift ? 'A' : 'a'));
            return c;
        }
        else if (key >= Keys.D0 && key <= Keys.D9)
        {
            return (char)(key - Keys.D0 + '0');
        }
        else if (key == Keys.Space)
        {
            return ' ';
        }
        else if (key == Keys.OemMinus)
        {
            return shift ? '_' : '-';
        }

        return null;
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
                _statusMessage = "Login exitoso!";
            }
            else if (!string.IsNullOrEmpty(_gameState.LastError))
            {
                _statusMessage = $"Error: {_gameState.LastError}";
            }
            else
            {
                _statusMessage = "No se recibio respuesta del servidor";
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
