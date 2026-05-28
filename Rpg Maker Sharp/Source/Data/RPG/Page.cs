using System.Collections.Generic;

namespace RpgSharp.Data.RPG;

public class Page
{
  public EventConditions Conditions { get; set; }
  public bool DirectionFix { get; set; }
  public Image Image { get; set; }
  public List<PageList> List { get; set; }
  public int MoveFrequency { get; set; }
  public MoveRoute MoveRoute { get; set; }
  public int MoveSpeed { get; set; }
  public MoveType MoveType { get; set; }
  public int PriorityType { get; set; }
  public bool StepAnime { get; set; }
  public bool Through { get; set; }
  public int Trigger { get; set; }
  public bool WalkAnime { get; set; }
}
