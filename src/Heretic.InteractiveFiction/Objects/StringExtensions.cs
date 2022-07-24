namespace Heretic.InteractiveFiction.Objects;

public static class StringExtensions
{
    public static string LowerFirstChar(this string str)
    {
        return str[..1].ToLower() + str[1..];
    }
}