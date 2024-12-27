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

        StructToGenerate GetStruct() =>
            new(
                DeclarationKind.Struct,
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

        StructToGenerate GetStruct() =>
            new(
                DeclarationKind.Struct,
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

        StructToGenerate GetStruct()
        {
            return new StructToGenerate(
                DeclarationKind.Struct,
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

        static Result<(StructToGenerate, bool)> GetResult()
        {
            var instance = new StructToGenerate(
                DeclarationKind.Struct,
                name: "MyStruct",
                nameSpace: "MyNamespace",
                template: Template.Guid,
                templateNames: null,
                parent: new ParentClass(null, "class", "b", "", null, false),
                _templateLocation);

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
                DeclarationKind.Struct,
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
            return new Result<(StructToGenerate, bool)>((instance, true), errors);
        }
    }

    [Fact]
    public void EquatableArray_PrimitiveComparison()
    {
        int[] val1 = [1, 2, 3, 4, 5];
        int[] val2 = [1, 2, 3, 4, 5];

        var arr1 = new EquatableArray<int>(val1);
        var arr2 = new EquatableArray<int>(val2);

        Assert.True(arr1.Equals(arr2));
    }

    [Fact]
    public void EquatableArray_RecordComparison()
    {
        Record[] val1 = [new(1), new(2), new(3), new(4), new(5)];
        Record[] val2 = [new(1), new(2), new(3), new(4), new(5)];

        var arr1 = new EquatableArray<Record>(val1);
        var arr2 = new EquatableArray<Record>(val2);

        Assert.True(arr1.Equals(arr2));
    }

    [Fact]
    public void EquatableArray_NestedEquatableArrayComparison()
    {
        EquatableArray<int>[] val1 = [new([1]), new([2]), new([3]), new([4]), new([5])];
        EquatableArray<int>[] val2 = [new([1]), new([2]), new([3]), new([4]), new([5])];

        var arr1 = new EquatableArray<EquatableArray<int>>(val1);
        var arr2 = new EquatableArray<EquatableArray<int>>(val2);

        Assert.True(arr1.Equals(arr2));
    }

    [Fact]
    public void EquatableArray_BoxedNestedEquatableArrayComparison()
    {
        EquatableArray<int>[] val1 = [new([1]), new([2]), new([3]), new([4]), new([5])];
        EquatableArray<int>[] val2 = [new([1]), new([2]), new([3]), new([4]), new([5])];

        object arr1 = new EquatableArray<EquatableArray<int>>(val1);
        var arr2 = new EquatableArray<EquatableArray<int>>(val2);

        Assert.True(arr1.Equals(arr2));
        Assert.True(arr2.Equals(arr1));
    }

    public record Record
    {
        public Record(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}