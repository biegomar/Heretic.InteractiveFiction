namespace Heretic.InteractiveFiction.Exceptions;

public class QuitGameException: Exception
{
    public QuitGameException(string message) : base(message)
    {
    }
}