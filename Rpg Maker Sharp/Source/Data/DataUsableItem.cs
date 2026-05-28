using RpgSharp.Data.RPG;

namespace RpgSharp.Data;

/// <summary>
/// The data object that shapes a UsableItem model
/// </summary>
public abstract class DataUsableItem : DataItemBase
{
  public int AnimationId { get; set; }
  public ItemDamage Damage { get; set; }
  public ItemEffect[] Effects { get; set; }
  public HitType HitType { get; set; }
  public OccasionType Occasion { get; set; }
  public ScopeType Scope { get; set; }
  public int Speed { get; set; }
  public int SuccessRate { get; set; }
  public int TpGain { get; set; }
}
