namespace RpgSharp.Data.RPG;

public enum DropItemKind
{
  None = 0,
  Item = 1,
  Weapon = 2,
  Armor = 3
}
public class DropItem
{
  public DropItemKind Kind { get; set; }
  public int DataId { get; set; }
  public int Denominator { get; set; }
}
