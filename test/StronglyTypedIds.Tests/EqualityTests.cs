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

        StructToGenerate GetStruct() =>
            new(
                name: "MyStruct",
                nameSpace: "MyNamespace",
                templateName: "Guid",
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
                templateName: "Guid",
                parent: new ParentClass(null, "class", "b", "", null, false));
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
                templateName: "Guid",
                parent: new ParentClass(null, "class", "b", "", null, false));

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
                templateName: "Guid",
                parent: new ParentClass(null, "class", "b", "", null, false));
            var diagnostics = new DiagnosticInfo(new DiagnosticDescriptor(
                    NotPartialDiagnostic.Id, NotPartialDiagnostic.Title, NotPartialDiagnostic.Message, category: Constants.Usage,
                    defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
                Location.Create("somepath.cs", new TextSpan(), new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)));

            var errors = new EquatableArray<DiagnosticInfo>(new[] { diagnostics });
            return new Result<(StructToGenerate, bool)>((instance, true), errors);
        }
    }
}