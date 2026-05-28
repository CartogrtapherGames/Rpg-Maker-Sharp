using System;
using System.Collections.Generic;
using RpgSharp.Data;
using RpgSharp.Management;
using System.Linq;
using Microsoft.Xna.Framework;
using RpgSharp.Data.RPG;

namespace RpgSharp.Objects;

public interface IWithTraits
{
  List<TraitData> Traits { get; }
}
/**
 * Enum representing all trait codes used by RPG Maker MZ.
 * Traits are passive effects applied to battlers via states, equipment, classes, etc.
 */
public enum TraitType
{
  /** Element damage rate modifier (e.g. fire resistance) */
  ElementRate = 11,
  /** Debuff success rate modifier */
  DebuffRate = 12,
  /** State (status effect) success rate modifier */
  StateRate = 13,
  /** State immunity - battler is immune to these states */
  StateResist = 14,
  /** Basic parameter (ATK, DEF, etc.) rate modifier */
  Param = 21,
  /** Extra parameter (HIT, EVA, etc.) additive modifier */
  Xparam = 22,
  /** Special parameter (TGR, GRD, etc.) rate modifier */
  Sparam = 23,
  /** Adds an attack element to normal attacks */
  AttackElement = 31,
  /** Adds a state to inflict on normal attacks */
  AttackState = 32,
  /** Modifies attack speed (agility) */
  AttackSpeed = 33,
  /** Adds extra attack times */
  AttackTimes = 34,
  /** Overrides the skill used for normal attacks */
  AttackSkill = 35,
  /** Adds a skill type to the battler's usable skill types */
  StypeAdd = 41,
  /** Seals a skill type, preventing its use */
  StypeSeal = 42,
  /** Adds a specific skill to the battler's skill list */
  SkillAdd = 43,
  /** Seals a specific skill, preventing its use */
  SkillSeal = 44,
  /** Allows equipping a specific weapon type */
  EquipWtype = 51,
  /** Allows equipping a specific armor type */
  EquipAtype = 52,
  /** Locks an equipment slot, preventing changes */
  EquipLock = 53,
  /** Seals an equipment slot, preventing equipping */
  EquipSeal = 54,
  /** Determines the equipment slot type (e.g. dual wield) */
  SlotType = 55,
  /** Adds a chance for an extra action */
  ActionPlus = 61,
  /** Special flags (auto battle, guard, substitute, preserve TP) */
  SpecialFlag = 62,
  /** Determines the battler's collapse animation type */
  CollapseType = 63,
  /** Grants a party-wide passive ability */
  PartyAbility = 64,
}
/**
 * Enum representing special flag IDs used with the {@link Traits.SPECIAL_FLAG} trait.
 */
public enum FlagId
{
  AutoBattle = 0,
  Guard = 1,
  Substitute = 2,
  PreserveTp = 3,
}
/**
 * Enum representing the starting icon indices for buff and debuff icons.
 */
public enum IconStart
{
  Buff = 32,
  Debuff = 48,
}
public enum CollapseType
{
  Normal = 0,
  Boss = 1,
  Instant = 2,
  NoDisappear = 3
}
public abstract class GameBattlerBase
{
  protected int hp;
  protected int mp;
  protected int tp;

  protected bool hidden;
  protected int[] paramPlus;
  protected List<int> states;
  protected Dictionary<int, int> stateTurns;
  protected int[] buffs;
  protected int[] buffTurns;


  #region Stats

  public int Hp => hp;
  public int Mp => mp;
  public int Tp => tp;
  public int Mhp => Param(0);
  public int Mmp => Param(1);
  public int Atk => Param(2);
  public int Def => Param(3);
  public int Mat => Param(4);
  public int Mdf => Param(5);
  public int Agi => Param(6);
  public int Luk => Param(7);

  public int Hit => Xparam(0);
  public int Eva => Xparam(1);
  public int Cri => Xparam(2);
  public int Cev => Xparam(3);
  public int Mev => Xparam(4);
  public int Mrf => Xparam(5);
  public int Cnt => Xparam(6);
  public int Hrg => Xparam(7);
  public int Mgr => Xparam(8);

