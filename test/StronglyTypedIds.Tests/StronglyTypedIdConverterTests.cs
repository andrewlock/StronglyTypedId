using System.Threading.Tasks;
using StronglyTypedIds.Diagnostics;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace StronglyTypedIds.Tests;

[UsesVerify]
public class StronglyTypedIdConverterTests
{
    readonly ITestOutputHelper _output;

    public StronglyTypedIdConverterTests(ITestOutputHelper output)
    {
        _output = output;
    }

     [Fact]
     public Task DefaultConverterIdInGlobalNamespaceWithoutDefaultsDoesntGenerateConverters()
     {
         const string input =
             """
             using StronglyTypedIds;

             [StronglyTypedId]
             public partial struct MyId {}
             
             [StronglyTypedIdConverters<MyId>]
             public partial struct MyIdConverters {}
             """;
         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

         Assert.Contains(diagnostics, diagnostic => diagnostic.Id == MissingDefaultsDiagnostic.Id);

         return Verifier.Verify(output)
             .UseDirectory("Snapshots");
     }

     [Fact]
     public Task CanGenerateDefaultConverterIdInGlobalNamespace()
     {
         const string input =
             """
             using StronglyTypedIds;
             [assembly:StronglyTypedIdConvertersDefaults("guid-dapper")]

             [StronglyTypedId]
             public partial struct MyId {}
             
             [StronglyTypedIdConverters<MyId>]
             public partial struct MyIdConverters {}
             """;
         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

         Assert.Empty(diagnostics);

         return Verifier.Verify(output)
             .UseDirectory("Snapshots");
     }
//
//     [Theory]
//     [InlineData("")]
//     [InlineData("Template.Guid")]
//     [InlineData("template: Template.Guid")]
//     public Task CanGenerateIdInNamespace(string template)
//     {
//         var input = $$"""
//                       using StronglyTypedIds;
//                       namespace SomeNamespace
//                       {
//                           [StronglyTypedId({{template}})]
//                           public partial struct MyId {}
//                       }
//                       """;
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .DisableRequireUniquePrefix()
//             .UseDirectory("Snapshots");
//     }
//
//     [Theory]
//     [InlineData("Template.Int")]
//     [InlineData("template: Template.Int")]
//     public Task CanGenerateNonDefaultIdInNamespace(string template)
//     {
//         var input = $$"""
//                       using StronglyTypedIds;
//                       namespace SomeNamespace
//                       {
//                           [StronglyTypedId({{template}})]
//                           public partial struct MyId {}
//                       }
//                       """;
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .DisableRequireUniquePrefix()
//             .UseDirectory("Snapshots");
//     }
//
//     [Theory]
//     [InlineData("\"newid-full\"")]
//     [InlineData("templateNames: \"newid-full\"")]
//     public Task CanGenerateForCustomTemplate(string templateName)
//     {
//         var input = $$"""
//                       using StronglyTypedIds;
//                       namespace SomeNamespace
//                       {
//                           [StronglyTypedId({{templateName}})]
//                           public partial struct MyId {}
//                       }
//                       """;
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .DisableRequireUniquePrefix()
//             .UseDirectory("Snapshots");
//     }
//
//     [Fact]
//     public Task CanGenerateIdInFileScopedNamespace()
//     {
//         const string input = @"using StronglyTypedIds;
//
// namespace SomeNamespace;
// [StronglyTypedId]
// public partial struct MyId {}";
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .UseDirectory("Snapshots");
//     }
//
//     [Fact]
//     public Task CanGenerateNestedIdInFileScopeNamespace()
//     {
//         const string input = @"using StronglyTypedIds;
//
// namespace SomeNamespace;
//
// public class ParentClass
// {
//     [StronglyTypedId]
//     public partial struct MyId {}
// }";
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .UseDirectory("Snapshots");
//     }
//
//     [Fact]
//     public Task CanGenerateVeryNestedIdInFileScopeNamespace()
//     {
//         const string input = @"using StronglyTypedIds;
//
// namespace SomeNamespace;
//
// public partial class ParentClass
// {
//     internal partial record InnerClass
//     {
//         public readonly partial record struct InnerStruct
//         {
//             [StronglyTypedId]
//             public readonly partial struct MyId {}
//         }
//     }
// }";
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .UseDirectory("Snapshots");
//     }
//
//     [Fact]
//     public Task CanGenerateGenericVeryNestedIdInFileScopeNamespace()
//     {
//         const string input = @"using StronglyTypedIds;
//
// namespace SomeNamespace;
//
// public class ParentClass<T> 
//     where T: new() 
// {
//     internal record InnerClass<TKey, TValue>
//     {
//         public struct InnerStruct
//         {
//             [StronglyTypedId]
//             public partial struct MyId {}
//         }
//     }
// }";
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .UseDirectory("Snapshots");
//     }
//
//     [Theory]
//     [InlineData("Template.Int")]
//     [InlineData("template: Template.Int")]
//     public Task CanOverrideDefaultsWithTemplateUsingGlobalAttribute(string template)
//     {
//         var input = $$"""
//                       using StronglyTypedIds;
//                       [assembly:StronglyTypedIdDefaults({{template}})]
//
//                       [StronglyTypedId]
//                       public partial struct MyId {}
//                       """;
//
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .DisableRequireUniquePrefix()
//             .UseDirectory("Snapshots");
//     }
//
//     [Theory]
//     [InlineData("\"newid-full\"")]
//     [InlineData("templateName: \"newid-full\"")]
//     public Task CanOverrideDefaultsWithCustomTemplateUsingGlobalAttribute(string templateNames)
//     {
//         var input = $$"""
//                       using StronglyTypedIds;
//                       [assembly:StronglyTypedIdDefaults({{templateNames}})]
//
//                       [StronglyTypedId]
//                       public partial struct MyId {}
//                       """;
//
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .DisableRequireUniquePrefix()
//             .UseDirectory("Snapshots");
//     }
//
//     [Fact]
//     public Task CanGenerateMultipleIdsWithSameName()
//     {
//         // https://github.com/andrewlock/StronglyTypedId/issues/74
//         const string input = @"using StronglyTypedIds;
//
// namespace MyContracts.V1
// {
//     [StronglyTypedId]
//     public partial struct MyId {}
// }
//
// namespace MyContracts.V2
// {
//     [StronglyTypedId]
//     public partial struct MyId {}
// }";
//
//         // This only includes the last ID but that's good enough for this
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .UseDirectory("Snapshots");
//     }
//
//     [Theory]
//     [InlineData(false, "Template.Guid, \"guid-efcore\", \"guid-dapper\"")]
//     [InlineData(false, "template: Template.Guid, \"guid-efcore\", \"guid-dapper\"")]
//     [InlineData(false, "Template.Guid, templateNames: new [] {\"guid-efcore\", \"guid-dapper\"}")]
//     [InlineData(true, "Template.Guid, \"guid-efcore\", \"guid-dapper\"")]
//     [InlineData(true, "template: Template.Guid, \"guid-efcore\", \"guid-dapper\"")]
//     [InlineData(true, "Template.Guid, templateNames: new [] {\"guid-efcore\", \"guid-dapper\"}")]
//     public Task CanGenerateMultipleTemplatesWithBuiltIn(bool useDefault, string template)
//     {
//         var defaultAttr = useDefault
//             ? $"[assembly:StronglyTypedIdDefaults({template})]"
//             : string.Empty;
//
//         var templateAttr = useDefault ? string.Empty : template;
//                 
//         var input = $$"""
//                       using StronglyTypedIds;
//                       {{defaultAttr}}
//
//                       [StronglyTypedId({{templateAttr}})]
//                       public partial struct MyId {}
//                       """;
//
//         // This only includes the last ID but that's good enough for this
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .DisableRequireUniquePrefix()
//             .UseDirectory("Snapshots");
//     }
//
//     [Theory]
//     [InlineData(false, "\"guid-efcore\", \"guid-dapper\"")]
//     [InlineData(false, "templateNames: new [] {\"guid-efcore\", \"guid-dapper\"}")]
//     [InlineData(true, "\"guid-dapper\", \"guid-efcore\"")]
//     [InlineData(true, "\"guid-dapper\", new [] {\"guid-efcore\"}")]
//     [InlineData(true, "\"guid-dapper\", templateNames: new [] {\"guid-efcore\"}")]
//     [InlineData(true, "templateName: \"guid-dapper\", templateNames: new [] {\"guid-efcore\"}")]
//     public Task CanGenerateMultipleTemplatesWithoutBuiltIn(bool useDefault, string template)
//     {
//         var defaultAttr = useDefault
//             ? $"[assembly:StronglyTypedIdDefaults({template})]"
//             : string.Empty;
//
//         var templateAttr = useDefault ? string.Empty : template;
//                 
//         var input = $$"""
//                       using StronglyTypedIds;
//                       {{defaultAttr}}
//
//                       [StronglyTypedId({{templateAttr}})]
//                       public partial struct MyId {}
//                       """;
//
//         // This only includes the last ID but that's good enough for this
//         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);
//
//         Assert.Empty(diagnostics);
//
//         return Verifier.Verify(output)
//             .DisableRequireUniquePrefix()
//             .UseDirectory("Snapshots");
//     }
}