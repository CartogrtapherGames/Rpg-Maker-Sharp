using System.Collections.Generic;
using RpgSharp.Data.RPG;
using RpgSharp.Objects;

namespace RpgSharp.Data;

/**
 * The class that shape an equipable model
 */
public abstract class DataEquipable : DataItemBase , IDataGameItem, IWithTraits
{
  public int EtypeId { get; set; }
  public List<TraitData> Traits { get; set; }
  public int[] Params { get; set; }
  public int Price { get; set; }
}
