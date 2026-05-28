using System;
using RpgSharp.Management;

namespace RpgSharp.Objects;

public class GameVariables
{
  object[] data;

  public GameVariables()
  {
    Clear();
  }

  public void Clear()
  {
    data = new object[DataManager.DataSystem.Variables.Length];
  }
  
  public object this[int id]
  {
    get => id > 0 && id < data.Length ? data[id] ?? 0 : 0;
    set
    {
      if (id > 0 && id < data.Length)
      {
        // floor it if its a number, leave it alone if its a string
        data[id] = value is double or float
          ? (int)Math.Floor(Convert.ToDouble(value))
          : value;
        OnChange();
      }
    }
  }

  public int    GetInt   (int id) => Convert.ToInt32(data[id] ?? 0);
  public string GetString(int id) => data[id]?.ToString() ?? "";

   void OnChange()
  {
    DataManager.GameMap?.RequestRefresh();
  }

  public object[] ToArray() => data;
  public void FromArray(object[] newData) => data = newData;
}
