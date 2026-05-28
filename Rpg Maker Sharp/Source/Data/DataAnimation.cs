using RpgSharp.Data.RPG;

namespace RpgSharp.Data;

/// <summary>
/// The data object that shape an Animation model
/// </summary>
public class DataAnimation
{
  public int Id { get; set; }
  public int DisplayType { get; set; }
  public string EffectName { get; set; }
  public FlashTiming[] FlashTimings { get; set; }
  public string Name { get; set; }
  public int OffsetX { get; set; }
  public int OffsetY { get; set; }
  public Rotation Rotation { get; set; }
  public float Scale { get; set; }
  public SoundTiming[] SoundTimings { get; set; }
  public int Speed { get; set; }
}
public class DataAnimationMV
{
  public int Id { get; set; }
  public int Animation1Hue { get; set; }
  public int Animation2Hue { get; set; }
  public string Animation1Name { get; set; }
  public string Animation2Name { get; set; }
  public int[][] Frames { get; set; }
}