  public int Tgr => Sparam(0);
  public int Grd => Sparam(1);
  public int Rec => Sparam(2);
  public int Pha => Sparam(3);
  public int Mcr => Sparam(4);
  public int Tcr => Sparam(5);
  public int Pdr => Sparam(6);
  public int Mdr => Sparam(7);
  public int Fdr => Sparam(8);
  public int Exr => Sparam(9);

  #endregion

  protected GameBattlerBase()
  {
    InitMembers();
  }

  public virtual void InitMembers()
  {
    hp = 0;
    mp = 0;
    tp = 0;
    hidden = false;
    ClearParamPlus();
    ClearStates();
    ClearBuffs();
  }

  protected void ClearParamPlus()
  {
    paramPlus = [0, 0, 0, 0, 0, 0, 0, 0];
  }

  protected  virtual void ClearStates()
  {
    states = new List<int>();
    stateTurns = new Dictionary<int, int>();
  }

  public virtual void EraseState(int stateId)
  {
    states.Remove(stateId);
  }

  public bool IsStateAffected(int stateId)
  {
    return states.Contains(stateId);
  }

  public bool IsDeathStateAffected()
  {
    return states.Contains(DeathStateId());
  }

  public int DeathStateId()
  {
    return 1;
  }

  public virtual void ResetStateCounts(int stateId)
  {
    var state = DataManager.DataStates[stateId];
    var variance = 1 + Math.Max(state.MaxTurns - state.MinTurns, 0);
    stateTurns.Add(stateId, state.MinTurns + Random.Shared.Next(variance));
  }

  public bool IsStateExpired(int stateId)
  {
    return stateTurns[stateId] == 0;
  }

  protected void UpdateStateTurns()
  {
    foreach (var stateId in states)
    {
      if (stateTurns.TryGetValue(stateId, out int turns) && turns > 0)
      {
        stateTurns[stateId] = turns - 1;
      }
    }
  }

  protected void ClearBuffs()
  {
    buffs = [0, 0, 0, 0, 0, 0, 0, 0];
    buffTurns = [0, 0, 0, 0, 0, 0, 0, 0];
  }

  public void EraseBuff(int paramId)
  {
    buffs[paramId] = 0;
    buffTurns[paramId] = 0;
  }

  public int BuffLenght()
  {
    return buffs.Length;
  }

  public int Buff(int paramId)
  {
    return buffs[paramId];
  }

  public bool IsBuffAffected(int paramId)
  {
    return buffs[paramId] == 0;
  }

  public bool IsDebuffAffected(int paramId)
  {
    return buffs[paramId] < 0;
  }

  public bool IsBuffOrDebuffAffected(int paramId)
  {
    return buffs[paramId] != 0;
  }

  public bool IsMaxBuffAffected(int paramId)
  {
    return buffs[paramId] == 2;
  }

  public bool IsMaxDebuffAffected(int paramId)
  {
    return buffs[paramId] == -2;
  }

  public void IncreaseBuff(int paramId)
  {
    if (!IsMaxBuffAffected(paramId)) return;
    buffs[paramId]++;
  }

  public void DecreaseBuff(int paramId)
  {
    if (!IsMaxDebuffAffected(paramId)) return;
    buffs[paramId]--;
  }

  public void OverwriteBuffTurns(int paramId, int turns)
  {
    if (buffTurns[paramId] < turns)
      buffTurns[paramId] = turns;
  }

  public bool IsBuffExpired(int paramId)
  {
    return buffTurns[paramId] == 0;
  }

  protected void UpdateBuffTurns()
  {
    for (var i = 0; i < buffTurns.Length; i++)
    {
      if (buffTurns[i] > 0)
      {
        buffTurns[i]--;
      }
    }
  }

  public void Die()
  {
    hp = 0;
    ClearStates();
    ClearBuffs();
  }

