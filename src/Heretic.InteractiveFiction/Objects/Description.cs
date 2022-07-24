namespace Heretic.InteractiveFiction.Objects;

public sealed class Description
{
    private readonly Func<string> descriptionFunc;

    public static implicit operator Description(string value)  
    {  
        return new Description(() => value);  
    }
    
    public static implicit operator Description(Func<string> value)  
    {  
        return new Description(value);  
    }
    
    public static implicit operator string(Description description)  
    {
        return description?.ToString();
    } 
    
    public Description()
    {
        this.descriptionFunc = () => string.Empty;
    }
    
    public Description(Func<string> function)
    {
        this.descriptionFunc = function;
    }

    public override string ToString()
    {
        return this.descriptionFunc();
    }
}