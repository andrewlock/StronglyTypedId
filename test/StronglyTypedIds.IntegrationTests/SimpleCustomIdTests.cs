using StronglyTypedIds.IntegrationTests.Types;
using Xunit;

namespace StronglyTypedIds.IntegrationTests;

public class SimpleCustomIdTests
{
    [Fact]
    public void SameValuesAreEqual()
    {
        var id = 123;
        var foo1 = new SimpleCustomId(id);
        var foo2 = new SimpleCustomId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new SimpleCustomId(1);
        var foo2 = new SimpleCustomId(2);

        Assert.NotEqual(foo1, foo2);
    }
}