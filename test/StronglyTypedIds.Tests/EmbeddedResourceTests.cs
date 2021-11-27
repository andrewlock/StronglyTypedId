using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace StronglyTypedIds.Tests
{
    [UsesVerify]
    public class EmbeddedResourceTests
    {
        public static TheoryData<string> EmbeddedResources { get; } = new()
        {
            "StronglyTypedIdAttribute",
            "StronglyTypedIdDefaultsAttribute",
            "StronglyTypedIdBackingType",
            "StronglyTypedIdConverter",
            "StronglyTypedIdImplementations",
        };

        [Theory]
        [MemberData(nameof(EmbeddedResources))]
        public Task EmittedResourceIsSameAsCompiledResource(string resource)
        {
            var embedded = EmbeddedSources.LoadTemplateForEmitting(resource);

            return Verifier.Verify(embedded)
                .UseDirectory("Snapshots")
                .UseParameters(resource);
        }
    }
}