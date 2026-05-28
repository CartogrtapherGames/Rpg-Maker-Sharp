namespace RpgSharp.Objects;


public enum ActionEffect {
  RecoverHp = 11,
  RecoverMp = 12,
  GainTp = 13,
  AddState = 21,
  RemoveState = 22,
  AddBuff = 31,
  AddDebuff = 32,
  RemoveBuff = 33,
  RemoveDebuff = 34,
  Special = 41,
  Grow = 42,
  LearnSkill = 43,
  CommonEvent = 44,
  SpecialEffectEscape = 0,
}
public class GameAction
{
  protected int subjectActorId;
  protected int subjectEnemyIndex;
  protected bool forcing;
  protected GameItem item;
  protected int targetIndex;
  protected GameBattler reflectionTarget;


  public GameAction(GameBattler subject, bool force = false)
  {
    subjectActorId = 0;
    subjectEnemyIndex = 0;
    forcing = force;
    SetSubject(subject);
    Clear();
  }

  public void Clear()
  {
    item = new GameItem();
    targetIndex = -1;
  }

  public void SetSubject(GameBattler subject)
  {
    if (subject is GameActor actor)
    {
      subjectActorId = actor.actorId;
      subjectEnemyIndex = -1;
      return;
    }

    if (subject is GameEnemy enemy)
    {
      subjectEnemyIndex = enemy.Subject();
      subjectActorId = 0;
      return;
    }
  }
}
