using System.Collections.Generic;
using RpgSharp.Data.RPG;
using RpgSharp.Objects;

namespace RpgSharp.Data;

/// <summary>
/// The data object that shape a Class model
/// </summary>
public class DataClass : BaseData, IWithTraits
{
  public int[] ExpParams { get; set; }
  public List<TraitData> Traits { get; set; }
  public Learning[] Learnings { get; set; }
  public int[][] Params { get; set; }
}
