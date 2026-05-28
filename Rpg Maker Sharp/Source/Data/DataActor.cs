using System.Collections.Generic;
using RpgSharp.Data.RPG;
using RpgSharp.Objects;

namespace RpgSharp.Data;

/// <summary>
/// the data object that shape an Actor model
/// </summary>
public class DataActor : BaseData, IWithTraits
{
  public string BattlerName { get; set; }
  public int CharacterIndex { get; set; }
  public string CharacterName { get; set; }
  public int ClassId { get; set; }
  public int[] Equips { get; set; }
  public int FaceIndex { get; set; }
  public string FaceName { get; set; }
  public List<TraitData> Traits { get; set; }
  public int InitialLevel { get; set; }
  public int MaxLevel { get; set; }
  public string Nickname { get; set; }
  public string Profile { get; set; }
}
