
namespace Core.Trying
{

  public interface ITryWaitingStrategy
  {
    /// <summary>
    /// should return the amout of wait time until next execution of action
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    int ComputeDelay(TryContext context);
  }
}
