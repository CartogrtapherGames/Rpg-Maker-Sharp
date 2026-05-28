using RpgSharp.Data.RPG;

namespace RpgSharp.Data;

/// <summary>
/// The data object that shape a Class model
/// </summary>
public class DataClass : BaseData
{
  public int[] ExpParams { get; set; }
  public TraitData[] Traits { get; set; }
  public Learning[] Learnings { get; set; }
  public int[][] Params { get; set; }
}
