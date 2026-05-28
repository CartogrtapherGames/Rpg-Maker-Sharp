using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RpgSharp.Core;

/// <summary>
/// The utility class that manage a texture atlas.
/// </summary>
public class TextureAtlas 
{
  private Texture2D texture;
  private readonly Dictionary<string, Rectangle> regions;
  
  /// <summary>
  /// The Atlas texture
  /// </summary>
  public Texture2D Texture => texture;
  
  /// <summary>
  /// The atlas regions.
  /// </summary>
  public IReadOnlyDictionary<string, Rectangle> Regions => regions;
  
  /// <summary>
  /// The texture atlas constructor.
  /// </summary>
  /// <param name="texture"> the atlas texture</param>
  public TextureAtlas(Texture2D texture)
  {
    this.texture = texture; 
    regions = new Dictionary<string, Rectangle>();
  }
  
  /// <summary>
  /// Add a region to the atlas.
  /// </summary>
  /// <param name="name"> the region name</param>
  /// <param name="region"> the region rect</param>
  /// <exception cref="ArgumentException"></exception>
  public void AddRegion(string name, Rectangle region)
  {
    if( !regions.TryAdd(name, region))
      throw new ArgumentException($"The region {name} already exists.");
  }
  
  /// <summary>
  /// Get the Atlas region by name.
  /// </summary>
  /// <param name="name"> the region name</param>
  /// <param name="region"> the region rectangle to copy the value to</param>
  /// <returns>return the textures region coordinates</returns>
  public bool GetRegion(string name, out Rectangle region)
  {
    return regions.TryGetValue(name, out region);
  }
  
  /// <summary>
  /// Parse a json file to create the atlas such as Texture Packer.
  /// </summary>
  /// <param name="json"></param>
  public void Parse(string json)
  {
    
  }
} 



