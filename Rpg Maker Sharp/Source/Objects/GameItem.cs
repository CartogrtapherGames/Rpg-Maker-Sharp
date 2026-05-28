using RpgSharp.Data;
using RpgSharp.Management;

namespace RpgSharp.Objects;


public enum ItemType
{
  None,
  Armor,
  Weapon,
  Item,
  Skill,
}

/// <summary>
/// The game object class for handling skills, items, weapons, and armor. It is
/// required because save data should not include the database object itself.
/// </summary>
public class GameItem
{
  protected ItemType dataClass;
  protected int itemId;

  public GameItem(IDataGameItem item = null)
  {
    if (item == null) return;
    SetObject(item);
  }
  
  public int ItemId => itemId;

  public IDataGameItem Object()
  {
    return dataClass switch
    {
      ItemType.Skill => DataManager.DataSkills[itemId],
      ItemType.Item => DataManager.DataItems[itemId],
      ItemType.Armor => DataManager.DataArmors[itemId],
      ItemType.Weapon => DataManager.DataWeapons[itemId],
      _ => null
    };
    
  }
  
  public void SetObject(IDataGameItem item)
  {
    dataClass = item switch
    {
      DataSkill => ItemType.Skill,
      DataItem => ItemType.Item,
      DataArmor => ItemType.Armor,
      DataWeapon => ItemType.Weapon,
      _ => ItemType.None
    };
    itemId = item?.Id ?? 0;
  }

  public void SetEquip(bool isWeapon, int id)
  {
    dataClass = isWeapon ? ItemType.Weapon : ItemType.Armor;
    itemId = id;
  }
}
