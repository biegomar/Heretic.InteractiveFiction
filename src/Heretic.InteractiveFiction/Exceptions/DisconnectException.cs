namespace Heretic.InteractiveFiction.Exceptions;

public class DisconnectException: Exception
{
    public DisconnectException(string message) : base(message)
    {
    }
}