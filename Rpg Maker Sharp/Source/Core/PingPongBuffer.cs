using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RpgSharp.Core;

internal enum PingPongBufferType
{
  A,
  B
}

/// <summary>
/// Provides a double-buffered render target system ("ping-pong" buffer)
/// for iterative texture operations and post-processing in MonoGame.
/// </summary>
/// <remarks>
/// The ping-pong technique alternates between two render targets — one
/// for reading (<see cref="Source"/>) and one for writing (<see cref="Target"/>)
/// — avoiding GPU read/write conflicts. This allows chained rendering
/// passes, feedback effects, and texture blitting without reallocating
/// textures or copying data between frames.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// var buffer = new PingPongBuffer(graphicsDevice, 1024, 1024);
///
/// // Draw to the current target
/// graphicsDevice.SetRenderTarget(buffer.Target);
/// spriteBatch.Begin();
/// spriteBatch.Draw(texture1, Vector2.Zero, Color.White);
/// spriteBatch.Draw(texture2, new Vector2(512, 0), Color.White);
/// spriteBatch.End();
/// graphicsDevice.SetRenderTarget(null);
///
/// // Swap read/write targets for the next iteration
/// buffer.Swap();
///
/// // Use the final composed texture
/// Texture2D result = buffer.Source;
/// </code>
/// </example>
/// <seealso href="https://en.wikipedia.org/wiki/Ping-pong_scheme">Ping-pong scheme (Wikipedia)</seealso>
public class PingPongBuffer : IDisposable
{
  
  private GraphicsDevice graphicsDevice;
  private SpriteBatch spriteBatch;
  private RenderTarget2D textureA;
  private RenderTarget2D textureB;
  private PingPongBufferType current;
  private bool hasBeenInitialized;
  
  /// <summary>
  /// The ping pong buffer constructor.
  /// </summary>
  /// <param name="device"></param>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <param name="format"></param>
  public PingPongBuffer(GraphicsDevice device, int width, int height, SurfaceFormat format = SurfaceFormat.Color)
  {
    graphicsDevice = device;
    spriteBatch = new SpriteBatch(graphicsDevice);
    textureA = CreateRenderTarget(width, height, format);
    textureB = CreateRenderTarget(width, height, format);
    current = PingPongBufferType.A;
    hasBeenInitialized = false;
  }

  /// <summary>
  /// create a render target.
  /// </summary>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <param name="format"></param>
  /// <returns></returns>
  private RenderTarget2D CreateRenderTarget(int width, int height, SurfaceFormat format)
  {
    return new RenderTarget2D(
      graphicsDevice,
      width,
      height,
      false,
      format,
      DepthFormat.None
    );
  }
  
  /// <summary>Gets the current source texture (read from this).</summary>
  public RenderTarget2D Source => current == PingPongBufferType.A ? textureA : textureB;
  
  /// <summary>Gets the current target texture (write to this).</summary>
  public RenderTarget2D Target => current == PingPongBufferType.A ? textureB : textureA;
  
  /// <summary>
  /// the buffer width.
  /// </summary>
  public int Width => textureA.Width;
  
  /// <summary>
  /// the buffer height. 
  /// </summary>
  public int Height => textureA.Height;
  
  /// <summary>
  /// Swap source and target buffers.
  /// </summary>
  public void Swap()
  {
    current = current == PingPongBufferType.A ? PingPongBufferType.B : PingPongBufferType.A;
  }


  /// <summary>
  /// Clear the current source to transparent.
  /// </summary>
  public void Clear()
  {
    graphicsDevice.SetRenderTarget(Source);
    graphicsDevice.Clear(Color.Transparent);
    graphicsDevice.SetRenderTarget(null);
  }

  /// <summary>
  /// Clear both buffers.
  /// </summary>
  public void ClearAll()
  {
    graphicsDevice.SetRenderTarget(textureA);
    graphicsDevice.Clear(Color.Transparent);

    graphicsDevice.SetRenderTarget(textureB);
    graphicsDevice.Clear(Color.Transparent);

    graphicsDevice.SetRenderTarget(null);
  }

  /// <summary>
  /// Resize both buffers.
  /// </summary>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <param name="format"></param>
  public void Resize(int width, int height, SurfaceFormat format = SurfaceFormat.Color)
  {
    textureA?.Dispose();
    textureB?.Dispose();
    textureA = CreateRenderTarget(width, height, format);
    textureB = CreateRenderTarget(width, height, format);
  }

  /// <summary>
  /// First assign a texture to the source and target buffers.
  /// </summary>
  /// <param name="texture"></param>
  public void Assign(Texture2D texture)
  {
    Resize(texture.Width, texture.Height, texture.Format);
    DrawToRenderTarget(texture, Source);
    DrawToRenderTarget(texture, Target);
    hasBeenInitialized = true;
  }

  /// <summary>
  /// draw to the render target.
  /// </summary>
  /// <param name="texture"></param>
  /// <param name="renderTarget"></param>
  private void DrawToRenderTarget(Texture2D texture, RenderTarget2D renderTarget)
  {
    graphicsDevice.SetRenderTarget(renderTarget);
    graphicsDevice.Clear(Color.Transparent);

    spriteBatch.Begin();
    spriteBatch.Draw(texture, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
    spriteBatch.End();

    graphicsDevice.SetRenderTarget(null);
  }
  
  /// <summary>
  /// Check if the buffer has been initialized.
  /// </summary>
  public bool HasSource => hasBeenInitialized;
  
  /// <summary>
  /// Dispose the buffer.
  /// </summary>
  public void Dispose()
  {
    graphicsDevice = null;
    textureA?.Dispose();
    textureB?.Dispose();
    spriteBatch?.Dispose();
    spriteBatch = null;
    textureA = null;
    textureB = null;
  }
}
