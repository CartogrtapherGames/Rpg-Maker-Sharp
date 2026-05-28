using System.Collections.Generic;
using RpgSharp.Data.RPG;
using RpgSharp.Objects;

namespace RpgSharp.Data;

/// <summary>
/// The data object that shapes a State model
/// </summary>
public class DataState : BaseData, IWithTraits
{
  public int AutoRemovalTiming { get; set; }
  public int ChanceByDamage { get; set; }
  public int IconIndex { get; set; }
  public int MaxTurns { get; set; }
  public string Message1 { get; set; }
  public string Message2 { get; set; }
  public string Message3 { get; set; }
  public string Message4 { get; set; }
  public int MinTurns { get; set; }
  public int Motion { get; set; }
  public int Overlay { get; set; }
  public int Priority { get; set; }
  public bool ReleaseByDamage { get; set; }
  public bool RemoveAtBattleEnd { get; set; }
  public bool RemoveByDamage { get; set; }
  public bool RemoveByRestriction { get; set; }
  public bool RemoveByWalking { get; set; }
  public int Restriction { get; set; }
  public int StepsToRemove { get; set; }
  public List<TraitData> Traits { get; set; }
  public int MessageType { get; set; }
}
