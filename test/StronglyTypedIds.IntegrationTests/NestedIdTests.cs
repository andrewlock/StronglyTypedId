using StronglyTypedIds.IntegrationTests.Types;
using Xunit;

namespace StronglyTypedIds.IntegrationTests;

public class NestedIdTests
{
    [Fact]
    public void CanCreateNestedId()
    {
        var id = SomeType<object>.NestedType<string, int>.MoreNesting.VeryNestedId.New();
    }
}