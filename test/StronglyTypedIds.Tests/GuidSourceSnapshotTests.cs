using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace StronglyTypedIds.Tests
{
    [UsesVerify]
    public class GuidGeneratorSnapshotTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(StronglyTypedIdJsonConverter.NewtonsoftJson)]
        [InlineData(StronglyTypedIdJsonConverter.SystemTextJson)]
        [InlineData(StronglyTypedIdJsonConverter.NewtonsoftJson | StronglyTypedIdJsonConverter.SystemTextJson)]
        public Task GeneratesGuidCorrectly(StronglyTypedIdJsonConverter? converter)
        {
            const string idNamespace = "Some.Namespace";
            const string idName = "MyTestId";
            var result = Sources.CreateGuidId(
                idName: idName,
                idNamespace: idNamespace,
                jsonConverter: converter
            );

            return Verifier.Verify(result)
                .UseParameters(converter);
        }
    }
}