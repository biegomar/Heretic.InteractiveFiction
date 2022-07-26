namespace Heretic.InteractiveFiction.Exceptions;

public class StandUpException: Exception
{
    public StandUpException()
    {
    }

    public StandUpException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public StandUpException(string message) : base(message)
    {
    }

    public StandUpException(string message, Exception innerException) : base(message, innerException)
    {
    }   
}