using Core.Trying.ExceptionHandlers;
using Core.Trying.RepeatStrategies;
using Core.Trying.WaitingStrategies;
using System;

namespace Core.Trying
{
  public static class TryExtensions
  {
    /// <summary>
    /// sets the waiting strategy to backoff expentially when failing
    /// </summary>
    /// <param name="self"></param>
    /// <param name="timeout"></param>
    /// <param name="initialTimeout"></param>
    /// <returns></returns>
    public static Try BackoffExponentially(this Try self, int timeout = 10, int initialTimeout = 0)
    {
      var strategy = new ExponentialBackoffWaitingStrategy(timeout, initialTimeout);
      self.WithWaitingStrategy(strategy);
      return self;
    }
    /// <summary>
    /// sets the waiting strategy to backkoff a linearly increasing amount of time 100, 200, 300 after each failure
    /// </summary>
    /// <param name="self"></param>
    /// <param name="timeout"></param>
    /// <param name="initialTimeout"></param>
    /// <returns></returns>
    public static Try BackoffLinearly(this Try self, int timeout = 100, int initialTimeout = 0)
    {
      var strategy = new LinearWaitingStrategy(timeout, initialTimeout);
      self.WithWaitingStrategy(strategy);
      return self;
    }

    /// <summary>
    /// sets up waiting strategy to always wait same amount
    /// </summary>
    /// <param name="self"></param>
    /// <param name="timeout"></param>
    /// <param name="initialTimeout"></param>
    /// <returns></returns>
    public static Try WaitConstantAmount(this Try self, int timeout, int initialTimeout = 0)
    {
      var strategy = new ConstantWaitingStrategy(timeout, initialTimeout);
      self.WithWaitingStrategy(strategy);
      return self;
    }

    /// <summary>
    /// sets the waitingstrategy fluently
    /// </summary>
    /// <param name="self"></param>
    /// <param name="strategy"></param>
    /// <returns></returns>
    public static Try WithWaitingStrategy(this Try self, ITryWaitingStrategy strategy)
    {
      self.WaitingStrategy = strategy;
      return self;
    }
    /// <summary>
    /// adds an exceptionhandler fluently
    /// </summary>
    /// <param name="self"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static Try HandleException(this Try self, IExpectedExceptionHandler handler)
    {
      self.AddExceptionHandler(handler);
      return self;
    }

    /// <summary>
    /// sets the number of times that the action is to be repeated if it continuosly fails
    /// </summary>
    /// <param name="self"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Try Repeat(this Try self, int n)
    {
      var strategy = new NRepeatsTryRepeatStrategy(n);
      self.RepeatStrategy = strategy;
      return self;
    }

    /// <summary>
    /// sets the max number of repeats and max time to wait until action fails continuously
    /// </summary>
    /// <param name="self"></param>
    /// <param name="n"></param>
    /// <param name="maxWaitingTime"></param>
    /// <returns></returns>
    public static Try Repeat(this Try self, int n, int maxWaitingTime)
    {
      var strategy = new NRepeatsWithTimelimitStrategy(n, maxWaitingTime);
      self.RepeatStrategy = strategy;
      return self;
    }

    /// <summary>
    /// adds a simple exceptionhandler which expects an exception of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Try Expect<T>(this Try self) where T : Exception
    {
      return self.HandleException(new SimpleExceptionHandler<T>());

    }

    /// <summary>
    /// adds a custom exceptionhandler that is called whenever an exception of type T occurs
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="exceptionHandler"></param>
    /// <returns></returns>
    public static Try Expect<T>(this Try self, Action<T> exceptionHandler) where T : Exception
    {
      self.HandleException(new DelegateExceptionHandler<T>(exceptionHandler));
      return self;
    }
    /// <summary>
    /// sets wether to throw exception when failing or to fail quietly
    /// </summary>
    /// <param name="self"></param>
    /// <param name="quiet"></param>
    /// <returns></returns>
    public static Try BeQuiet(this Try self, bool quiet)
    {
      self.FailQueitly = quiet;
      return self;
    }
    /// <summary>
    /// Executes a action with result and returns this result on success
    /// on failure an exception is thrown
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="self"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static TResult Execute<TResult>(this Try self, Func<TResult> action)
    {
      return self.Execute<TResult>(context => action());
    }
    /// <summary>
    /// executes the action with result and passes in the current try context
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="self"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static TResult Execute<TResult>(this Try self, Func<TryContext, TResult> action)
    {

      TResult result = default(TResult);
      self.BeQuiet(false)
      .Execute((context) =>
      {
        result = action(context);
      });
      return result;
    }

    /// <summary>
    /// allows a nullary(no args) action to be executed
    /// </summary>
    /// <param name="self"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static TryContext Execute(this Try self, Action action)
    {
      return self.Execute(context=>action());
    }
  }
}
