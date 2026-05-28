using RpgSharp.Management;

namespace RpgSharp.Objects;

/// <summary>
/// The class that manages the game switches
/// </summary>
public class GameSwitches
{
  bool[] data;

  public GameSwitches()
  {
    Clear();
  }

  public void Clear() => data = new bool[DataManager.DataSystem.Switches.Length];

  /// <summary>
  /// Gets or sets a game switch by its id
  /// </summary>
  /// <param name="id"></param>
  public bool this[int id]
  {
    get => id > 0 && id < data.Length && data[id];
    set
    {
      if (id > 0 && id < data.Length)
      {
        data[id] = value;
        OnChange();
      }
    } 
  }

  void OnChange()
  {
    DataManager.GameMap?.RequestRefresh();
  }
  
  /// <summary>
  /// Returns the current game switches as an array
  /// </summary>
  /// <returns></returns>
  public bool[] ToArray() => data;
  
  /// <summary>
  /// Sets the game switches from an array
  /// </summary>
  /// <param name="newData"></param>
  public void FromArray(bool[] newData) => this.data = newData;
  
}
