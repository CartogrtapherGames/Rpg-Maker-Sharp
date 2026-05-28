using System;
using System.Collections.Generic;
using System.Linq;
using RpgSharp.Data;
using RpgSharp.Management;

namespace RpgSharp.Objects;

public interface IBattleUnit
{
}
public enum EffectType
{
  None,
  Whiten,
  Blink,
  Collapse,
  BossCollapse,
  InstantCollapse
}
public enum MotionType
{
  None,
  Guard,
  Spell,
  Skill,
  Item,
  Thrust,
  Swing,
  Missile,
  Damage,
  Evade,
  Victory,
  Escape
}
public enum TpbState
{
  None,
  Charging,
  Casting,
  Acting,
  Ready,
  Charged
}
public enum ActionState
{
  None,
  Undecided,
  Inputting,
  Waiting,
  Acting,
  Done
}
public abstract class GameBattler : GameBattlerBase
{
  protected List<GameAction> actions;
  protected double speed;
  protected GameActionResult result;
  protected ActionState actionState;
  protected int lastTargetIndex;
  protected bool damagePopup;
  protected bool motionRefresh;
  protected bool selected;
  protected TpbState tpbState;
  protected double tpbChargeTime;
  protected double tpbCastTime;
  protected double tpbIdleTime;
  protected int tpbTurnCount;
  protected bool tpbTurnEnd;


  public override void InitMembers()
  {
    base.InitMembers();
    actions = [];
    speed = 0;
    result = new GameActionResult();
    actionState = ActionState.None;
    lastTargetIndex = 0;
    damagePopup = false;
    EffectType = EffectType.None;
    MotionType = MotionType.None;
    WeaponImageId = 0;
    motionRefresh = false;
    selected = false;
    tpbState = TpbState.None;
    tpbChargeTime = 0;
    tpbCastTime = 0;
    tpbIdleTime = 0;
    tpbTurnCount = 0;
    tpbTurnEnd = false;
  }

  public abstract IBattleUnit FriendsUnit();
  public abstract IBattleUnit OpponentsUnit();

  public void ClearDamagePopup() => damagePopup = false;

  public void ClearWeaponAnimation() => WeaponImageId = 0;
  public void ClearEffect() => EffectType = EffectType.None;
  public void ClearMotion()
  {
    MotionType = MotionType.None;
    motionRefresh = false;
  }
  public void RequestEffect(EffectType effectType) => EffectType = effectType;
  public void RequestMotion(MotionType motionType) => MotionType = motionType;
  public void RequestMotionRefresh() => motionRefresh = true;
  public void CancelMotionRefresh() => motionRefresh = false;
  public void Select() => selected = true;
  public void Deselect() => selected = false;

  public bool IsDamagePopupRequested() => damagePopup;
  public bool IsEffectRequested() => EffectType != EffectType.None;
  public bool IsMotionRequested() => MotionType != MotionType.None;
  public bool IsWeaponAnimationRequested() => WeaponImageId > 0;
  public bool IsMotionRefreshRequested() => motionRefresh;
  public bool IsSelected() => selected;

  public EffectType EffectType { get; protected set; }

  public MotionType MotionType { get; protected set; }

  public int WeaponImageId { get; protected set; }

  public void StartDamagePopup() => damagePopup = true;
  
  public bool ShouldPopupDamage()
  {
    return (
      result.missed ||
      result.evaded ||
      result.hpAffected ||
      result.mpDamage != 0
      );
  }

  public void StartWeaponAnimation(int weaponImageId)
  {
    WeaponImageId = weaponImageId;
  }
  
  // TODO : maybe later set this more like an array accessor??
  public GameAction Action(int index) => actions[index];
  public void SetAction(int index, GameAction action) => actions[index] = action;
  
  public int ActionCount => actions.Count;
  
  public void ClearActions() => actions.Clear();
  
  public GameActionResult Result => result;

  public void ClearResult() => result.Clear();

  public void ClearTpbChargeTime()
  {
    tpbState = TpbState.Charging;
    tpbChargeTime = 0;
  }

  public void ApplyTpbChargeTime()
  {
    tpbState = TpbState.Charging;
    tpbChargeTime -= 1;
  }

  public void InitTpbChargeTime(bool advantageous)
  {
    var speed = TpbRelativeSpeed();
    tpbState = TpbState.Charging;
    tpbChargeTime = advantageous ? 1 : speed * Random.Shared.NextDouble() * 0.5;
    if (IsRestricted()) tpbChargeTime = 0;
  }
  