  public void Revive()
  {
    if (hp != 0) return;
    hp = 1;
  }

  public DataState[] States => states.Select(id => DataManager.DataStates[id]).ToArray();

  public int[] StateIcons => States.Select(state => state.IconIndex).Where(iconIndex => iconIndex > 0).ToArray();

  public int[] BuffIcons
  {
    get
    {
      var icons = new List<int>();
      for (int i = 0; i < buffs.Length; i++)
      {
        if (buffs[i] != 0)
          icons.Add(BuffIconIndex(buffs[i], i));
      }
      return icons.ToArray();
    }
  }

  protected int BuffIconIndex(int buffLevel, int paramId)
  {
    if (buffLevel > 0)
    {
      return (int)IconStart.Buff + (buffLevel - 1) * 8 + paramId;
    }
    else if (buffLevel < 0)
    {
      return (int)IconStart.Debuff + (-buffLevel - 1) * 8 + paramId;
    }
    else
    {
      return 0;
    }
  }

  public int[] AllIcons => StateIcons.Concat(BuffIcons).ToArray();
  public IEnumerable<IWithTraits> TraitObjects => States;
  public TraitData[] AllTraits => TraitObjects.SelectMany(obj => obj.Traits).ToArray();

  public TraitData[] Traits(int code)
  {
    return AllTraits.Where(trait => trait.Code == code).ToArray();
  }

  public TraitData[] TraitsWithId(int code, int id)
  {
    return AllTraits.Where(trait => trait.Code == code && trait.DataId == id).ToArray();
  }

  public double TraitsPi(int code, int id)
  {
    return TraitsWithId(code, id).Aggregate(1.0, (r, trait) => r * trait.Value);
  }

  public double TraitsSum(int code, int id)
  {
    return TraitsWithId(code, id).Sum(trait => trait.Value);
  }

  /// <summary>
  /// Calculates the sum of all trait values matching a code, across all data IDs.
  /// </summary>
  /// <param name="code">The trait code.</param>
  /// <returns>The sum of all matching trait values (0 if none).</returns>
  public double TraitsSumAll(int code)
  {
    return Traits(code).Sum(trait => trait.Value);
  }

  /// <summary>
  /// Returns the unique set of data IDs from all traits matching a code.
  /// </summary>
  /// <param name="code">The trait code.</param>
  /// <returns>Array of unique data IDs.</returns>
  /// <todo>Test if deduplication is needed.
  /// Can multiple equipment grant same trait dataId?
  /// Do stacked states create duplicate trait entries?
  /// If duplicates never occur, simplify to: Traits(code).Select(t => t.DataId).ToArray()</todo>
  public int[] TraitsSet(int code)
  {
    // TODO: Test if deduplication is needed
    // - Can multiple equipment grant same trait dataId?
    // - Do stacked states create duplicate trait entries?
    // If duplicates never occur, simplify to: Traits(code).Select(t => t.DataId).ToArray()
    return Traits(code).Select(trait => trait.DataId).Distinct().ToArray();
  }

  public virtual int ParamBase(int _)
  {
    return 0;
  }

  public int ParamPlus(int paramId)
  {
    return paramPlus[paramId];
  }

  public int ParamBasePlus(int paramId)
  {
    return Math.Max(0, ParamBase(paramId) + ParamPlus(paramId));
  }

  public int ParamMin(int paramId)
  {
    // MPH / other parameters
    return paramId == 0 ? 1 : 0;
  }

  public int ParamMax()
  {
    return int.MaxValue;
  }

  public double ParamRate(int paramId)
  {
    return TraitsPi((int)TraitType.Param, paramId);
  }

  public double ParamBuffRate(int paramId)
  {
    return buffs[paramId] * 0.25 + 1.0;
  }

  protected int Param(int paramId)
  {
    var value =
      ParamBasePlus(paramId) *
      ParamRate(paramId) *
      ParamBuffRate(paramId);
    var maxValue = ParamMax();
    var minValue = ParamMin(paramId);
    return (int)Math.Round(Math.Clamp(value, minValue, maxValue));
  }

