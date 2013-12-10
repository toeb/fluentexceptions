using System;

namespace Core.Trying.ExceptionHandlers
{
  /// <summary>
  /// just accepts any exception of type T and returns true(indicating the exception was handled)
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class SimpleExceptionHandler<T> : AbstractExceptionHandler<T> where T : Exception
  {
    protected override bool HandleException(T exception, TryContext tryContext)
    {
      return true;
    }
  }
}
