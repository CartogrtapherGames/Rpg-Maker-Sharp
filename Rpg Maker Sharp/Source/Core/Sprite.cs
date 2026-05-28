using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RpgSharp.Core;

public class Sprite : DisplayObject
{

  Vector2 anchor;
  Rectangle frame;
  SpriteOrientation orientation;
  Bitmap bitmap;

  public Sprite()
  {
    Frame = Rectangle.Empty;
    Color = Color.White;
    Origin = Vector2.Zero;
    anchor = Vector2.Zero;
    orientation = SpriteOrientation.Normal;
  }

  public Sprite(Bitmap bitmap) : this()
  {
    this.bitmap = bitmap;
  }

  /// <summary>
  /// The sprite bitmap
  /// </summary>
  public virtual Bitmap Bitmap
  {
    get => bitmap;
    set
    {
      bitmap = value;
      Frame = bitmap.Texture?.Bounds ?? Rectangle.Empty;
      UpdateOriginFromAnchor();
    }
  }
  
  public Color Color { get; set; }

  public Rectangle Frame
  {
    get => frame;
    set
    {
      frame = value;
      UpdateOriginFromAnchor();
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

  public override Rectangle GetBounds()
  {
    if (bitmap.Texture == null) return Rectangle.Empty;
    return new Rectangle(
      (int)-Origin.X,
      (int)-Origin.Y,
      bitmap.Width,
      bitmap.Height
    );
  }
  
  
  public override void Draw(SpriteBatch spriteBatch)
  {
    var texture = Bitmap.Texture;
    if (texture == null || !this.Visible) return;

    var colorWithAlpha = Color * WorldAlpha;
    Rectangle? sourceRectangle = (Frame != Rectangle.Empty && Frame != texture.Bounds)
      ? Frame
      : null;
    DecomposeMatrix2D(WorldTransform, out var position, out var rotation, out var scale);
    spriteBatch.Draw(
      texture,
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
    bitmap?.Dispose();
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
