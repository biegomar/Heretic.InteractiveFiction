namespace Heretic.InteractiveFiction.Exceptions;

public class AmbiguousHereticObjectException: Exception
{
    public AmbiguousHereticObjectException()
    {
    }

    public AmbiguousHereticObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public AmbiguousHereticObjectException(string message) : base(message)
    {
    }

    public AmbiguousHereticObjectException(string message, Exception innerException) : base(message, innerException)
    {
    }
}