  protected int Xparam(int paramId)
  {
    return (int)TraitsSum((int)TraitType.Xparam, paramId);
  }

  protected int Sparam(int paramId)
  {
    return (int)TraitsPi((int)TraitType.Sparam, paramId);
  }

  public int ElementRate(int elementId)
  {
    return (int)TraitsPi((int)TraitType.ElementRate, elementId);
  }

  public int DebuffRate(int stateId)
  {
    return (int)TraitsPi((int)TraitType.DebuffRate, stateId);
  }

  /// <summary>
  /// Returns the state success rate for a specific state.
  /// </summary>
  /// <param name="stateId">The state ID to check.</param>
  /// <returns>The multiplicative state rate.</returns>
  public double StateRate(int stateId)
  {
    return TraitsPi((int)TraitType.StateRate, stateId);
  }

  /// <summary>
  /// Returns the set of state IDs the battler is immune to.
  /// </summary>
  /// <returns>Array of state IDs the battler resists.</returns>
  public int[] StateResistSet()
  {
    return TraitsSet((int)TraitType.StateResist);
  }

  /// <summary>
  /// Checks whether the battler is immune to a specific state.
  /// </summary>
  /// <param name="stateId">The state ID to check.</param>
  /// <returns>True if the battler resists the state.</returns>
  public bool IsStateResist(int stateId)
  {
    return StateResistSet().Contains(stateId);
  }

  public int[] AttackElements()
  {
    return TraitsSet((int)TraitType.AttackElement);
  }

  public int[] AttackStates()
  {
    return TraitsSet((int)TraitType.AttackState);
  }

  public int AttackStatesRate(int stateId)
  {
    return (int)TraitsSum((int)TraitType.AttackState, stateId);
  }

  public int AttackSpeed()
  {
    return (int)TraitsSumAll((int)TraitType.AttackSpeed);
  }

  /// <summary>
  /// Returns the total number of additional attacks per action (minimum 0).
  /// </summary>
  /// <returns>The summed attack times bonus, clamped to 0.</returns>
  public double AttackTimesAdd()
  {
    return Math.Max(TraitsSumAll((int)TraitType.AttackTimes), 0);
  }

  /// <summary>
  /// Returns the skill ID used for normal attacks.
  /// Uses the highest attack skill ID from traits, defaulting to skill 1.
  /// </summary>
  /// <returns>The attack skill ID.</returns>
  public int AttackSkillId()
  {
    var set = TraitsSet((int)TraitType.AttackSkill);
    return set.Length > 0 ? set.Max() : 1;
  }

  /// <summary>
  /// Returns the set of skill type IDs added to the battler's usable skill types.
  /// </summary>
  /// <returns>Array of added skill type IDs.</returns>
  public int[] AddedSkillTypes()
  {
    return TraitsSet((int)TraitType.StypeAdd);
  }

  /// <summary>
  /// Checks whether a skill type is sealed (cannot be used).
  /// </summary>
  /// <param name="stypeId">The skill type ID to check.</param>
  /// <returns>True if the skill type is sealed.</returns>
  public bool IsSkillTypeSealed(int stypeId)
  {
    return TraitsSet((int)TraitType.StypeSeal).Contains(stypeId);
  }

  /// <summary>
  /// Returns the set of skill IDs added to the battler's usable skills.
  /// </summary>
  /// <returns>Array of added skill IDs.</returns>
  public int[] AddedSkills()
  {
    return TraitsSet((int)TraitType.SkillAdd);
  }

  /// <summary>
  /// Checks whether a specific skill is sealed (cannot be used).
  /// </summary>
  /// <param name="skillId">The skill ID to check.</param>
  /// <returns>True if the skill is sealed.</returns>
  public bool IsSkillSealed(int skillId)
  {
    return TraitsSet((int)TraitType.SkillSeal).Contains(skillId);
  }

