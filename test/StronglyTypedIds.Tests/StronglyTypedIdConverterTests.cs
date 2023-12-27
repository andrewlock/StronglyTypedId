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

     [Theory]
     [InlineData("", true)]
     [InlineData("(\"guid-dapper\")", true)]
     [InlineData("(\"guid-dapper\")", false)]
     public Task CanGenerateDefaultConverterIdInGlobalNamespace(string template, bool includeDefaults)
     {
         var attribute = includeDefaults
             ? "[assembly:StronglyTypedIdConvertersDefaults(\"guid-dapper\")]"
             : string.Empty;

         var input =
             $$"""
             using StronglyTypedIds;
             {{attribute}}

             [StronglyTypedId]
             public partial struct MyId {}
             
             [StronglyTypedIdConverters<MyId>{{template}}]
             public partial struct MyIdConverters {}
             """;
         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

         Assert.Empty(diagnostics);

         return Verifier.Verify(output)
             .DisableRequireUniquePrefix()
             .UseDirectory("Snapshots");
     }

     [Theory]
     [InlineData("", true)]
     [InlineData("(\"guid-dapper\")", true)]
     [InlineData("(\"guid-dapper\")", false)]
     public Task CanGenerateDefaultConverterIdInNamespace(string template, bool includeDefaults)
     {
         var attribute = includeDefaults
             ? "[assembly:StronglyTypedIdConvertersDefaults(\"guid-dapper\")]"
             : string.Empty;

         var input =
             $$"""
             using StronglyTypedIds;
             {{attribute}}

             namespace SomeNamespace
             {
                [StronglyTypedId]
                public partial struct MyId {}
             
                [StronglyTypedIdConverters<MyId>{{template}}]
                public partial struct MyIdConverters {}
             }
             """;
         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

         Assert.Empty(diagnostics);

         return Verifier.Verify(output)
             .DisableRequireUniquePrefix()
             .UseDirectory("Snapshots");
     }

     [Theory]
     [InlineData("", true)]
     [InlineData("(\"int-dapper\")", true)]
     [InlineData("(\"int-dapper\")", false)]
     public Task CanGenerateNonDefaultConverterIdInNamespace(string template, bool includeDefaults)
     {
         var attribute = includeDefaults
             ? "[assembly:StronglyTypedIdConvertersDefaults(\"int-dapper\")]"
             : string.Empty;

         var input =
             $$"""
             using StronglyTypedIds;
             {{attribute}}

             namespace SomeNamespace
             {
                [StronglyTypedId(Template.Int)]
                public partial struct MyId {}
             
                [StronglyTypedIdConverters<MyId>{{template}}]
                public partial struct MyIdConverters {}
             }
             """;
         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

         Assert.Empty(diagnostics);

         return Verifier.Verify(output)
             .DisableRequireUniquePrefix()
             .UseDirectory("Snapshots");
     }

     [Theory]
     [InlineData("", true)]
     [InlineData("(\"guid-dapper\")", true)]
     [InlineData("(\"guid-dapper\")", false)]
     public Task CanGenerateDefaultConverterIdInFileScopedNamespace(string template, bool includeDefaults)
     {
         var attribute = includeDefaults
             ? "[assembly:StronglyTypedIdConvertersDefaults(\"guid-dapper\")]"
             : string.Empty;

         var input =
             $$"""
               using StronglyTypedIds;
               {{attribute}}

               namespace SomeNamespace;
                
               [StronglyTypedId]
               public partial struct MyId {}
               
               [StronglyTypedIdConverters<MyId>{{template}}]
               public partial struct MyIdConverters {}
               """;
         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

         Assert.Empty(diagnostics);

         return Verifier.Verify(output)
             .DisableRequireUniquePrefix()
             .UseDirectory("Snapshots");
     }

     [Theory]
     [InlineData("", true)]
     [InlineData("(\"guid-dapper\")", true)]
     [InlineData("(\"guid-dapper\")", false)]
     public Task CanGenerateDefaultConverterIdInDifferentNamespace(string template, bool includeDefaults)
     {
         var attribute = includeDefaults
             ? "[assembly:StronglyTypedIdConvertersDefaults(\"guid-dapper\")]"
             : string.Empty;

         var input =
             $$"""
               using StronglyTypedIds;
               {{attribute}}

               namespace SomeNamespace1
               {
                   [StronglyTypedId]
                   public partial struct MyId {}
               }
               namespace SomeNamespace2
               {
                   using SomeNamespace1;
               
                   [StronglyTypedIdConverters<MyId>{{template}}]
                   public partial struct MyIdConverters {}
               }
               """;
         var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

         Assert.Empty(diagnostics);

         return Verifier.Verify(output)
             .DisableRequireUniquePrefix()
             .UseDirectory("Snapshots");
     }

    [Theory]
    [InlineData("", true)]
    [InlineData("(\"guid-dapper\")", true)]
    [InlineData("(\"guid-dapper\")", false)]
    public Task CanGenerateNestedIdInFileScopeNamespace(string template, bool includeDefaults)
    {
        var attribute = includeDefaults
            ? "[assembly:StronglyTypedIdConvertersDefaults(\"guid-dapper\")]"
            : string.Empty;

        var input = $$"""
                      using StronglyTypedIds;
                      {{attribute}}

                      namespace SomeNamespace;

                      public class ParentClass
                      {
                          [StronglyTypedId]
                          public partial struct MyId {}
                      }

                      public class ConverterClass
                      {
                          [StronglyTypedIdConverters<MyId>{{template}}]
                          public partial struct MyIdConverters {}
                      }
                      """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

        Assert.Empty(diagnostics);

        return Verifier.Verify(output)
            .DisableRequireUniquePrefix()
            .UseDirectory("Snapshots");
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("(\"guid-dapper\")", true)]
    [InlineData("(\"guid-dapper\")", false)]
    public Task CanGenerateMultipleConvertersWithSameName(string template, bool includeDefaults)
    {
        var attribute = includeDefaults
            ? "[assembly:StronglyTypedIdConvertersDefaults(\"guid-dapper\")]"
            : string.Empty;

        var input = $$"""
                      using StronglyTypedIds;
                      {{attribute}}

                      public class ParentClass
                      {
                          [StronglyTypedId]
                          public partial struct MyId {}
                      }

                      namespace MyContracts.V1
                      {
                          public class ConverterClass
                          {
                              [StronglyTypedIdConverters<MyId>{{template}}]
                              public partial struct MyIdConverters {}
                          }
                      }
                      
                      namespace MyContracts.V2
                      {
                          public class ConverterClass
                          {
                              [StronglyTypedIdConverters<MyId>{{template}}]
                              public partial struct MyIdConverters {}
                          }
                      }
                      """;
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

        Assert.Empty(diagnostics);

        return Verifier.Verify(output)
            .DisableRequireUniquePrefix()
            .UseDirectory("Snapshots");
    }
}