using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using StronglyTypedIds.Diagnostics;
using Xunit;

namespace StronglyTypedIds.Tests;

public class EqualityTests
{
    [Fact]
    public void ParentClassHasExpectedEqualityBehaviour()
    {
        var instance1 = GetParentClass();
        var instance2 = GetParentClass();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        ParentClass GetParentClass() => new("struct", "TestName", "where T : class", null);
    }

    [Fact]
    public void ParentClassWithParentHasExpectedEqualityBehaviour()
    {
        var instance1 = GetParentClass();
        var instance2 = GetParentClass();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        ParentClass GetParentClass() => new("struct", "TestName", "where T : class", new ParentClass("class", "b", "", null));
    }

    [Fact]
    public void StronglyTypedIdConfigurationHasExpectedEqualityBehaviour()
    {
        var id1 = GetId();
        var id2 = GetId();
        Assert.True(id1.Equals(id2));
        Assert.Equal(id1, id2);
    }

    private static StronglyTypedIdConfiguration GetId()
    {
        return new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.Default, StronglyTypedIdImplementations.Default);
    }

    [Fact]
    public void StructToGenerateHasExpectedEqualityBehaviour()
    {
        var instance1 = GetStruct();
        var instance2 = GetStruct();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        StructToGenerate GetStruct() =>
            new(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                config: new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.Default, StronglyTypedIdImplementations.Default),
                parent: null);
    }

    [Fact]
    public void StructToGenerateWithParentHasExpectedEqualityBehaviour()
    {
        var instance1 = GetStruct();
        var instance2 = GetStruct();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        StructToGenerate GetStruct()
        {
            return new StructToGenerate(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                config: new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.Default, StronglyTypedIdImplementations.Default),
                parent: new ParentClass("class", "b", "", null));
        }
    }
    
    [Fact]
    public void ResultWithoutDiagnosticHasExpectedEqualityBehaviour()
    {
        var instance1 = GetResult();
        var instance2 = GetResult();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        static Result<(StructToGenerate, bool)> GetResult()
        {
            var instance = new StructToGenerate(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                config: new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.Default, StronglyTypedIdImplementations.Default),
                parent: new ParentClass("class", "b", "", null));

            return new Result<(StructToGenerate, bool)>((instance, true), new EquatableArray<DiagnosticInfo>());
        }
    }

    [Fact]
    public void ResultWithDiagnosticHasExpectedEqualityBehaviour()
    {
        var instance1 = GetResult();
        var instance2 = GetResult();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        static Result<(StructToGenerate, bool)> GetResult()
        {
            var instance = new StructToGenerate(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                config: new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.Default, StronglyTypedIdImplementations.Default),
                parent: new ParentClass("class", "b", "", null));
            var diagnostics = new DiagnosticInfo(new DiagnosticDescriptor(
                    InvalidBackingTypeDiagnostic.Id, InvalidBackingTypeDiagnostic.Title, InvalidBackingTypeDiagnostic.Message, category: Constants.Usage,
                    defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
                Location.Create("somepath.cs", new TextSpan(), new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)));

            var errors = new EquatableArray<DiagnosticInfo>(new[] { diagnostics });
            return new Result<(StructToGenerate, bool)>((instance, true), errors);
        }
    }
}