  public double TpbChargeTime => tpbChargeTime;

  public void StartTpbCasting()
  {
    tpbState = TpbState.Casting;
    tpbCastTime = 0;
  }

  public void StartTpbAction()
  {
    tpbState = TpbState.Acting;
  }

  public bool IsTpbCharged()
  {
    return tpbState == TpbState.Charged;
  }

  public bool IsTpbReady()
  {
    return tpbState == TpbState.Ready;
  }

  public bool IsTpbTimeout()
  {
    return tpbIdleTime >= 1;
  }

  public void UpdateTpb()
  {
    if (!CanMove()) return;
    UpdateTpbChargeTime();
    UpdateTpbCastTime();
    UpdateTpbAutoBattle();
    if (IsAlive()) UpdateTpbIdleTime();
  }

  protected void UpdateTpbChargeTime()
  {
    if (tpbState != TpbState.Charging) return;
    tpbChargeTime += TpbAcceleration();
    if (tpbChargeTime >= 1)
    {
      tpbChargeTime = 1;
      OnTpbCharged();
    }
  }

  protected void UpdateTpbCastTime()
  {
    if (tpbState != TpbState.Casting) return;
    tpbCastTime += TpbAcceleration();
    if (tpbCastTime >= TpbRequiredCastTime())
    {
      tpbCastTime = TpbRequiredCastTime();
      tpbState = TpbState.Ready;
    }
  }

  protected void UpdateTpbAutoBattle()
  {
    if (IsTpbCharged() && IsTpbTurnEnd() && IsAutoBattle())
    {
      MakeTpbActions();
    }
  }

  protected void UpdateTpbIdleTime()
  {
    if (!CanMove() || IsTpbCharged())
    {
      tpbIdleTime += TpbAcceleration();
    }
  }

  protected double TpbAcceleration()
  {
    var _speed = TpbRelativeSpeed();
    var referenceTime = DataManager.GameParty.TpbReferenceTime();
    return _speed / referenceTime;
  }

  public double TpbRelativeSpeed()
  {
    return TpbSpeed() / DataManager.GameParty.TpbBaseSpeed();
  }

  public double TpbSpeed()
  {
    return Math.Sqrt(Agi) + 1;
  }

  public double TpbBaseSpeed()
  {
    var baseAgility = ParamBasePlus(6);
    return Math.Sqrt(baseAgility) + 1;
  }

  public double TpbRequiredCastTime()
  {
    var act = actions.Where(action => action.IsValid());
    var items = actions.Select(action => action.Item());
    var delay = items.Agregate((r, item) => r + Math.Max(0, -item.Speed), 0);
    return Math.Sqrt(delay) / TpbSpeed();
  }
  
  public void OnTpbCharged()
  {
    if (!ShouldDelayTpbCharge())
    {
      FinishTpbCharge();
    }
  }

  public bool ShouldDelayTpbCharge()
  {
    return !BattleManager.IsActiveTpb() && DataManager.GameParty.CanInput();
  }

  public void FinishTpbCharge()
  {
    tpbState = TpbState.Charged;
    tpbTurnEnd = true;
    tpbIdleTime = 0;
  }

  public bool IsTpbTurnEnd()
  {
    return tpbTurnEnd;
  }

  public void InitTpbTurn()
  {
    tpbTurnEnd = false;
    tpbTurnCount = 0;
    tpbIdleTime = 0;
  }

  public void StartTpbTurn()
  {
    tpbTurnEnd = false;
    tpbTurnCount++;
    tpbIdleTime = 0;

    if (ActionCount == 0)
    {
      MakeTpbActions();
    }
  }

  public void MakeTpbActions()
  {
    MakeActions();

    if (CanInput())
    {
      SetActionState(ActionState.Undecided);
    }
    else
    {
      StartTpbCasting();
      SetActionState(ActionState.Waiting);
    }
  }
  
  public void OnTpbTimeout()
  {
    OnAllActionsEnd();
    tpbTurnEnd = true;
    tpbIdleTime = 0;
  }

  public int TurnCount()
  {
    if (BattleManager.IsTpb())
    {
      return tpbTurnCount; 
    }
    else
    {
      return DataManager.GameTroop.TurnCount() + 1;
    }
  }

  public override bool CanInput()
  {
    if (BattleManager.IsTpb() && !IsTpbCharged())
    {
      return false;
    }

    return base.CanInput();
  }

