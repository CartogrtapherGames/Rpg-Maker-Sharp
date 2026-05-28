namespace RpgSharp.Data.RPG;

public class ItemDamage
{
  public bool Critical { get; set; }
  public int ElementId { get; set; }
  public string Formula { get; set; }
  public HitType Type { get; set; }
  public int Variance { get; set; }
}
