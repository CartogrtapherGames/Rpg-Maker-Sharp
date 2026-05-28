using System;
using System.Collections.Generic;
using RpgSharp.Data;
using RpgSharp.Management;

namespace RpgSharp.Objects;

public class GameActor : GameBattler
{

  protected int actorId;
  protected string name;
  protected string nickname;
  protected int classId;
  protected int level;
  protected string characterName;
  protected int characterIndex;
  protected string faceName;
  protected int faceIndex;
  protected string battlerName;
  protected Dictionary<int, int> exp;
  protected List<int> skills;
  protected List<GameItem> equips;
  protected int actionInputIndex;
  protected GameItem lastMenuSkill;
  protected GameItem lastBattleSkill;
  protected string lastCommandSymbol;
  protected string profile;
  protected Dictionary<int, int> stateSteps;

  
  public int Level => level;
  
  public GameActor(int id) : base()
  {
    Setup(id);
  }

  public void Setup(int id)
  {
    var actor = DataManager.DataActors[id];
    actorId = id;
    name = actor.Name;
    nickname = actor.Nickname;
    profile = actor.Profile;
    classId = actor.ClassId;
    level = actor.InitialLevel;
    InitImages();
    InitExp();
    InitSkills(); 
    initEquips(actor.Equips);
    ClearParamPlus();
    RecoverAll();
  }
  
  public int ActorId => actorId;
  public DataActor Actor => DataManager.DataActors[actorId];
  public string Name { get => name; set => name = value; }
  public string Nickname { get => nickname; set => nickname = value; }
  public string Profile { get => profile; set => profile = value; }
  public string CharacterName { get => characterName; set => characterName = value; }
  public int CharacterIndex { get => characterIndex; set => characterIndex = value; }
  public string FaceName { get => faceName; set => faceName = value; }
  public int FaceIndex { get => faceIndex; set => faceIndex = value; }
  public string BattlerName { get => battlerName; set => battlerName = value; }

  protected override void ClearStates()
  {
    base.ClearStates();
    stateSteps.Clear();
  }

  public override void EraseState(int stateId)
  {
    base.EraseState(stateId);
    var steps = DataManager.DataStates[stateId].StepsToRemove;
  }

  public override void ResetStateCounts(int stateId)
  {
    base.ResetStateCounts(stateId);
    var steps = DataManager.DataStates[stateId].StepsToRemove;
    stateSteps.Add(stateId, steps);
  }

  protected void InitImages()
  {
    CharacterName = Actor.CharacterName;
    CharacterIndex = Actor.CharacterIndex;
    FaceName = Actor.FaceName;
    FaceIndex = Actor.FaceIndex;
    BattlerName = Actor.BattlerName;
  }

  public int ExpForLevel(int level)
  {
    var c = CurrentClass();
    var basis = c.ExpParams[0];
    var extra = c.ExpParams[1];
    var accA = c.ExpParams[2];
    var accB = c.ExpParams[3];
    return (int)Math.Round(
      (basis * Math.Pow(level - 1, 0.9 + accA / 250.0) * level * (level + 1)) /
      (6 + Math.Pow(level, 2) / 50.0 / accB) +
      (level - 1) * extra
    );
  }

  public void InitExp()
  {
    exp[classId] = CurrentLevelExp();
  }

  public int CurrentExp()
  {
    return exp[classId];
  }

  public int CurrentLevelExp()
  {
    return ExpForLevel(level);
  }

  public int NextLevelExp()
  {
    return ExpForLevel(level + 1);
  }
  public override GameParty FriendsUnit()
  {
    throw new System.NotImplementedException();
  }
  public override GameTroop OpponentsUnit()
  {
    throw new System.NotImplementedException();
  }
}
