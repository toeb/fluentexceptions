using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Trying.ExceptionHandlers
{
  class DelegateExceptionHandler<T> : IExpectedExceptionHandler where T : Exception
  {
    private Action<T> exceptionHandler;

    public DelegateExceptionHandler(Action<T> exceptionHandler)
    {

      this.exceptionHandler = exceptionHandler;
    }

    public bool HandleException(TryContext tryObject)
    {
      var ex = tryObject.LastException as T;
      exceptionHandler(ex);
      return true;
    }
  }
}
