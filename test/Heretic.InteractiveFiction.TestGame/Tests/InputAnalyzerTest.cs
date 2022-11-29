using FluentAssertions;
using FluentAssertions.Execution;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.TestGame.GamePlay;
using Xunit;

namespace Heretic.InteractiveFiction.TestGame.Tests;

public class InputAnalyzerTest: IClassFixture<TestFixture>
{
    private Universe universe;
    private TestFixture testFixture;
    private IGrammar grammar => new GermanGrammar(new ResourceProvider());

    public InputAnalyzerTest(TestFixture testFixture)
    {
        this.testFixture = testFixture;
        this.universe = this.testFixture.Universe;
    }
    
    [Theory]
    [InlineData("l", new[] { "LOOK" })]
    [InlineData("look", new[] { "LOOK" })]
    public void SingleWordsTest(string input, string[] expected)
    {
        var sut = new InputAnalyzer(this.universe, this.grammar);

        var actual = sut.Analyze(input);

        Assert.Equal(expected.Length, actual.Length);
        Assert.Equal(expected[0], actual[0]);
    }

    [Theory]
    [InlineData("l kerze", new[] { "LOOK", "kerze" })]
    [InlineData("look kerze", new[] { "LOOK", "kerze" }) ]
    [InlineData("kerze look", new[] { "LOOK", "kerze" })]
    [InlineData("schaue auf die kerze", new[] { "LOOK", "kerze" })]
    [InlineData("kerze schaue auf die ", new[] { "LOOK", "kerze" })]
    [InlineData("Rede mit dem Tisch ", new[] { "TALK", "Tisch" })]
    [InlineData("schaue dich an", new[] { "LOOK", "dich" })]
    [InlineData("lege die kerze hin", new[] { "DROP", "kerze" })]
    [InlineData("lege sie hin", new[] { "DROP", "sie" })]
    public void TwoWordsTest(string input, string[] expected)
    {
        var sut = new InputAnalyzer(this.universe, this.grammar);

        var actual = sut.Analyze(input);

        Assert.Equal(expected.Length, actual.Length);
        Assert.Equal(expected[0], actual[0]);
        Assert.Equal(expected[1], actual[1]);
    }
    
    [Theory]
    [InlineData("TABLE", "lege sie hin", new[] { "DROP", "sie" })]
    [InlineData("TABLE", "lege ihn hin", new[] { "DROP", "Holztisch" })]
    [InlineData("CEILING", "lege sie hin", new[] { "DROP", "Decke" })]
    public void TwoWordsWithPronounTest(string itemKey, string input, string[] expected)
    {
        this.testFixture.SetActiveObject(itemKey);
        var sut = new InputAnalyzer(this.universe, this.grammar);

        var actual = sut.Analyze(input);

        Assert.Equal(expected.Length, actual.Length);
        Assert.Equal(expected[0], actual[0]);
        Assert.Equal(expected[1], actual[1]);
    }
    
    [Theory]
    [InlineData("schaue dir den Tisch an", new[] { "LOOK", "dir", "Tisch" })]
    [InlineData("schaue dir die kerze an", new[] { "LOOK", "dir", "kerze" })]
    [InlineData("schaue sie dir an", new[] { "LOOK", "dir", "sie" })]
    [InlineData("schaue dir sie an", new[] { "LOOK", "dir", "sie" })]
    [InlineData("schaue ihn dir an", new[] { "LOOK", "dir", "ihn" })]
    [InlineData("schaue dir ihn an", new[] { "LOOK", "dir", "ihn" })]
    [InlineData("schaue es dir an", new[] { "LOOK", "dir", "es" })]
    [InlineData("schaue dir es an", new[] { "LOOK", "dir", "es" })]
    [InlineData("nimm dir die Kerze", new[] { "TAKE", "dir", "Kerze" })]
    [InlineData("nimm sie dir", new[] { "TAKE", "dir", "sie" })]
    [InlineData("nimm dir sie", new[] { "TAKE", "dir", "sie" })]
    public void ThreeWordsTest(string input, string[] expected)
    {
        var sut = new InputAnalyzer(this.universe, this.grammar);

        var actual = sut.Analyze(input);

        Assert.Equal(expected.Length, actual.Length);
        Assert.Equal(expected[0], actual[0]);
        Assert.Equal(expected[1], actual[1]);
        Assert.Equal(expected[2], actual[2]);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void AnalyzeSentenceTest(List<string> sentence, string verbKey, string objectOne = "", string objectTwo = "")
    {
        var sut = new InputAnalyzer(this.universe, this.grammar);

        var actualRequest = sut.AnalyzeSentence(sentence);

        using (new AssertionScope())
        {
            actualRequest.Predicate.Key.Should().Be(verbKey);
            actualRequest.ObjectOne?.Key.Should().Be(objectOne);
            actualRequest.ObjectTwo?.Key.Should().Be(objectTwo);
        }
    }
    
    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { new List<string>() {"l"}, "LOOK" },
            new object[] { new List<string>() {"schaue", "dir", "den", "rostigen", "Schlüssel", "an"}, "LOOK", "PLAYER", "RUSTY_KEY" },
            new object[] { new List<string>() {"schaue", "dir", "den", "alten", "Schlüssel", "an"}, "LOOK", "PLAYER", "RUSTY_KEY" },
            new object[] { new List<string>() {"schaue", "dir", "den", "alten", "rostigen", "Schlüssel", "an"}, "LOOK", "PLAYER", "RUSTY_KEY" },
            new object[] { new List<string>() {"schaue", "dir", "den", "rostigen", "alten", "Schlüssel", "an"}, "LOOK", "PLAYER", "RUSTY_KEY" },
            new object[] { new List<string>() {"schaue", "dir", "den", "hölzernen", "Schlüssel", "an"}, "LOOK", "PLAYER", "WOODEN_KEY" },
            new object[] { new List<string>() {"schaue", "dir", "die", "Kerze", "an"}, "LOOK", "PLAYER", "CANDLE" },
            new object[] { new List<string>() {"l", "rostigen", "Schlüssel"}, "LOOK", "RUSTY_KEY" },
            new object[] { new List<string>() {"l", "hölzernen", "Schlüssel"}, "LOOK", "WOODEN_KEY" },
            new object[] { new List<string>() {"nimm", "den", "rostigen", "Schlüssel"}, "TAKE", "RUSTY_KEY", string.Empty },
        };
}
