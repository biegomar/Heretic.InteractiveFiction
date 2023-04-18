namespace Heretic.InteractiveFiction.Exceptions;

public class GameWonException: Exception
{
    public GameWonException(string message) : base(message)
    {
    }
}