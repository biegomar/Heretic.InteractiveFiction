namespace Heretic.InteractiveFiction.Exceptions;

public class LeaveLocationException : Exception
{
    public LeaveLocationException()
    {
    }

    protected LeaveLocationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public LeaveLocationException(string message) : base(message)
    {
    }

    public LeaveLocationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
