using System.Threading.Tasks;
using StronglyTypedIds.Diagnostics;
using VerifyXunit;
using Xunit;

namespace StronglyTypedIds.Tests.Diagnostics;

public class DiagnosticsTests
{
    [Theory]
    [InlineData("null")]
    [InlineData("\"\"")]
    [InlineData("\"       \"")]
    public void EmptyTemplateGivesInvalidTemplateNameDiagnostic(string template)
    {
        var input = $$"""
                      using StronglyTypedIds;

                      [StronglyTypedId({{template}})]
                      public partial struct MyId {}
                      """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == InvalidTemplateNameDiagnostic.Id);
    }
}