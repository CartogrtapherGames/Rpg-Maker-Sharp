using System;
using System.Collections.Generic;
using System.Linq;

namespace RpgSharp.Objects;

public abstract class GameUnit<T> where T : GameBattler
{

  protected bool inBattle;

  protected GameUnit()
  {
    inBattle = false;
  }

  public bool InBattle()
  {
    return inBattle;
  }

  public abstract T[] Members();

  public List<T> AliveMembers() => Members().Where(m => m.IsAlive()).ToList();
  public List<T> DeadMembers() => Members().Where(m => m.IsDead()).ToList();
  public List<T> MovableMembers() => Members().Where(m => m.CanMove()).ToList();

  public void ClearActions()
  {
    foreach (var member in Members())
    {
      member.ClearActions();
    }
  }

  public virtual double Agility()
  {
    var members = Members();
    var sum = members.Sum(m => m.Agi);
    return Math.Max(1, sum / Math.Max(1, members.Length));
  }

  public double TgrSum()
  {
    return AliveMembers().Sum(m => m.Tgr);
  }

  public T RandomTarget()
  {
    var tgrRand = Random.Shared.NextDouble() * TgrSum();
    T target = default;
    foreach (var member in AliveMembers())
    {
      tgrRand -= member.Tgr;
      if (tgrRand <= 0 && target == null)
        target = member;
    }
    return target;
  }

  public T RandomDeadTarget()
  {
    var members = DeadMembers();
    return members.Count > 0 ? members[Random.Shared.Next(members.Length)] : default;
  }

  public T SmoothTarget(int index)
  {
    var member = Members()[Math.Max(0, index)];
    return member != null && member.IsAlive() ? member : AliveMembers().FirstOrDefault();
  }
  
  public T SmoothDeadTarget(int index)
  {
    var member = Members()[Math.Max(0, index)];
    return member != null && member.IsDead() ? member : DeadMembers().FirstOrDefault();
  }

  public void ClearResults()
  {
    foreach (var member in Members())
    {
      member.clearResult();
    }
  }

  public void OnBattleStart(bool advantageous)
  {
    foreach (var member in Members())
    {
      member.OnBattleStart(advantageous);
    }
    inBattle = true;
  }

  public void OnBattleEnd()
  {
    inBattle = false;
    foreach (var member in Members())
    {
      member.OnBattleEnd();
    }
  }

  public void MakeActions()
  {
    foreach (var member in Members())
    {
      member.MakeActions();
    }
  }

  public void Select(T activeMember)
  {
    foreach (var member in Members())
    {
      if (member == activeMember)
      {
        member.Select();
      } else {
        member.Deselect();
        
      }
    }
  }

  public bool IsAllDead()
  {
    return AliveMembers().Count == 0;
  }
  
  public T SubstituteTarget(T target)
  {
    return Members().FirstOrDefault(member => member.IsSubstitute() && member != target);
  }
  
  public double TpbBaseSpeed()
  {
    return Members().Max(m => m.TpbBaseSpeed());
  }

  public double TpbReferenceTime()
  {
    return BattleManager.IsActiveTpb() ? 240 : 60;
  }

  public void UpdateTpb()
  {
    foreach (var member in Members())
    {
      member.UpdateTpb();
    }
  }
}
