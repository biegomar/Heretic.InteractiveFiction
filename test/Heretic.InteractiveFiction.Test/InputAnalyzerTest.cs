using Heretic.GamePlay;
using Heretic.GamePlay.Prerequisites;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.GamePlay.EventSystem;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Xunit;
using Moq;

namespace Heretic.Test;

public class InputAnalyzerTest
{
    private IPrintingSubsystem printingSubsystem => Mock.Of<IPrintingSubsystem>();
    private IResourceProvider resourceProvider => Mock.Of<IResourceProvider>();

    [Theory]
    [InlineData("l", new[] { "l" })]
    [InlineData("look", new[] { "look" })]
    public void SingleWordsTest(string input, string[] expected)
    {
        var universe = new Universe(printingSubsystem, resourceProvider, new PeriodicEvent());
        var sut = new InputAnalyzer(universe);

        var actual = sut.Analyze(input);

        Assert.Equal(expected[0], actual[0]);
    }

    [Theory]
    [InlineData("l man", new[] { "l", "man" })]
    [InlineData("look man", new[] { "look", "man" })]
    [InlineData("man look", new[] { "look", "man" })]
    [InlineData("schaue auf den Mann", new[] { "schaue", "Mann" })]
    [InlineData("Mann schaue auf den ", new[] { "schaue", "Mann" })]
    [InlineData("Rede mit dem Tankwart ", new[] { "Rede", "Tankwart" })]
    public void TwoWordsTest(string input, string[] expected)
    {
        var universe = new Universe(printingSubsystem, resourceProvider, new PeriodicEvent());
        var sut = new InputAnalyzer(universe);

        var actual = sut.Analyze(input);

        Assert.Equal(expected[0], actual[0]);
    }
}
