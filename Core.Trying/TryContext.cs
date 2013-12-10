using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Trying
{
  /// <summary>
  /// a class which stores data about a action which is trying to execute
  /// </summary>
  public class TryContext
  {
    /// <summary>
    /// implicit conversion to allow chcking if action was successfull if(context){... success}else{... failure}
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static implicit operator bool(TryContext context)
    {
      return context.Succeeded;
    }
    /// <summary>
    /// default constructor initializes a failing action with no exceptions
    /// </summary>
    public TryContext()
    {
      Exceptions = new List<Exception>();
    }
    /// <summary>
    /// the amount of time spent waiting
    /// </summary>
    public int WaitedTime { get; set; }
    /// <summary>
    /// contains all exceptions thrown by action
    /// </summary>
    public IList<Exception> Exceptions { get; set; }
    /// <summary>
    /// indicator whether action completed successfull or not
    /// </summary>
    public bool Succeeded { get; set; }
    /// <summary>
    /// the tryobject which was used to execute the action
    /// </summary>
    public Try Try { get; set; }
    /// <summary>
    /// the number of times the action failed
    /// </summary>
    public int FailedCount { get; set; }
    /// <summary>
    /// the action itself
    /// </summary>
    public Action<TryContext> Action { get; set; }
    /// <summary>
    /// convenience method to an exception
    /// </summary>
    /// <param name="e"></param>
    public void AddException(Exception e) { Exceptions.Add(e); }
    /// <summary>
    /// the current (last) exception which was added
    /// </summary>
    public Exception LastException
    {
      get
      {
        return Exceptions.Last();
      }
    }
  }
}
