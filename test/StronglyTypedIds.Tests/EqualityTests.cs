using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using StronglyTypedIds.Diagnostics;
using Xunit;

namespace StronglyTypedIds.Tests;

public class EqualityTests
{
    private static LocationInfo _templateLocation = new("Some path", new TextSpan(0, 100), new LinePositionSpan(new LinePosition(23, 2), new LinePosition(23, 15)));

    [Fact]
    public void ParentClassHasExpectedEqualityBehaviour()
    {
        var instance1 = GetParentClass();
        var instance2 = GetParentClass();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        ParentClass GetParentClass() => new(null, "struct", "TestName", "where T : class", null, false);
    }

    [Fact]
    public void ParentClassWithParentHasExpectedEqualityBehaviour()
    {
        var instance1 = GetParentClass();
        var instance2 = GetParentClass();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        ParentClass GetParentClass() => new(null, "struct", "TestName", "where T : class", new ParentClass(null, "class", "b", "", null, false), false);
    }

    [Fact]
    public void StructToGenerateHasExpectedEqualityBehaviour()
    {
        var instance1 = GetStruct();
        var instance2 = GetStruct();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        TypeToGenerate GetStruct() =>
            new(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                template: Template.Guid,
                null,
                parent: null,
                _templateLocation);
    }
    
    [Fact]
    public void StructToGenerateWithTemplateAndLocationHasExpectedEqualityBehaviour()
    {
        var instance1 = GetStruct();
        var instance2 = GetStruct();

        Assert.Equal(instance1, instance2);
        Assert.True(instance1.Equals(instance2));
        Assert.True(instance1 == instance2);

        TypeToGenerate GetStruct() =>
            new(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                template: Template.Int,
                templateNames: new[] {"Guid"},
                templateLocation: _templateLocation,
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

        TypeToGenerate GetStruct()
        {
            return new TypeToGenerate(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                template: Template.Guid,
                templateNames: null,
                parent: new ParentClass(null, "class", "b", "", null, false),
                _templateLocation);
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

        static Result<(TypeToGenerate, bool)> GetResult()
        {
            var instance = new TypeToGenerate(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                template: Template.Guid,
                templateNames: null,
                parent: new ParentClass(null, "class", "b", "", null, false),
                _templateLocation);

            return new Result<(TypeToGenerate, bool)>((instance, true), new EquatableArray<DiagnosticInfo>());
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

        static Result<(TypeToGenerate, bool)> GetResult()
        {
            var instance = new TypeToGenerate(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                template: Template.Guid,
                templateNames: null,
                parent: new ParentClass(null, "class", "b", "", null, false),
                _templateLocation);
            var diagnostics = new DiagnosticInfo(new DiagnosticDescriptor(
                    NotPartialDiagnostic.Id, NotPartialDiagnostic.Title, NotPartialDiagnostic.Message, category: Constants.Usage,
                    defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
                Location.Create("somepath.cs", new TextSpan(), new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)));

            var errors = new EquatableArray<DiagnosticInfo>(new[] { diagnostics });
            return new Result<(TypeToGenerate, bool)>((instance, true), errors);
        }
    }
}