  public override void Refresh()
  {
    base.Refresh();

    if (Hp == 0)
    {
      AddState(DeathStateId());
    }
    else
    {
      RemoveState(DeathStateId());
    }
  }

  public void AddState(int stateId)
  {
    if (IsStateAddable(stateId))
    {
      if (!IsStateAffected(stateId))
      {
        AddNewState(stateId);
        Refresh();
      }

      ResetStateCounts(stateId);
      result.PushAddedState(stateId);
    }
  }

  public bool IsStateAddable(int stateId)
  {
    return (
      IsAlive() &&
      DataManager.DataStates[stateId] is not null &&
      !IsStateResist(stateId) 
      && !IsStateRestrict(stateId)
      );
  }

  public bool IsStateRestrict(int stateId)
  {
    return DataManager.DataStates[stateId].RemoveByRestriction && IsRestricted();
  }

  public override void OnRestrict()
  {
    base.OnRestrict();
    ClearTpbChargeTime();
    ClearActions();
    foreach (var state in States)
    {
      if (state.RemoveByRestriction)
      {
        RemoveState(state.Id);
      }
    }
  }

  public void RemoveState(int stateId)
  {
    if (IsStateAffected(stateId))
    {
      if (stateId == DeathStateId())
      {
        Revive();
      }
    }
    EraseState(stateId);
    Refresh();
    result.PushRemovedState(stateId);
  }

  public void Escape()
  {
    if (DataManager.GameParty.InBattle()) Hide();
    
    ClearActions();
    ClearStates();
    SoundManager.PlayEscape();
  }

  public void AddBuff(int paramId, int turns)
  {
    if (!IsAlive()) return;
    IncreaseBuff(paramId);
    if(IsBuffAffected(paramId)) OverwriteBuffTurns(paramId, turns);
    result.PushAddedBuff(paramId);
    Refresh();
  }

  public void AddDebuff(int paramId, int turns)
  {
    if (!IsAlive()) return;
    DecreaseBuff(paramId);
    if(IsDebuffAffected(paramId)) OverwriteBuffTurns(paramId, turns);
    result.PushAddedDebuffTurns(paramId, turns);
    Refresh();
  }

  public void RemoveBuff(int paramId)
  {
    if (!IsAlive() || !IsBuffOrDebuffAffected(paramId)) return;
    EraseBuff(paramId);
    result.PushRemovedBuff(paramId);
    Refresh();
  }

  public void RemoveBattleStates()
  {
    foreach (var state in States)
    {
      if (!state.RemoveAtBattleEnd) continue;
      RemoveState(state.Id);
    }
  }

  public void RemoveAllBuffs()
  {
    for (var i = 0; i < BuffLenght(); i++)
    {
      RemoveBuff(i);
    }
  }

  public void RemoveStatesAuto(int timing)
  {
    foreach (var state in States)
    {
      if (IsStateExpired(state.Id) && state.AutoRemovalTiming == timing)
        RemoveState(state.Id);
    }
  }

  public void RemoveBuffsAuto()
  {
    for (int i = 0; i < BuffLenght(); i++)
    {
      if (IsBuffExpired(i))
        RemoveBuff(i);
    }
  }

  public void RemoveStatesByDamage()
  {
    foreach (var state in States)
    {
      if (state.RemoveByDamage && Random.Shared.Next(100) < state.ChanceByDamage)
        RemoveState(state.Id);
    }
  }

  public int MakeActionTimes()
  {
    return ActionPlusSet().Aggregate(1, (r, p) => Random.Shared.NextDouble() < p ? r + 1 : r);
  }
  
  public void MakeActions()
  {
    ClearActions();
    if (CanMove())
    {
      var actionTimes = MakeActionTimes();
      actions = new List<GameAction>();
      for (int i = 0; i < actionTimes; i++)
        actions.Add(new GameAction(this));
    }
  }

  public double Speed() => speed;

  public void MakeSpeed()
  {
    speed = actions.Count > 0 ? actions.Min(action => action.Speed()) : 0;
  }

  public GameAction CurrentAction() => actions.FirstOrDefault();

  public void RemoveCurrentAction() => actions.RemoveAt(0);

  public void SetLastTarget(GameBattler target)
  {
    lastTargetIndex = target is GameEnemy enemy ? enemy.Index() : 0;
  }

