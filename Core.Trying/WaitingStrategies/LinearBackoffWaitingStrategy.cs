
namespace Core.Trying.WaitingStrategies
{

  public class LinearWaitingStrategy : ITryWaitingStrategy
  {

    public LinearWaitingStrategy(int timeout, int initialTimeout) { this.Timeout = timeout; this.InitialTimeout = initialTimeout; }
    public int ComputeDelay(TryContext context)
    {
      if (context.FailedCount == 1) return InitialTimeout;
      return (context.FailedCount - 1) * Timeout;
    }

    public int Timeout { get; set; }
    public int InitialTimeout { get; set; }
  }
}
