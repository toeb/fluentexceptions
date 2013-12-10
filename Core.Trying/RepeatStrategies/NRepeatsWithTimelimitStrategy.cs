
namespace Core.Trying.RepeatStrategies
{

  public class NRepeatsWithTimelimitStrategy : ITryRepeatStrategy
  {
    public NRepeatsWithTimelimitStrategy(int n, int timelimit)
    {
      this.Limit = timelimit;
      this.N = n;
    }


    public int N { get; set; }
    public bool ComputeRepeat(TryContext context)
    {
      if (context.WaitedTime >= Limit) return false;
      return context.FailedCount < N;
    }

    public int Limit { get; set; }
  }

}
