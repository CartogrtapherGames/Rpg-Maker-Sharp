using System.Collections.Generic;
using RpgSharp.Data.RPG;

namespace RpgSharp.Data;

public class DataTroop
{
  public int Id { get; set; }
  public string Name { get; set; }
  public List<TroopMember> Members { get; set; }
  public List<TroopPage> Pages { get; set; }
  public int Span { get; set; }
}
public class TroopMember
{
  public int EnemyId { get; set; }
  public int X { get; set; }
  public int Y { get; set; }
  public bool Hidden { get; set; }
}
public class TroopPage
{
  public BattleCondition Conditions { get; set; }
  public List<PageList> List { get; set; }
  public int Span { get; set; }
}
