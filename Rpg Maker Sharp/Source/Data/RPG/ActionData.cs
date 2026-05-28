namespace RpgSharp.Data.RPG;

/// <summary>
/// The data object that shape an Action model
/// </summary>
public class ActionData
{
  public int ConditionParam1 { get; set; }
  public int ConditionParam2 { get; set; }
  public ActionConditionType ConditionType { get; set; }
  public int Rating { get; set; }
  public int SkillId { get; set; }
}
