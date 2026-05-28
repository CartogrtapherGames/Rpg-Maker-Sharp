namespace RpgSharp.Data;

/// <summary>
/// The base class that shape all item data
/// </summary>
public abstract class DataItemBase : BaseData
{
  public string Description { get; set; }
  public int IconIndex { get; set; }
}
