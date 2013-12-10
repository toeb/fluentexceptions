fluentexceptions
================

A Fluent Way of Exception Handling


While doing some Filesystemwork I encountered alot of Access exceptions because a file is being opened to write multiple times.  Even after closing the file explicitly a short timeframe exists in which the file could not be opened again.  The simplest approach for me was to retry openening the file until it worked.  To allow a more easy and more general way to handle exceptions I developed a small extensible try repeat framework which allows me to configure waiting and repeating strategies and also only react to specific exceptions.  I have written a small fluent interface which allows easy usage:

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
// default configuration handles any exception
  // repeats 4 times at most and exponentially backs off
  Try.Default.Execute(() => {/*some action which potentially throws*/});
  Be warned that this type of repeat on exception strategy should not be applied blindly …

I developed the library because I needed it but some terminology was researched.  consulted sources:

http://msdn.microsoft.com/en-us/library/hh680905(v=pandp.50)
http://stackoverflow.com/questions/1563191/c-sharp-cleanest-way-to-write-retry-logic
