using Microsoft.Xna.Framework.Graphics;

namespace ImperiumAO.Client.Screens;

public interface IScreen
{
    void Update(float deltaTime);
    void Draw(SpriteBatch spriteBatch);
}
