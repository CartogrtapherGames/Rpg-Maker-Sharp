namespace RpgSharp.Data;

/// <summary>
/// the data object class that shape a weapon model
/// </summary>
public class DataWeapon : DataEquipable, IDataGameItem
{
  public int WtypeId { get; set; }
  public int AnimationId { get; set; }
}
