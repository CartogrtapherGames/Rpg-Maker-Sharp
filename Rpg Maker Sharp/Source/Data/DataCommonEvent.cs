using RpgSharp.Data.RPG;

namespace RpgSharp.Data;

/// <summary>
/// The data object that shape a CommonEvent model
/// </summary>
public class DataCommonEvent
{
  public int Id { get; set; }
  public PageList[] List { get; set; }
  public string Name { get; set; }
  public int SwitchId { get; set; }
  public CommonEventTrigger Trigger { get; set; }
}