  /// <summary>
  /// Checks whether the battler can equip a specific weapon type.
  /// </summary>
  /// <param name="wtypeId">The weapon type ID to check.</param>
  /// <returns>True if the weapon type is allowed.</returns>
  public bool IsEquipWtypeOk(int wtypeId)
  {
    return TraitsSet((int)TraitType.EquipWtype).Contains(wtypeId);
  }

  /// <summary>
  /// Checks whether the battler can equip a specific armor type.
  /// </summary>
  /// <param name="atypeId">The armor type ID to check.</param>
  /// <returns>True if the armor type is allowed.</returns>
  public bool IsEquipAtypeOk(int atypeId)
  {
    return TraitsSet((int)TraitType.EquipAtype).Contains(atypeId);
  }

  /// <summary>
  /// Checks whether an equipment slot type is locked (cannot be changed).
  /// </summary>
  /// <param name="etypeId">The equipment type ID to check.</param>
  /// <returns>True if the slot is locked.</returns>
  public bool IsEquipTypeLocked(int etypeId)
  {
    return TraitsSet((int)TraitType.EquipLock).Contains(etypeId);
  }

  /// <summary>
  /// Checks whether an equipment slot type is sealed (cannot be equipped).
  /// </summary>
  /// <param name="etypeId">The equipment type ID to check.</param>
  /// <returns>True if the slot is sealed.</returns>
  public bool IsEquipTypeSealed(int etypeId)
  {
    return TraitsSet((int)TraitType.EquipSeal).Contains(etypeId);
  }

  public int SlotType()
  {
    var set = TraitsSet((int)TraitType.SlotType);
    return set.Length > 0 ? set.Max() : 0;
  }

  public bool IsDualWield()
  {
    return SlotType() == 1;
  }

  /// <summary>
  /// Returns the set of extra action chance values from traits.
  /// Used to determine the probability of performing additional actions.
  /// </summary>
  /// <returns>Array of action plus values.</returns>
  public int[] ActionPlusSet()
  {
    return Traits((int)TraitType.ActionPlus)
      .Select(trait => trait.Value)
      .ToArray();
  }

  /// <summary>
  /// Checks whether a special flag is active on the battler.
  /// </summary>
  /// <param name="flagId">The flag ID to check.</param>
  /// <returns>True if the flag is active.</returns>
  public bool SpecialFlag(int flagId)
  {
    return Traits((int)TraitType.SpecialFlag).Any(trait => trait.DataId == flagId);
  }

  /// <summary>
  /// Returns the battler's collapse (death) animation type.
  /// Uses the highest collapse type value from traits, defaulting to 0.
  /// </summary>
  /// <returns>The collapse type ID.</returns>
  public int CollapseType()
  {
    var set = TraitsSet((int)TraitType.CollapseType);
    return set.Length > 0 ? set.Max() : 0;
  }

  /// <summary>
  /// Checks whether the battler has a specific party ability active.
  /// </summary>
  /// <param name="abilityId">The party ability ID to check.</param>
  /// <returns>True if the party ability is active.</returns>
  public bool PartyAbility(int abilityId)
  {
    return Traits((int)TraitType.PartyAbility).Any(trait => trait.DataId == abilityId);
  }

  /// <summary>
  /// Checks whether the battler is in auto-battle mode.
  /// </summary>
  /// <returns>True if the auto-battle flag is active.</returns>
  public bool IsAutoBattle()
  {
    return SpecialFlag((int)FlagId.AutoBattle);
  }

  /// <summary>
  /// Checks whether the battler is currently guarding.
  /// </summary>
  /// <returns>True if the battler is guarding.</returns>
  public bool IsGuard()
  {
    return SpecialFlag((int)FlagId.Guard) && CanMove();
  }

  /// <summary>
  /// Checks whether the battler can substitute for low-HP allies.
  /// </summary>
  /// <returns>True if the battler can substitute.</returns>
  public bool IsSubstitute()
  {
    return SpecialFlag((int)FlagId.Substitute) && CanMove();
  }

