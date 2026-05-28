namespace RpgSharp.Data;

/// <summary>
/// The data object that shapes a MapInfo model
/// </summary>
public class DataMapInfo
{
  public int Id { get; set; }
  public bool Expanded { get; set; }
  public string Name { get; set; }
  public int Order { get; set; }
  public int ParentId { get; set; }
  public int ScrollX { get; set; }
  public int ScrollY { get; set; }
  public bool Quick { get; set; }
}
