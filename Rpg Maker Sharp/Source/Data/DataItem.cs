using Rpg_Maker_Sharp.Enums;

namespace RpgSharp.Data;

/// <summary>
/// The data object that shape an Item model
/// </summary>
public class DataItem : DataUsableItem, IDataGameItem
{
  public ItemType ItypeId { get; set; }
  public bool Consumable { get; set; }
}
