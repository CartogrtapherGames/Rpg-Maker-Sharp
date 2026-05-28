using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RpgSharp.Core;

public interface IRenderable
{
  void Draw()
  {
    
  }
}

public interface IRenderableSpriteBatch : IRenderable
{
  void Draw(SpriteBatch spriteBatch);
}


public interface IRenderableGameTime : IRenderable {
  void Draw(GameTime gameTime);
}