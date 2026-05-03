using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ImperiumAO.Client.Screens;
using ImperiumAO.Client.GameState;
using ImperiumAO.Client.Network;
using ImperiumAO.Client.UI;

namespace ImperiumAO.Client;

public class ImperiumGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;
    private IScreen? _currentScreen;
    private GameClient? _gameClient;
    private ClientGameState? _gameState;
    private UIManager? _uiManager;

    public ImperiumGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 768;
    }

    protected override void Initialize()
    {
        _gameClient = new GameClient();
        _gameState = new ClientGameState();
        _uiManager = new UIManager();

        _currentScreen = new LoginScreen(_gameClient, _gameState, _uiManager);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _uiManager?.LoadContent(Content, GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _currentScreen?.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        if (_spriteBatch != null)
        {
            _spriteBatch.Begin();
            _currentScreen?.Draw(_spriteBatch, _uiManager);
            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _gameClient?.Dispose();
            _spriteBatch?.Dispose();
            _uiManager?.Dispose();
        }
        base.Dispose(disposing);
    }
}
