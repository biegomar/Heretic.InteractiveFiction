namespace Heretic.InteractiveFiction.Exceptions;

public class BuyException : Exception
{
    public BuyException()
    {
    }

    public BuyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public BuyException(string message) : base(message)
    {
    }

    public BuyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
