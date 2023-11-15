using System.Threading.Tasks;
using StronglyTypedIds.Diagnostics;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace StronglyTypedIds.Tests.Diagnostics;

[UsesVerify]
public class DiagnosticsTests
{
    public const string NoIdGenerationSnapshotName = "NoGeneratedIds";

    [Theory]
    [InlineData("\"\"")]
    [InlineData("\"       \"")]
    public void EmptyTemplate_GivesInvalidTemplateNameDiagnostic_AndDoesntGenerate(string template)
    {
        var input = $$"""
                      using StronglyTypedIds;

                      [StronglyTypedId({{template}})]
                      public partial struct MyId {}
                      """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == InvalidTemplateNameDiagnostic.Id);
        Assert.Empty(output);
    }

    [Fact]
    public void MultipleAssemblyAttributes_GivesMultipleAttributeDiagnostic_AndDoesntGenerate()
    {
        const string input = """
                             using StronglyTypedIds;
                             [assembly:StronglyTypedIdDefaults(Template.Int)]
                             [assembly:StronglyTypedIdDefaults(Template.Long)]

                             [StronglyTypedId]
                             public partial struct MyId {}
                             """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == MultipleAssemblyAttributeDiagnostic.Id);
        Assert.Empty(output);
    }

    [Fact]
    public void InvalidTemplate_GivesDiagnostic_AndDoesntGenerate()
    {
        const string input = """
                             using StronglyTypedIds;

                             [StronglyTypedId("some-template")]
                             public partial struct MyId {}
                             """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == UnknownTemplateDiagnostic.Id);
        Assert.Empty(output);
    }

    [Fact]
    public void InvalidTemplateInDefaultsAttribute_GivesDiagnostic_AndDoesntGenerate()
    {
        const string input = """
                             using StronglyTypedIds;
                             [assembly:StronglyTypedIdDefaults("some-template")]

                             [StronglyTypedId]
                             public partial struct MyId {}
                             """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == UnknownTemplateDiagnostic.Id);

        Assert.Empty(output);
    }
}