using Microsoft.Xna.Framework.Graphics;
using ImperiumAO.Client.UI;

namespace ImperiumAO.Client.Screens;

public interface IScreen
{
    void Update(float deltaTime);
    void Draw(SpriteBatch spriteBatch, UIManager? uiManager = null);
}