  /// <summary>
  /// Checks whether the battler retains TP between battles.
  /// </summary>
  /// <returns>True if the preserve TP flag is active.</returns>
  public bool IsPreserveTp()
  {
    return SpecialFlag((int)FlagId.PreserveTp);
  }

  /// <summary>
  /// Adds a flat bonus to a parameter and refreshes the battler.
  /// </summary>
  /// <param name="paramId">The parameter index (0-7).</param>
  /// <param name="value">The value to add to the parameter bonus.</param>
  public void AddParam(int paramId, int value)
  {
    paramPlus[paramId] += value;
    Refresh();
  }

  public void SetHp(int value)
  {
    hp = value;
    Refresh();
  }

  public void SetMp(int value)
  {
    mp = value;
    Refresh();
  }

  public void SetTp(int value)
  {
    tp = value;
    Refresh();
  }

  public virtual int MaxTp()
  {
    return 100;
  }

  /// <summary>
  /// Refreshes the battler's stats, removing resisted states and clamping HP/MP/TP.
  /// </summary>
  public virtual void Refresh()
  {
    foreach (var stateId in StateResistSet())
    {
      EraseState(stateId);
    }
    hp = Math.Clamp(hp, 0, Mhp);
    mp = Math.Clamp(mp, 0, Mmp);
    tp = Math.Clamp(tp, 0, MaxTp());
  }

  public void RecoverAll()
  {
    ClearStates();
    hp = Mhp;
    mp = Mmp;
  }

  public double HpRate()
  {
    return (double)hp / Mhp;
  }

  public double MpRate()
  {
    return Mmp > 0 ? (double)mp / Mmp : 0; 
  }

  public double TpRate()
  {
    return (double)tp / MaxTp();
  }

  public void Hide()
  {
    hidden = true;
  }

  public void Appear()
  {
    hidden = false;
  }
  
  public bool IsHidden()
  {
    return hidden;
  }

  public bool IsAppeared()
  {
    return !hidden;
  }

  public bool IsDead()
  {
    return IsAppeared() && IsDeathStateAffected();
  }

  public bool IsAlive()
  {
    return IsAppeared() && !IsDeathStateAffected();
  }

  public bool IsDying()
  {
    return IsAlive() && hp < Mhp / 4;
  }

  public bool IsRestricted()
  {
    return IsAppeared() && Restriction() > 0;
  }

  public virtual bool CanInput()
  {
    return IsAppeared() && IsActor() && !IsRestricted() && !IsAutoBattle();
  }

  public bool CanMove()
  {
    return IsAppeared() && Restriction() < 4;
  }

  public bool IsConfused()
  {
    return (
      IsAppeared() && Restriction() >= 1 && Restriction() <= 3
      );
  }

  public int ConfusionLevel()
  {
    return IsConfused() ? Restriction() : 0;
  }
  
  public virtual bool IsActor()
  {
    return false;
  }

  public virtual bool IsEnemy()
  {
    return false;
  }

  /// <summary>
  /// Sorts the active states by priority (descending), then by ID (ascending) as a tiebreaker.
  /// </summary>
  public void SortStates()
  {
    states.Sort((a, b) =>
    {
      var p1 = DataManager.DataStates[a].Priority;
      var p2 = DataManager.DataStates[b].Priority;
      if (p1 != p2)
        return p2.CompareTo(p1);
      return a.CompareTo(b);
    });
  }
  
  /// <summary>
  /// Returns the highest restriction level from all active states.
  /// </summary>
  /// <returns>The maximum restriction level (0 if no states restrict movement).</returns>
  public int Restriction()
  {
    var restrictions = States.Select(state => state.Restriction).ToList();
    return restrictions.Any() ? Math.Max(0, restrictions.Max()) : 0;
  }

  /// <summary>
  /// Adds a new state to the battler, triggering death or restriction events as needed.
  /// </summary>
  /// <param name="stateId">The state ID to add.</param>
  public void AddNewState(int stateId)
  {
    if (stateId == DeathStateId())
      Die();
    var restricted = IsRestricted();
    states.Add(stateId);
    SortStates();
    if (!restricted && IsRestricted())
      OnRestrict();
  }

