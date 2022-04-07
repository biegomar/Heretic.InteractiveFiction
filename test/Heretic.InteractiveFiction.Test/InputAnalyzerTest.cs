using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.GamePlay.EventSystem;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.Test.Mocks;
using Xunit;
using Moq;

namespace Heretic.Test;

public class InputAnalyzerTest
{
    private IPrintingSubsystem printingSubsystem => Mock.Of<IPrintingSubsystem>();
    private IGamePrerequisitesAssembler gamePrerequisitesAssembler => Mock.Of<IGamePrerequisitesAssembler>();

    [Theory]
    [InlineData("l", new[] { "l" })]
    [InlineData("look", new[] { "look" })]
    public void SingleWordsTest(string input, string[] expected)
    {
        var universe = new Universe(printingSubsystem, new ResourceProviderMock());
        var sut = new InputAnalyzer(universe);

        var actual = sut.Analyze(input);

        Assert.Equal(expected[0], actual[0]);
    }

    [Theory]
    [InlineData("l mann", new[] { "l", "mann" })]
    [InlineData("look mann", new[] { "look", "mann" }) ]
    [InlineData("mann look", new[] { "look", "mann" })]
    [InlineData("schaue auf den Mann", new[] { "schaue", "Mann" })]
    [InlineData("Mann schaue auf den ", new[] { "schaue", "Mann" })]
    [InlineData("Rede mit dem Tankwart ", new[] { "Rede", "Tankwart" })]
    public void TwoWordsTest(string input, string[] expected)
    {
        var universe = new Universe(printingSubsystem, new ResourceProviderMock());
        var sut = new InputAnalyzer(universe);

        var actual = sut.Analyze(input);

        Assert.Equal(expected[0], actual[0]);
        Assert.Equal(expected[1], actual[1]);
    }
}
