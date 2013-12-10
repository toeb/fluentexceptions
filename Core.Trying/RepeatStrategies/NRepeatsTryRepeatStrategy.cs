
namespace Core.Trying.RepeatStrategies
{

  public class NRepeatsTryRepeatStrategy : ITryRepeatStrategy
  {
    public NRepeatsTryRepeatStrategy(int n)
    {
      this.N = n;
    }


    public int N { get; set; }

    public bool ComputeRepeat(TryContext context)
    {
      return context.FailedCount < N;
    }
  }

}
