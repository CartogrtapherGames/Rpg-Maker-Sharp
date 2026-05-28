using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RpgSharp.Core;

public enum SpriteOrientation
{
  Normal,         // default
  FlipHorizontal, // flip the sprite horizontally
  FlipVertical,   // flip the sprite vertically
  Both            // flip both horizontally and vertically
}
/// <summary>
/// Represents a 2D renderable object that displays a texture in the scene graph.
/// A <see cref="OldSprite"/> is the primary visual element used for rendering images,
/// supporting transformations (position, rotation, scale), color tinting, and
/// orientation flips.
/// </summary>
/// <remarks>
/// The <see cref="OldSprite"/> class inherits from <see cref="DisplayObject"/>,
/// meaning it participates in the world transform hierarchy.
/// The texture is automatically disposed when the sprite is disposed, so it should
/// not be shared across multiple sprites unless handled externally.
/// </remarks>
/// TODO : Allow Bitmap?  ill rework this when needed

public class OldSprite : DisplayObject
{
  Vector2 anchor;
  Rectangle frame;
  SpriteOrientation orientation;
  Texture2D texture;

  public OldSprite()
  {
    Frame = Rectangle.Empty;
    Color = Color.White;
    Origin = Vector2.Zero;
    anchor = Vector2.Zero;
    orientation = SpriteOrientation.Normal;
  }

  /// <summary>
  ///  The sprite constructor.
  /// </summary>
  /// <param name="texture"> the sprite texture</param>
  public OldSprite(Texture2D texture) : this()
  {
    this.texture = texture;
  }

  public OldSprite(TextureAtlas textureAtlas, string regionName) : this(textureAtlas.Texture)
  {
    textureAtlas.GetRegion(regionName, out var region);
    Frame = region;
  }

  /// <summary>
  /// The sprite texture.
  /// </summary>
  public virtual Texture2D Texture
  {
    get => texture;
    set
    {
      if (TextureWrapper != null)
      {
        TextureWrapper.Texture = value;
        texture = TextureWrapper.Texture;
      }
      texture = value;
      Frame = texture?.Bounds ?? Rectangle.Empty;
      UpdateOriginFromAnchor();
    }
  }

  /// <summary>
  /// The sprite texture wrapper object used for allowing texture
  /// manipulation *such as blur*
  /// </summary>
  public Bitmap TextureWrapper { get; private set; }

  public Color Color { get; set; }


  public Rectangle Frame
  {
    get => frame;
    set
    {
      frame = value;
      UpdateOriginFromAnchor(); // ← THIS WAS MISSING
    }
  }

  public SpriteOrientation Orientation
  {
    get => orientation;
    set => orientation = value;
  }

  public Vector2 Anchor
  {
    get => anchor;
    set
    {
      anchor = value;
      UpdateOriginFromAnchor();
    }
  }

  public override Vector2 Origin
  {
    get => base.Origin;
    set
    {
      base.Origin = value;
      UpdateAnchorFromOrigin();
    }
  }

  public void SetScale(float x, float? y = null)
  {
    var scale = new Vector2(x, y ?? x);
    Scale = scale;
  } 
  public void BindTextureWrapper(Bitmap textureWrapper)
  {
    TextureWrapper = textureWrapper;
    texture = textureWrapper.Texture;
    Frame = Texture.Bounds;
    UpdateOriginFromAnchor();
  }

  public override Rectangle GetBounds()
  {
    if (Texture == null) return Rectangle.Empty;
    return new Rectangle(
      (int)-Origin.X,
      (int)-Origin.Y,
      Texture.Width,
      Texture.Height
    );
  }

  public override void Draw(SpriteBatch spriteBatch)
  {

    if (Texture == null || !this.Visible) return;

    var colorWithAlpha = Color * WorldAlpha;
    Rectangle? sourceRectangle = (Frame != Rectangle.Empty && Frame != Texture.Bounds)
      ? Frame
      : null;
    DecomposeMatrix2D(WorldTransform, out var position, out var rotation, out var scale);
    spriteBatch.Draw(
      Texture,
      position,
      sourceRectangle,
      colorWithAlpha,
      rotation,
      Vector2.Zero, // ✅ IMPORTANT
      scale,
      GetSpriteEffects(),
      Depth
    );
    base.Draw(spriteBatch);
  }

  /// <summary>
  /// Sets the frame of the texture based on its width, height, row, and column.
  /// </summary>
  /// <param name="width">The frame width.</param>
  /// <param name="height">The frame height.</param>
  /// <param name="row">The row where the frame is positioned (starting from 0).</param>
  /// <param name="col">The column where the frame is positioned (starting from 0).</param>
  public void SetFrame(int width, int height, int row, int col)
  {
    Frame = new Rectangle(
      col * width,
      row * height,
      width,
      height
    );
  }

  protected virtual SpriteEffects GetSpriteEffects()
  {
    return orientation switch
    {
      SpriteOrientation.Normal => SpriteEffects.None,
      SpriteOrientation.FlipHorizontal => SpriteEffects.FlipHorizontally,
      SpriteOrientation.FlipVertical => SpriteEffects.FlipVertically,
      SpriteOrientation.Both => SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically,
      _ => SpriteEffects.None
    };
  }
  protected override void Dispose(bool disposing)
  {
    texture?.Dispose();
    base.Dispose(true); // we call it last to make sure that the texture is disposed before the object.
  }

  protected void UpdateOriginFromAnchor()
  {
    if (Frame == Rectangle.Empty) return;

    // Origin MUST be frame-local
    base.Origin = new Vector2(
      Frame.Width * anchor.X,
      Frame.Height * anchor.Y
    );
  }

  protected void UpdateAnchorFromOrigin()
  {
    if (Frame == Rectangle.Empty || Frame.Width == 0 || Frame.Height == 0)
      return;

    anchor = new Vector2(
      base.Origin.X / Frame.Width,
      base.Origin.Y / Frame.Height
    );
  }
}
