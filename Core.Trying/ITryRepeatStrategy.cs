
namespace Core.Trying
{

  public interface ITryRepeatStrategy
  {
    /// <summary>
    /// should return true iff the action should be repeated
    /// should be a const operation so it can be called multiple times per iteration
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool ComputeRepeat(TryContext context);
  }
}
