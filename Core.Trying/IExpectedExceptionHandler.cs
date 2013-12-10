
namespace Core.Trying
{

  public interface IExpectedExceptionHandler
  {
    /// <summary>
    /// should return true if this handler could handle the exception
    /// </summary>
    /// <param name="tryObject"></param>
    /// <returns></returns>
    bool HandleException(TryContext tryObject);
  }

}
