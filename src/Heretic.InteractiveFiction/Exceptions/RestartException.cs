namespace Heretic.InteractiveFiction.Exceptions;

public class RestartException : Exception
{
    public RestartException(string message) : base(message)
    {
    }
}