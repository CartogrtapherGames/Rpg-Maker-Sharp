namespace RpgSharp.Data.RPG;

public class BattleCondition
{
  public int ActorHp { get; set; }
  public int ActorId { get; set; }
  public bool ActorValid { get; set; }
  public int EnemyHp { get; set; }
  public int EnemyIndex { get; set; }
  public bool EnemyValid { get; set; }
  public int SwitchId { get; set; }
  public bool SwitchValid { get; set; }
  public int TurnA { get; set; }
  public int TurnB { get; set; }
  public bool TurnEnding { get; set; }
  public bool TurnValid { get; set; }
}
