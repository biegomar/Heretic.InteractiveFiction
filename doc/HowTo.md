# How to use Heretic.InteractiveFiction

## 1. Setup a new console project

```shell
dotnet new console HereticExample
```

```shell
dotnet add package Heretic.InteractiveFiction
```

## 2. Resource files at the heart of your game
Heretic relies on resource files for all of its texts to provide a simple technical option for multi-language support. You should also take this approach. By convention, there should be at least one resource file where you maintain your messages and descriptions. This file should be named Descriptions.resx. Of course you can name the file whatever you like, or add and use any other resource files you want!  
Furthermore, the framework offers support for four special resource files, in which the names, or identifiers of the various items, characters, locations and conversations of the game are stored. These are:
* Locations
* Items
* Characters
* Conversations

But what's so special about these resource files? Well, it makes sense to provide an item not only with one caption, but to offer more options (e.g. bookshelf, shelf, rack) to make it more convenient for the player.  
This is achieved by the fact that the values of the resources are a number of words separated from each other by the pipe (|).  

Example:  
![Resource file example](LocalizationManager.png "Resource file example")

## 3. Ramp up your console printing
Heretic provides a simple way to print text to the console. You can use the abstract class and extend it to create your own print methods. Or you can implement your own class based on the underlying interface IPrintingSubsystem. But using the abstract class is easier in the beginning.

```csharp
internal sealed class ConsolePrinting: BaseConsolePrintingSubsystem
{
    public override bool Opening()
    {
        Console.WriteLine("Welcome to the Log Cabin!");

        return true;
    }

    public override bool Closing()
    {
        Console.WriteLine("Thank you for playing the Log Cabin!");
        
        return true;
    }

    public override bool TitleAndScore(int score, int maxScore)
    {
        Console.Title = $"{string.Format(BaseDescriptions.SCORE, score, maxScore)}";
        return true;
    }

    public override bool Credits()
    {
        Console.WriteLine("Written by me.");
        
        return true;
    }
}
```
### Tip: always use Resource files for your messages

If you use a resource file named "Descriptions" with a string called CREDITS you can use the following code to get the message from the resource file:

```csharp
public override bool Credits()
{
    Console.WriteLine(Descriptions.CREDITS);
        
    return true;
}
```


