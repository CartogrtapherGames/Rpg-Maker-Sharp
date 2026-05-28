#nullable enable
using System.Collections.Generic;
using System.Linq;
using RpgSharp.Data;
using RpgSharp.Management;

namespace RpgSharp.Objects;

public class GameParty : GameUnit<GameActor>, IBattleUnit
{

  protected int gold;
  protected int steps;
  protected GameItem lastItem;
  protected int menuActorId;
  protected int targetActorId;
  protected List<int> actors;

  protected Dictionary<int, int> items;
  protected Dictionary<int, int> weapons;
  protected Dictionary<int, int> armors;


  public GameParty()
  {
    gold = 0;
    steps = 0;
    lastItem = new GameItem();
    menuActorId = 0;
    targetActorId = 0;
    actors = [];
    InitAllItems();
  }

  protected void InitAllItems()
  {
    items = new Dictionary<int, int>();
    weapons = new Dictionary<int, int>();
    armors = new Dictionary<int, int>();
  }

  public bool Exists()
  {
    return actors.Count > 0;
  }
  
  public int Size()
  {
    return actors.Count;
  }

  public bool IsEmpty()
  {
    return actors.Count == 0;
  }
  
  
  public override List<GameActor> Members()
  {
    return InBattle() ? BattleMembers() : AllMembers();
  }

  public List<GameActor> AllMembers()
  {
    return actors.Select(id => DataManager.GameActors.Actor(id)).ToList();
  }

  public List<GameActor> BattleMembers()
  {
    return AllBattleMembers().Where(actor => actor.IsAppeared()).ToList();
  }

  public List<GameActor> AllBattleMembers()
  {
    return AllMembers().Take(MaxBattleMembers()).ToList();
  }

  public int MaxBattleMembers()
  {
    return 4;
  }

  public GameActor Leader()
  {
    return BattleMembers().FirstOrDefault();
  }

  public void RemoveInvalidMembers()
  {
    actors.RemoveAll(actorId => DataManager.DataActors[actorId] == null);
  }

  public void ReviveBattleMembers()
  {
    foreach (var actor in BattleMembers())
    {
      if (actor.IsDead())
      {
        actor.Revive();
      }
    }
  }

  public List<DataItem> Items()
  {
    return items.Keys.Select(id => DataManager.DataItems[id]).ToList();
  }
  
  public List<DataWeapon> Weapons()
  {
    return items.Keys.Select(id => DataManager.DataWeapons[id]).ToList();
  }
  
  public List<DataArmor> Armors()
  {
    return items.Keys.Select(id => DataManager.DataArmors[id]).ToList();
  }
  
  public List<DataEquipable> EquipItems()
  {
    return Weapons().Cast<DataEquipable>()
      .Concat(Armors().Cast<DataEquipable>())
      .ToList();
  }
  
  public List<IDataGameItem> AllItems()
  {
    return Items().Cast<IDataGameItem>()
      .Concat(EquipItems().Cast<IDataGameItem>())
      .ToList();
  }
  
  public Dictionary<int, int>? ItemContainer(IDataGameItem? item)
  {
    if (item == null) return null;
    if (item is DataItem) return items;
    if (item is DataWeapon) return weapons;
    if (item is DataArmor) return armors;
    return null;
  }
  
  public void SetupStartingMembers()
  {
    actors = new List<int>();
    foreach (var actorId in DataManager.DataSystem.PartyMembers)
    {
      if (DataManager.GameActors.Actor(actorId) != null)
        actors.Add(actorId);
    }
  }

  public string Name()
  {
    var numBattleMembers = BattleMembers().Count;
    if(numBattleMembers == 0) return string.Empty;
    if (numBattleMembers == 1) return Leader().Name;
    return TextManager.PartyName.Format(Leader().Name);
  }

  public void SetupBattleTest()
  {
    SetupBattleTestMembers();
    SetupBattleTestItems();
  }

  protected void SetupBattleTestMembers()
  {
    foreach (var battler in DataManager.DataSystem.TestBattlers)
    {
      var actor = DataManager.GameActors.Actor(battler.ActorId);
      if (actor == null) continue;
      actor.ChangeLevel(battler.Level, false);
      actor.InitEquips(battler.Equips);
      actor.RecoverAll();
      AddActor(battler.ActorId);
    }
  }

  protected void SetupBattleTestItems()
  {
    foreach (var item in DataManager.DataItems)
    {
      if (item != null && item.Name.Length > 0)
        GainItem(item, MaxItems(item));
    }
  }

  public int HighestLevel()
  {
    return Members().Max(actor => actor.Level);
  }

  public void AddActor(int actorId)
  {
    
  }
}
