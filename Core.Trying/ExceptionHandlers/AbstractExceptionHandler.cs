using System;

namespace Core.Trying.ExceptionHandlers
{

  public abstract class AbstractExceptionHandler<T> : IExpectedExceptionHandler where T : Exception
  {
    protected abstract bool HandleException(T exception, TryContext tryContext);
    bool IExpectedExceptionHandler.HandleException(TryContext tryContext)
    {
      if (!(tryContext.LastException is T)) return false;
      return HandleException(tryContext.LastException as T, tryContext);
    }
  }
}
