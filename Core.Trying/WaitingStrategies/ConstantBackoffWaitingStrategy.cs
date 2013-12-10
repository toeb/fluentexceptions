
namespace Core.Trying.WaitingStrategies
{
  public class ConstantWaitingStrategy : ITryWaitingStrategy
  {
    public ConstantWaitingStrategy(int timeout, int initialTimeout) { this.Timeout = timeout; this.InitialTimeout = initialTimeout; }
    public int ComputeDelay(TryContext context)
    {
      if (context.FailedCount == 1) return InitialTimeout;
      return Timeout;
    }

    public int InitialTimeout { get; set; }

    public int Timeout { get; set; }
  }

}
