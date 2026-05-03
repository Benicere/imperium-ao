using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ImperiumAO.Client.UI;

public class UIManager
{
    private SpriteFont? _defaultFont;
    private Texture2D? _whitePixel;

    public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
    {
        try
        {
            _defaultFont = content.Load<SpriteFont>("Fonts/DefaultFont");
        }
        catch
        {
            _defaultFont = null;
        }

        _whitePixel = new Texture2D(graphicsDevice, 1, 1);
        _whitePixel.SetData(new[] { Color.White });
    }

    public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
    {
        if (_defaultFont != null)
        {
            spriteBatch.DrawString(_defaultFont, text, position, color);
        }
        else
        {
            DrawSimpleText(spriteBatch, text, position, color);
        }
    }

    public void DrawBox(SpriteBatch spriteBatch, Rectangle rect, Color color, int borderWidth = 2)
    {
        if (_whitePixel == null) return;

        spriteBatch.Draw(_whitePixel, new Rectangle(rect.X, rect.Y, rect.Width, borderWidth), color);
        spriteBatch.Draw(_whitePixel, new Rectangle(rect.X, rect.Y + rect.Height - borderWidth, rect.Width, borderWidth), color);
        spriteBatch.Draw(_whitePixel, new Rectangle(rect.X, rect.Y, borderWidth, rect.Height), color);
        spriteBatch.Draw(_whitePixel, new Rectangle(rect.X + rect.Width - borderWidth, rect.Y, borderWidth, rect.Height), color);
    }

    public void DrawButton(SpriteBatch spriteBatch, Rectangle rect, string text, Color backColor, Color textColor, bool isHovered = false)
    {
        if (_whitePixel == null) return;

        var color = isHovered ? Color.Lerp(backColor, Color.White, 0.3f) : backColor;
        spriteBatch.Draw(_whitePixel, rect, color);
        DrawBox(spriteBatch, rect, Color.White);
        DrawText(spriteBatch, text, new Vector2(rect.X + 10, rect.Y + 5), textColor);
    }

    private void DrawSimpleText(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
    {
        if (_whitePixel == null) return;

        int x = (int)position.X;
        int y = (int)position.Y;
        const int charWidth = 6;
        const int charHeight = 8;

        foreach (char c in text)
        {
            if (c == '\n')
            {
                y += charHeight + 2;
                x = (int)position.X;
            }
            else if (c >= 32 && c < 127)
            {
                var rect = new Rectangle(x, y, charWidth, charHeight);
                spriteBatch.Draw(_whitePixel, rect, color);
                x += charWidth + 1;
            }
        }
    }

    public void Dispose()
    {
        _whitePixel?.Dispose();
    }
}
