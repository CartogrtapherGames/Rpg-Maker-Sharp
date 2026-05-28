using System.Collections.Generic;
using RpgSharp.Data.RPG;

namespace RpgSharp.Data;

/// <summary>
/// The class that shape a map model
/// </summary>
public class DataMap
{
  public bool AutoplayBgm { get; set; }
  public bool AutoplayBgs { get; set; }
  public string Battleback1Name { get; set; }
  public string Battleback2Name { get; set; }
  public AudioObject Bgm { get; set; }
  public AudioObject Bgs { get; set; }
  public bool DisableDashing { get; set; }
  public string DisplayName { get; set; }
  public List<Encounter> EncounterList { get; set; }
  public int EncounterStep { get; set; }
  public int Height { get; set; }
  public string Note { get; set; }
  public bool ParallaxLoopX { get; set; }
  public bool ParallaxLoopY { get; set; }
  public string ParallaxName { get; set; }
  public bool ParallaxShow { get; set; }
  public int ParallaxSx { get; set; }
  public int ParallaxSy { get; set; }
  public int ScrollType { get; set; }
  public bool SpecifyBattleback { get; set; }
  public int TilesetId { get; set; }
  public int Width { get; set; }
  public int[] Data { get; set; }
  public List<RpgEvent> Events { get; set; }
}
