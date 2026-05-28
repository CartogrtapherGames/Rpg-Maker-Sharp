using System.Collections.Generic;

namespace RpgSharp.Data;

/// <summary>
/// the data object that shape a SavefileInfo model
/// </summary>
public class SavefileInfo
{
  public string Title { get; set; }
  public List<FaceEntry> Faces { get; set; }
  public string Playtime { get; set; }
  public int Timestamp { get; set; }
}
public struct FaceEntry
{
  public string Name { get; set; }
  public int Index { get; set; }
}