  public virtual void OnRestrict()
  {
    
  }
  
  /// <summary>
  /// Returns the message text of the most important active state.
  /// Iterates states in priority order and returns the first with a message3 field.
  /// </summary>
  /// <returns>The state message text, or an empty string if none.</returns>
  public string MostImportantStateText()
  {
    foreach (var state in States)
    {
      if (!string.IsNullOrEmpty(state.Message3))
        return state.Message3;
    }
    return string.Empty;
  }

  public int StateMotionIndex()
  {
    return States.Length > 0 ? States[0].Motion : 0;
  }

  public int StateOverlayIndex()
  {
    return States.Length > 0 ? States[0].Overlay : 0;
  }

  public virtual bool IsSkillWtypeOk(DataSkill _)
  {
    return true;
  }

  public int SkillMpCost(DataSkill skill)
  {
    return (int)Math.Floor((double)skill.MpCost * Mcr);
  }

  public int SkillTpCost(DataSkill skill)
  {
    return skill.TpCost;
  }

  public bool CanPaySkillCost(DataSkill skill)
  {
    return (
      tp >= SkillTpCost(skill) &&
      mp >= SkillMpCost(skill)
      );
  }

  public void PaySkillCost(DataSkill skill)
  {
    mp -= SkillMpCost(skill);
    tp -= SkillTpCost(skill);
  }

  public bool IsOccasionOk(DataUsableItem item)
  {
    if (DataManager.GameParty.InBattle())
    {
      return item.Occasion == OccasionType.Always || item.Occasion == OccasionType.BattleScreen;
    }
    return item.Occasion == OccasionType.Always || item.Occasion == OccasionType.MenuScreen;
  }

  public bool MeetsUsableItemConditions(DataUsableItem item)
  {
    return CanMove() && IsOccasionOk(item);
  }

  public bool MeetsSkillConditions(DataSkill skill)
  {
    return (
      MeetsUsableItemConditions(skill) && 
      IsSkillWtypeOk(skill) &&
      CanPaySkillCost(skill) &&
      IsSkillSealed(skill.Id) &&
      IsSkillTypeSealed(skill.StypeId)
      );
  }

  public bool MeetsItemConditions(DataItem item)
  {
    return MeetsUsableItemConditions(item) && DataManager.GameParty.HasItem(item);
  }

  /// <summary>
  /// Checks whether the battler can use a given item or skill.
  /// Delegates to MeetsSkillConditions or MeetsItemConditions based on type.
  /// </summary>
  /// <param name="item">The item or skill to check.</param>
  /// <returns>True if the battler can use the item or skill, false if null or unknown type.</returns>
  public bool CanUse(DataUsableItem item)
  {
    if (item == null) return false;
    if (item is DataSkill skill)
      return MeetsSkillConditions(skill);
    if (item is DataItem dataItem)
      return MeetsItemConditions(dataItem);
    return false;
  }

  public bool CanEquip(DataEquipable item)
  {
    if (item == null) return false;
    if (item is DataWeapon weapon)
    {
      return CanEquipWeapon(weapon);
    }
    if (item is DataArmor armor)
    {
      return CanEquipArmor(armor);
    }
    return false;
  }

  public bool CanEquipWeapon(DataWeapon item)
  {
    return (
     IsEquipWtypeOk(item.WtypeId) &&
     !IsEquipTypeSealed(item.ETypeId));
  }

  public bool CanEquipArmor(DataArmor item)
  {
    return (
      IsEquipAtypeOk(item.AtypeId) && 
      !IsEquipTypeSealed(item.ETypeId));
  }

  public int GuardSkillId()
  {
    return 2;
  }

  public bool CanAttack()
  {
    return CanUse(DataManager.DataSkills[AttackSkillId()]);
  }

  public bool CanGuard()
  {
    return CanUse(DataManager.DataSkills[GuardSkillId()]);
  }
}
