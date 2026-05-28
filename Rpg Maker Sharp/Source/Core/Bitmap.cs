using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RpgSharp.Core;

/// <summary>
/// The class that shape a bitmap
/// </summary>
public class Bitmap : IDisposable
{
  GraphicsDevice device;
  SpriteBatch spriteBatch;
  PingPongBuffer buffer;
  
  Texture2D texture;
  
  public Bitmap(GraphicsDevice graphicsDevice, int width, int height)
  {
    device = graphicsDevice;
    spriteBatch = new SpriteBatch(device);
    buffer = new PingPongBuffer(device, width, height);
  }
  public Bitmap(GraphicsDevice graphicDevice,Texture2D texture) : this(graphicDevice, texture.Width, texture.Height)
  {
    this.texture = texture;
  }

  public Texture2D Texture
  {
    get => texture;
    set
    {
      texture = value;
      buffer.Assign(texture);
    }
  }
  
  public int Width => texture.Width;
  public int Height => texture.Height;

  /// <summary>
  ///  copy a region of one texture to the current texture.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="sourceRect"></param>
  /// <param name="destRect"></param>
  public void Blit(Texture2D source, Rectangle sourceRect, Rectangle destRect)
  {
    if (texture == null) return;

    var renderTarget = buffer.Target;

    device.SetRenderTarget(renderTarget);
    device.Clear(Color.Transparent);

    spriteBatch.Begin();
    spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    spriteBatch.Draw(source, destRect, sourceRect, Color.White);
    spriteBatch.End();

    device.SetRenderTarget(null);
    buffer.Swap();
    texture = buffer.Source;
  }
  
  public void Blur(){}
  
  
  /// <summary>
  /// create a deep clone of the current texture.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public Texture2D Clone()
  {
    if(texture == null) throw new InvalidOperationException("Texture is null");
    
    var clonedTexture = new RenderTarget2D(device, texture.Width, texture.Height);
    device.SetRenderTarget(clonedTexture);
    device.Clear(Color.Transparent);
    
    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
    spriteBatch.Draw(texture, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
    spriteBatch.End();
    
    device.SetRenderTarget(null);
    return clonedTexture;
  }
  
  /// <summary>
  /// create a bitmap with a deep clone of the current texture.
  /// </summary>
  /// <returns></returns>
  public Bitmap CloneBitmap()
  {
    return new Bitmap(device, Clone());
  }
  
  public void Dispose()
  {
    buffer?.Dispose();
    texture?.Dispose();
    spriteBatch?.Dispose();
  }
}
