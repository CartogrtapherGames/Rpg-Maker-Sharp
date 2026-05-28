using System.Collections.Generic;

namespace RpgSharp.Data.RPG;

public class MoveRoute
{
  public List<PageList> List { get; set; }
  public bool Repeat { get; set; }
  public bool Skippable { get; set; }
  public bool Wait { get; set; }
}