  public void ForceAction(int skillId, int targetIndex)
  {
    ClearActions();
    var action = new GameAction(this, true);
    action.SetSkill(skillId);
    if (targetIndex == -2)
      action.SetTarget(lastTargetIndex);
    else if (targetIndex == -1)
      action.DecideRandomTarget();
    else
      action.SetTarget(targetIndex);
    if (action.Item() != null)
      actions.Add(action);
  }
  
  public void UseItem(DataUsableItem item)
  {
    if (item is DataSkill skill)
      PaySkillCost(skill);
    else if (item is DataItem dataItem)
      ConsumeItem(dataItem);
  }

  public void ConsumeItem(DataItem item)
  {
    DataManager.GameParty.ConsumeItem(item);
  }

  public void GainHp(int value)
  {
    result.HpDamage = -value;
    result.HpAffected = true;
    SetHp(Hp + value);
  }

  public void GainMp(int value)
  {
    result.MpDamage = -value;
    SetMp(Mp + value);
  }

  public void GainTp(int value)
  {
    result.TpDamage = -value;
    SetTp(Tp + value);
  }

  public void GainSilentTp(int value)
  {
    SetTp(Tp + value);
  }

  public void InitTp()
  {
    SetTp(Random.Shared.Next(25));
  }

  public void ClearTp()
  {
    SetTp(0);
  }

  public void ChargeTpByDamage(double damageRate)
  {
    var value = (int)Math.Floor(50 * damageRate * Tcr);
    GainSilentTp(value);
  }

  public void RegenerateHp()
  {
    var minRecover = -MaxSlipDamage();
    var value = Math.Max((int)Math.Floor((double)Mhp * Hrg), minRecover);
    if (value != 0)
      GainHp(value);
  }

  public int MaxSlipDamage()
  {
    return DataManager.DataSystem.OptSlipDeath ? Hp : Math.Max(Hp - 1, 0);
  }

  public void RegenerateMp()
  {
    var value = (int)Math.Floor((double)Mmp * Mgr);
    if (value != 0)
      GainMp(value);
  }

  public void RegenerateTp()
  {
    var value = (int)Math.Floor(100 * (double)Tgr);
    GainSilentTp(value);
  }

  public void RegenerateAll()
  {
    if (IsAlive())
    {
      RegenerateHp();
      RegenerateMp();
      RegenerateTp();
    }
  }
  
  public void OnBattleStart(bool advantageous = false)
{
    SetActionState(ActionState.Undecided);
    ClearMotion();
    InitTpbChargeTime(advantageous);
    InitTpbTurn();
    if (!IsPreserveTp())
        InitTp();
}

public void OnAllActionsEnd()
{
    ClearResult();
    RemoveStatesAuto(1);
    RemoveBuffsAuto();
}

public void OnTurnEnd()
{
    ClearResult();
    RegenerateAll();
    UpdateStateTurns();
    UpdateBuffTurns();
    RemoveStatesAuto(2);
}

public void OnBattleEnd()
{
    ClearResult();
    RemoveBattleStates();
    RemoveAllBuffs();
    ClearActions();
    if (!IsPreserveTp())
        ClearTp();
    Appear();
}

public void OnDamage(int value)
{
    RemoveStatesByDamage();
    ChargeTpByDamage((double)value / Mhp);
}

public void SetActionState(ActionState state)
{
    actionState = state;
    RequestMotionRefresh();
}

public bool IsUndecided() => actionState == ActionState.Undecided;
public bool IsInputting() => actionState == ActionState.Inputting;
public bool IsWaiting() => actionState == ActionState.Waiting;
public bool IsActing() => actionState == ActionState.Acting;

public bool IsChanting()
{
    return IsWaiting() && actions.Any(action => action.IsMagicSkill());
}

public bool IsGuardWaiting()
{
    return IsWaiting() && actions.Any(action => action.IsGuard());
}

public void PerformActionStart(GameAction action)
{
    if (!action.IsGuard())
        SetActionState(ActionState.Acting);
}

public virtual void PerformAction(GameAction action) { }
public virtual void PerformActionEnd() { }
public virtual void PerformDamage() { }

public void PerformMiss() => SoundManager.PlayMiss();
public void PerformRecovery() => SoundManager.PlayRecovery();
public void PerformEvasion() => SoundManager.PlayEvasion();
public void PerformMagicEvasion() => SoundManager.PlayMagicEvasion();
public void PerformCounter() => SoundManager.PlayEvasion();
public void PerformReflection() => SoundManager.PlayReflection();

public virtual void PerformSubstitute(GameBattler target) { }
public virtual void PerformCollapse() { }
}
