
namespace Core.Trying.WaitingStrategies
{
  public class ExponentialBackoffWaitingStrategy : ITryWaitingStrategy
  {
    public ExponentialBackoffWaitingStrategy(int ms, int initialWaitMs)
    {
      this.Timeout = ms;
      this.InitialTimeout = initialWaitMs;
    }
    public int ComputeDelay(TryContext context)
    {
      var i = context.FailedCount;
      if (i == 1) return InitialTimeout;
      return i * i * Timeout;

    }

    public int InitialTimeout { get; set; }

    public int Timeout { get; set; }
  }
}
