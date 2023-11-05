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
    [InlineData("null")]
    [InlineData("\"\"")]
    [InlineData("\"       \"")]
    public Task EmptyTemplate_GivesInvalidTemplateNameDiagnostic_AndDoesntGenerate(string template)
    {
        var input = $$"""
                      using StronglyTypedIds;

                      [StronglyTypedId({{template}})]
                      public partial struct MyId {}
                      """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == InvalidTemplateNameDiagnostic.Id);

        return Verifier.Verify(output)
            .DisableRequireUniquePrefix()
            .UseDirectory("Snapshots")
            .UseFileName(NoIdGenerationSnapshotName);
    }

    [Fact]
    public Task MultipleAssemblyAttributes_GivesMultipleAttributeDiagnostic_AndDoesntGenerate()
    {
        const string input = """
                             using StronglyTypedIds;
                             [assembly:StronglyTypedIdDefaults(Template.Int)]
                             [assembly:StronglyTypedIdDefaults(Template.Long)]

                             [StronglyTypedId]
                             public partial struct MyId {}
                             """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == MultipleAssemblyAttributeDiagnostic.Id);

        return Verifier.Verify(output)
            .DisableRequireUniquePrefix()
            .UseDirectory("Snapshots")
            .UseFileName(NoIdGenerationSnapshotName);
    }

    [Fact]
    public Task InvalidTemplate_GivesDiagnostic_AndDoesntGenerate()
    {
        const string input = """
                             using StronglyTypedIds;

                             [StronglyTypedId("some-template")]
                             public partial struct MyId {}
                             """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == UnknownTemplateDiagnostic.Id);

        return Verifier.Verify(output)
            .DisableRequireUniquePrefix()
            .UseDirectory("Snapshots")
            .UseFileName(NoIdGenerationSnapshotName);
    }

    [Fact]
    public Task InvalidTemplateInDefaultsAttribute_GivesDiagnostic_AndDoesntGenerate()
    {
        const string input = """
                             using StronglyTypedIds;
                             [assembly:StronglyTypedIdDefaults("some-template")]

                             [StronglyTypedId]
                             public partial struct MyId {}
                             """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == UnknownTemplateDiagnostic.Id);

        return Verifier.Verify(output)
            .DisableRequireUniquePrefix()
            .UseDirectory("Snapshots")
            .UseFileName(NoIdGenerationSnapshotName);
    }
}