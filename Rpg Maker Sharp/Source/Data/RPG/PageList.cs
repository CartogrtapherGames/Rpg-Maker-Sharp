using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace RpgSharp.Data.RPG;

public class PageList
{
  public int Code { get; set; }
  public int? Indent { get; set; }
  public List<JToken> Parameters { get; set; }
}
