namespace RpgSharp.Data;

public enum TilesetType
{
  Overworld = 0,
  Area = 1,
}
/// <summary>
/// The data object that shapes a Tileset model
/// </summary>
public class DataTileset : BaseData
{
  public int[] Flags { get; set; }
  public TilesetType TilesetType { get; set; }
  public string[] TilesetNames { get; set; }
}
