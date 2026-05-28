using RpgSharp.Data.RPG;

namespace RpgSharp.Data;

/// <summary>
/// the data object that shape an enemy model
/// </summary>
public class DataEnemy : BaseData
{
  public ActionData[] Actions { get; set; }
  public int BattlerHue { get; set; }
  public string BattlerName { get; set; }
  public DropItem[] DropItems { get; set; }
  public int Exp { get; set; }
  public TraitData[] Traits { get; set; }
  public int Gold { get; set; }
  public int[] Params { get; set; }

}
