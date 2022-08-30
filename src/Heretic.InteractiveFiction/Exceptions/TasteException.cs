namespace Heretic.InteractiveFiction.Exceptions;

public class TasteException: Exception
{
    public TasteException()
    {
    }

    public TasteException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public TasteException(string message) : base(message)
    {
    }

    public TasteException(string message, Exception innerException) : base(message, innerException)
    {
    } 
}