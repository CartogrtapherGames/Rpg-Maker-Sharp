using System.Collections.Generic;
using RpgSharp.Management;

namespace RpgSharp.Objects;

/// <summary>
/// The class that manages the game self switches
/// </summary>
public class GameSelfSwitches
{
  Dictionary<string, bool> data;

  public GameSelfSwitches()
  {
    Clear();
  }
  
  public void Clear() => data = new Dictionary<string, bool>();
  
  public bool this[string key]
  {
    get => data.TryGetValue(key, out var val) && val;
    set
    {
      if (value)
        data[key] = true;
      else
        data.Remove(key); // mirrors MZ's delete behavior
      OnChange();
    }
  }

  public bool this[int mapId, int eventId, string sw]
  {
    get => this[$"{mapId},{eventId},{sw}"];
    set => this[$"{mapId},{eventId},{sw}"] = value;
  }

  void OnChange()
  {
    DataManager.GameMap?.RequestRefresh();
  }

  public Dictionary<string, bool> ToDictionary() => data;
  public void FromDictionary(Dictionary<string, bool> newData) => data = newData;
}
