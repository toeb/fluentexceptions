using Core.Trying.WaitingStrategies;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Trying
{

  /// <summary>
  /// Central class which handles execution of action.
  /// should be configured via fluent api
  /// </summary>
  public class Try
  {
    private IList<IExpectedExceptionHandler> exceptionHandlers = new List<IExpectedExceptionHandler>();
    public ITryWaitingStrategy WaitingStrategy { get; set; }
    public ITryRepeatStrategy RepeatStrategy { get; set; }

    /// <summary>
    /// indicates wether an exception is thrown if action fails continuosly
    /// </summary>
    public bool FailQueitly
    {
      get;
      set;
    }
    /// <summary>
    /// initializes a default configuration 
    /// * 4 repeats
    /// * fails loudly (throws exception on failure)
    /// * exponentially backs off on failure
    /// </summary>
    public Try()
    {
      this.Repeat(4)
          .BeQuiet(false)
          .WithWaitingStrategy(new ExponentialBackoffWaitingStrategy(50, 0));
    }
    /// <summary>
    /// selfexplainatory
    /// </summary>
    /// <param name="handler"></param>
    public void AddExceptionHandler(IExpectedExceptionHandler handler)
    {
      this.exceptionHandlers.Add(handler);
    }

    private void ExecuteAction(TryContext context)
    {
      // if action fails an exception is thrown
      context.Action(context);
      // so now action must habe been successful
      context.Succeeded = true;
    }

    /// <summary>
    /// a default configuration
    /// repeats the action 3 times exponentially backing of when failing
    /// throws exception if fails
    /// expects any exception
    /// </summary>
    public static Try Default
    {
      get
      {
        return new Try()
          .Expect<Exception>()
          .Repeat(4)
          .BeQuiet(false)
          .WithWaitingStrategy(new ExponentialBackoffWaitingStrategy(50, 0));
      }
    }



    /// <summary>
    /// executes the specified action and returns the corresponsing TryContext
    /// also see the extensions method TryExtensions.Execute which returns the result of an action rather than a trycontext
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public TryContext Execute(Action<TryContext> action)
    {
      var tryContext = new TryContext()
      {
        Action = action,
        FailedCount = 0,
        Try = this,


      };
      // as long as action should repeat
      while (RepeatStrategy.ComputeRepeat(tryContext))
      {
        try
        {
          ExecuteAction(tryContext);
          // action was successfully executed
          return tryContext;
        }
        catch (Exception e)
        {
          //increase failedcount
          tryContext.FailedCount++;
          tryContext.AddException(e);
          // try to handle exceptions
          bool handled = false;
          foreach (var handler in exceptionHandlers)
          {
            handled |= handler.HandleException(tryContext);
          }
          // if exception is unhandled stop retrying
          if (!handled)
          {
            if (FailQueitly)
            {
              break;
            }
            //throw exception directly
            throw e;

          }
        }
        // do not wait if action should not be repeated anyways
        if (!RepeatStrategy.ComputeRepeat(tryContext)) break;

        // wait for a strategic amount of time before next retry
        var wait = WaitingStrategy.ComputeDelay(tryContext);
        Thread.Sleep(wait);
        tryContext.WaitedTime += wait;
      }
      // throw exception if FailQuietly is false
      if (!FailQueitly && !tryContext)
      {
        throw new AggregateException("failed to execute", tryContext.Exceptions);
      }

      return tryContext;
    }


  }

}
