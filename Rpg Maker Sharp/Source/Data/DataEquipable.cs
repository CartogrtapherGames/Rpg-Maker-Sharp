using RpgSharp.Data.RPG;

namespace RpgSharp.Data;

/**
 * The class that shape an equipable model
 */
public abstract class DataEquipable : DataItemBase
{
  public int ETypeId { get; set; }
  public TraitData[] Traits { get; set; }
  public int[] Params { get; set; }
  public int Price { get; set; }
}
