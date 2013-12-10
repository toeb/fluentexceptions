using Core.Trying;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

using System.IO;
using Core.Trying.Test;

namespace Core.Test
{

  [TestClass]
  public class CoreTryTests
  {

    [TestMethod]
    public void ShouldExecuteActionWithoutFailure()
    {
      var executed = false;
      var result = new Try().Execute(() => executed = true);
      Assert.IsTrue(result);
      Assert.IsTrue(executed);
    }
    [TestMethod]
    public void ShouldRetryFailingAction()
    {
      var result = new Try()
       .Repeat(3)
       .BeQuiet(true)
         .Expect<TestException>()
         .Execute(() => { throw new TestException(); });

      Assert.IsFalse(result);
      Assert.AreEqual(3, result.FailedCount);
      Assert.AreEqual(3, result.Exceptions.Count());
    }
    [TestMethod]
    public void ShouldRetryNTimesWhenFailing()
    {
      var result = new Try()
       .Repeat(4)
       .BeQuiet(true)
         .Expect<TestException>()
         .Execute(() => { throw new TestException(); });
      Assert.AreEqual(4, result.FailedCount);
    }

    [TestMethod]
    public void ShouldShowCapabilitiesOfFramework()
    {
      var configuration = new Try()
      .BeQuiet(true)// do not throw exceptions when action continouosly fails
      .Expect<AccessViolationException>() // only expect access violation exceptions
      .Repeat(5, 1000)// repeat at most 5 times or until 1s of waiting time has passed
      .BackoffExponentially();// when actions fails backoff exponentially e.g. first wait 10 ms then 100 ms then 1000ms
      //configuration is reusable
      var result = configuration.Execute(() => { /*something which potentially throws */});

      if (result)
      {
        // success
        Console.WriteLine("number of retries needed until success: " + result.FailedCount);
      }
      else
      {
        // failure
        Console.WriteLine("time spent waiting [ms]: " + result.WaitedTime);
        Console.WriteLine("number of retries: " + result.FailedCount);
        Console.WriteLine("last throw exception message: " + result.LastException.Message);
      }

    }
    [TestMethod]
    public void ShouldShowHowShortTheConfigurationCanBe()
    {
      // default configuration handles any exception
      // repeats 4 times at most and exponentially backs off
      Try.Default.Execute(() => {/*some action which potentially throws*/});
    }

    [TestMethod]
    public void ShouldExponentiallyBackoffWhenFailing()
    {
      var watch = Stopwatch.StartNew();
      var result = new Try()
      .BeQuiet(true)
        .Repeat(10)
        .Expect<Exception>()
        .BackoffLinearly(10, 0)
        .Execute(() => { throw new Exception(); });
      var t = watch.ElapsedMilliseconds;
      Assert.IsTrue(t > 300);
    }
    [TestMethod]
    public void ShouldReturnEventually()
    {
      var fail = true;
      var resul = new Try()
      .Repeat(2)
      .Expect<Exception>()
      .Execute(() => { if (fail) { fail = false; throw new Exception(); } });

      Assert.AreEqual(1, resul.FailedCount);
      Assert.IsTrue(resul);

    }
    [TestMethod]
    public void ShouldReturnResultOnSuccess()
    {
      var result = new Try().Execute(() => 3);
      Assert.AreEqual(3, result);
    }

    [TestMethod]
    [ExpectedException(typeof(AggregateException))]
    public void ShouldThrowAggregateExceptionIfFailed()
    {
      var result = new Try().Expect<Exception>().Execute(() => { throw new Exception(); });
    }

    [TestMethod]
    public void ShouldThrowDirectlyIfExceptionIsUnhandled()
    {
      var uut = new Try();
      try
      {
        var i = 0;
        uut.Expect<RightException>().Execute(() =>
        {
          i++;
          if (i == 1) throw new RightException();
          throw new WrongException();
        });
      }
      catch (WrongException e)
      {

      }
      catch (RightException re)
      {

      }
    }

    [TestMethod]
    public void ShouldHaveAllExceptionInAggregateException()
    {

      int i = 0;
      try
      {
        var result = new Try().Repeat(3).Expect<Exception>().WaitConstantAmount(0).Execute(() => { throw new Exception("" + (i++)); });
      }
      catch (AggregateException e)
      {
        Assert.AreEqual(3, e.InnerExceptions.Count);
        Assert.IsFalse(e.InnerExceptions.Select(ex => ex.Message).Except(new[] { "0", "1", "2" }).Any());
      }
    }

    [TestMethod]
    public void ShouldCallCustomCatchBlock()
    {
      var exceptionHandled = false;
     // expects RightException and calls a custom handler when it occurs
      // expect any other exception which will be ignored quitely
      // returns value of operation on success
      var result = new Try().Repeat(3)
        .Expect<RightException>(ex => { exceptionHandled = true; })
        .Expect<Exception>()
        .Execute(context =>
        {
          switch (context.FailedCount)
          {
            case 0:
              throw new WrongException();
            case 1: throw new RightException();
            case 2: return "result";
            default: throw new Exception();
          }
        });

      Assert.AreEqual("result", result);
      Assert.IsTrue(exceptionHandled);
    }
  }
}
