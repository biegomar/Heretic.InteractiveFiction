namespace Heretic.InteractiveFiction.Exceptions;

public class JumpException: Exception
{
    public JumpException()
    {
    }

    public JumpException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public JumpException(string message) : base(message)
    {
    }

    public JumpException(string message, Exception innerException) : base(message, innerException)
    {
    }
}