using RpgSharp.Management;

namespace RpgSharp.Objects;

public class GameActors
{
  protected GameActor[] data;

  public GameActors()
  {
    data = new GameActor[DataManager.DataActors.Count];
  }

  public GameActor Actor(int actorId)
  {
    if (DataManager.DataActors[actorId] == null)
      return null;
    data[actorId] ??= new GameActor(actorId);
    return data[actorId];
  }
}
