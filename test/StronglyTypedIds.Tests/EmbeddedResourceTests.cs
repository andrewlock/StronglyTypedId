namespace StronglyTypedIds.Tests
{
    [UsesVerify]
    public class EmbeddedResourceTests
    {
        public static TheoryData<string> EmbeddedResources { get; } = new()
        {
            "StronglyTypedIdAttribute",
            "StronglyTypedIdDefaultsAttribute",
            "Template",
        };

        [Theory]
        [MemberData(nameof(EmbeddedResources))]
        public Task EmittedResourceIsSameAsCompiledResource(string resource)
        {
            var embedded = EmbeddedSources.LoadAttributeTemplateForEmitting(resource);

            return Verifier.Verify(embedded)
                .UseDirectory("Snapshots")
                .UseParameters(resource);
        }
    }
}