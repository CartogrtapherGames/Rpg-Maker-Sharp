namespace RpgSharp.Data;

/// <summary>
/// The data object that shapes a Skill model
/// </summary>
public class DataSkill : DataUsableItem, IDataGameItem
{
  public string Message1 { get; set; }
  public string Message2 { get; set; }
  public int MpCost { get; set; }
  public int StypeId { get; set; }
  public int TpCost { get; set; }
  public int RequiredWtypeId1 { get; set; }
  public int RequiredWtypeId2 { get; set; }
  public int MessageType { get; set; }

}
