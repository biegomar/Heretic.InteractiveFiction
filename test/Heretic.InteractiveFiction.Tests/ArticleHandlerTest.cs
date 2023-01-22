using FluentAssertions;
using Heretic.InteractiveFiction.Grammars;

namespace Heretic.InteractiveFiction.Tests;

public class ArticleHandlerTest
{
    [Theory]
    [InlineData("lege die kerze in ofen", "kerze", true)]
    [InlineData("lege kerze in ofen", "kerze", false)]
    [InlineData("lege kerze in den ofen", "kerze", false)]
    [InlineData("lege die kerze in den ofen", "kerze", true)]
    public void CandleHasArticleInFrontOfObjectShouldSucceed(string sentence, string associatedWord, bool expected)
    {
        var parts = sentence.Split(' ').ToList();
        var candle = TestDataProvider.Candle;

        var actual = ArticleHandler.HasArticleInFrontOfObject(parts, candle, associatedWord);

        actual.Should().Be(expected);

    }
    
    [Theory]
    [InlineData("lege die kerze in ofen", "ofen", false)]
    [InlineData("lege kerze in ofen", "ofen", false)]
    [InlineData("lege kerze in den ofen", "ofen", true)]
    [InlineData("lege die kerze in den ofen", "ofen", true)]
    public void StoveHasArticleInFrontOfObjectShouldSucceed(string sentence, string associatedWord, bool expected)
    {
        var parts = sentence.Split(' ').ToList();
        var candle = TestDataProvider.Stove;

        var actual = ArticleHandler.HasArticleInFrontOfObject(parts, candle, associatedWord);

        actual.Should().Be(expected);

    }
}