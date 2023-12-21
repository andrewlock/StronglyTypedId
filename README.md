# StronglyTypedId

![StronglyTypedId logo](https://raw.githubusercontent.com/andrewlock/StronglyTypedId/master/logo.png)

![Build status](https://github.com/andrewlock/StronglyTypedId/actions/workflows/BuildAndPack.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/StronglyTypedId.svg)](https://www.nuget.org/packages/StronglyTypedId/)

StronglyTypedId makes creating strongly-typed IDs as easy as adding an attribute! No more [accidentally passing arguments in the wrong order to methods](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/#an-example-of-the-problem) - StronglyTypedId uses .NET 7+'s compile-time incremental source generators to generate [the boilerplate](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#a-full-example-implementation) required to use strongly-typed IDs.

Simply, [install the required package](#installing) add the `[StronglyTypedId]` attribute to a `struct` (in the `StronglyTypedIds` namespace):

```csharp
using StronglyTypedIds;
 
[StronglyTypedId] // <- Add this attribute to auto-generate the rest of the type
public partial struct FooId { }
```

and the source generator magically generates the backing code when you save the file! Use _Go to Definition_ to see the generated code:

<img src="https://raw.githubusercontent.com/andrewlock/StronglyTypedId/master/docs/strongly_typed_id.gif" alt="Generating a strongly-typed ID using the StronglyTypedId packages"/>

> StronglyTypedId requires [the .NET Core SDK v7.0.100 or greater](https://dotnet.microsoft.com/download/dotnet/7.0).


## Why do I need this library?

I have [written a blog-post series](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/) on strongly-typed IDs that explains the issues and rational behind this library. For a detailed view, I suggest starting there, but I provide a brief introduction here.

This library is designed to tackle a specific instance of [_primitive obsession_](https://lostechies.com/jimmybogard/2007/12/03/dealing-with-primitive-obsession/), whereby we use primitive objects (`Guid`/`string`/`int`/`long` etc) to represent the IDs of domain objects. The problem is that these IDs are all interchangeable - an order ID can be assigned to a product ID, despite the fact that is likely nonsensical from the domain point of view. [See here for a more concrete example](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/#an-example-of-the-problem).

By using strongly-typed IDs, we give each ID its own `Type` which _wraps_ the underlying primitive value. This ensures you can only use the ID where it makes sense: `ProductId`s can only be assigned to products, or you can only search for products using a `ProductId`, not an `OrderId`.

Unfortunately, taking this approach requires [a lot of boilerplate and ceremony](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#a-full-example-implementation) to make working with the IDs manageable. This library abstracts all that away from you, by generating the boilerplate at build-time by using a Roslyn-powered code generator.

## Requirements

The StronglyTypedId NuGet package is a .NET Standard 2.0 package. 

You must be using the .NET 7+ SDK (though you can compile for other target frameworks like .NET Core 2.1 and .NET Framework 4.8)

## Installing

To use StronglyTypedIds, install the [StronglyTypedId NuGet package](https://www.nuget.org/packages/StronglyTypedId) into your _csproj_ file, for example by running

```bash
dotnet add package StronglyTypedId --version 1.0.0-beta07
```

This adds a `<PackageReference>` to your project. You can additionally mark the package as `PrivateAsets="all"` and `ExcludeAssets="runtime"`.

> Setting `PrivateAssets="all"` means any projects referencing this one will not also get a reference to the _StronglyTypedId_ package. Setting `ExcludeAssets="runtime"` ensures the _StronglyTypedId.Attributes.dll_ file is not copied to your build output (it is not required at runtime).

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <!-- Add the package -->
    <PackageReference Include="StronglyTypedId" Version="1.0.0-beta07" PrivateAssets="all" ExcludeAssets="runtime" />
    <!-- -->
  </ItemGroup>

</Project>
```

## Usage

To create a strongly-typed ID, create a `partial struct` with the desired name, and decorate it with the `[StronglyTypedId]` attribute, in the `StronglyTypedIds` namespace:

```csharp
using StronglyTypedIds;

[StronglyTypedId] // Add this attribute to auto-generate the rest of the type
public partial struct FooId { }
```

This generates the "default" strongly-typed ID using a `Guid` backing field. You can use your IDE's _Go to Definition_ functionality on your ID to see the_exact code generated by the source generator. The ID implements the following interfaces automatically:

- `IComparable<T>`
- `IEquatable<T>`
- `IFormattable`
- `ISpanFormattable` (.NET 6+)
- `IParsable<T>` (.NET 7+)
- `ISpanParsable<T>` (.NET 7+)
- `IUtf8SpanFormattable` (.NET 8+)
- `IUtf8SpanParsable<T>` (.NET 8+)

And it additionally includes two converters/serializers:

- `System.ComponentModel.TypeConverter`
- `System.Text.Json.Serialization.JsonConverter`

This provides basic integration for many use cases, but you may want to customize the IDs further, as you'll see shortly.

### Using different types as a backing fields

The default strongly-typed ID uses a `Guid` backing field:

```csharp
using StronglyTypedIds;

[StronglyTypedId]
public partial struct FooId { }

var id = new FooId(Guid.NewGuid());
```

You can choose a different type backing field, by passing a value of the `Template` enum in the constructor. 

```csharp
using StronglyTypedIds;

[StronglyTypedId(Template.Int)]
public partial struct FooId { }

var id = new FooId(123);
```

Currently supported built-in backing types are:

- `Guid` (the default)
- `int`
- `long`
- `string`

### Changing the defaults globally

If you wish to change the template used by default for _all_ the `[StronglyTypedId]`-decorated IDs in your project, you can use the assembly attribute `[StronglyTypedIdDefaults]` to set all of these. For example, the following changes the default backing-type for all IDs to `int` 

```csharp
// Set the defaults for the project
[assembly:StronglyTypedIdDefaults(Template.Int)]

[StronglyTypedId] // Uses the default 'int' template
public partial struct OrderId { }

[StronglyTypedId] // Uses the default 'int' template
public partial struct UserId { } 

[StronglyTypedId(Template.Guid)] // Overrides the default to use 'Guid' template
public partial struct HostId { } 
```

### Using custom templates

In addition to the built-in templates, you can provide your _own_ templates for use with strongly typed IDs. To do this, do the following:

- Add a file to your project with the name _TEMPLATE.typedid_, where `TEMPLATE` is the name of the template
- Update the template with your desired ID content. Use `PLACEHOLDERID` inside the template. This will be replaced with the ID's name when generating the template.
- Update the "build action" for the template to `AdditionalFiles` or `C# analyzer additional file` (depending on your IDE).

For example, you could create a template that provides an EF Core `ValueConverter` implementation called _guid-efcore.typedid_ like this:

```csharp
partial struct PLACEHOLDERID
{
    public class EfCoreValueConverter : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<PLACEHOLDERID, global::System.Guid>
    {
        public EfCoreValueConverter() : this(null) { }
        public EfCoreValueConverter(global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints? mappingHints = null)
            : base(
                id => id.Value,
                value => new PLACEHOLDERID(value),
                mappingHints
            ) { }
    }
}
```

> Note that the content of the _guid-efcore.typedid_ file is valid C#. One easy way to author these templates is to create a _.cs_ file containing the code you want for your ID, then rename your ID to `PLACEHOLDERID`, change the file extension from _.cs_ to _.typedid, and then set the build action.

After creating a template in your project you can apply it to your IDs like this:

```csharp
// Use the built-in Guid template and also the custom template
[StronglyTypedId(Template.Guid, "guid-efcore")] 
public partial struct GuidId {}
```

This shows another important feature: you can specify _multiple_ templates to use when generating the ID.

### Using multiple templates

When specifying the templates for an ID, you can specify

- 0 or 1 built-in templates (using `Template.Guid` etc)
- 0 or more custom templates

For example:

```csharp
[StronglyTypedId] // Use the default templates
public partial struct MyDefaultId {}

[StronglyTypedId(Template.Guid)] // Use a built-in template only
public partial struct MyId1 {}

[StronglyTypedId("my-guid")] // Use a custom template only
public partial struct MyId2 {}

[StronglyTypedId("my-guid", "guid-efcore")] // Use multiple custom templates
public partial struct MyId2 {}

[StronglyTypedId(Template.Guid, "guid-efcore")] // Use a built-in template _and_ a custom template
public partial struct MyId3 {}

// Use a built-in template _and_ multiple custom template
[StronglyTypedId(Template.Guid, "guid-efcore", "guid-dapper")]
public partial struct MyId4 {}
```

Similarly, for the optional `[StronglyTypedIdDefaults]` assembly attribute, which defines the _default_ templates to use when you use the raw `[StronglyTypedId]` attribute, you use a combination of built-in and/or custom templates:

```csharp
//⚠ You can only use _one_ of these in your project, they're all shown here for comparison

[assembly:StronglyTypedIdDefaults(Template.Guid)] // Use a built-in template only

[assembly:StronglyTypedIdDefaults("my-guid")] // Use a custom template only

[assembly:StronglyTypedIdDefaults("my-guid", "guid-efcore")] // Use multiple custom templates

[assembly:StronglyTypedIdDefaults(Template.Guid, "guid-efcore")] // Use a built-in template _and_ a custom template

// Use a built-in template _and_ multiple custom template
[assembly:StronglyTypedIdDefaults(Template.Guid, "guid-efcore", "guid-dapper")]

[StronglyTypedId] // Uses whatever templates were specified!
public partial struct MyDefaultId {}
```

To simplify the creation of templates, the _StronglyTypedId_ package includes a code-fix provider to generate a template.

## Creating a custom template with the Roslyn CodeFix provider

As well as the source generator, the _StronglyTypedId_ NuGet package includes a CodeFix provider that looks for cases where you have specified a custom template that the source generator cannot find. For example, in the following code,the `"some-int"` template does not yet exist:

```csharp
[StronglyTypedId("some-int")] // does not exist
public partial struct MyStruct { }
```

In the IDE, you can see the generator has marked this as an error:

![An error is shown when the template does not exist](https://github.com/andrewlock/StronglyTypedId/assets/18755388/2a0ed4ce-0c0b-4508-b2c0-46ba7b756b8e)

The image above also shows that there's a CodeFix action available. Clicking the action reveals the possible fix: **Add some-int.typedid template to the project**, and shows a preview of the file that will be added:

![Showing the CodeFix in action, suggesting you can add a project](https://github.com/andrewlock/StronglyTypedId/assets/18755388/ffd62acd-3ea9-448b-adc7-5255cae651c3)

Choosing this option will add the template to your project. 

> Unfortunately, [due to limitations with the Roslyn APIs](https://github.com/dotnet/roslyn/issues/4655), it's not possible to add the new template with the required **AdditionalFiles**/**C# analyzer additional file** build action already set. Until you change the build-action, the error will remain on your `[StronglyTypedId]` attribute. 

Right-click the newly-added template, choose **Properties**, and change the **Build Action** to either **C# analyzer additional file** (Visual Studio 2022) or **AdditionalFiles** (JetBrains Rider). The source generator will then detect your template and the error will disappear. 

The CodeFix provider does a basic check against the name of the template you're trying to create. If it includes `int`, `long`, or `string`, the template it creates will be based on one of those backing types. Otherwise, the template is based on a `Guid` backing type.

Once the template is created, you're free to edit it as required.

## "Community" templates package _StronglyTypedId.Templates_

The "template-based" design of StronglyTypedId is intended to make it easy to get started, while also giving you the flexibility to customise your IDs to your needs.

To make it easier to share templates with multiple people, and optional _StronglyTypedId.Templates_ NuGet package is available that includes various converters and other backing types. To use these templates, add the _StronglyTypedId.Templates_ package to your project:

```bash
dotnet add package StronglyTypedId.Templates --version 1.0.0-beta07
```

You will then be able to reference any of the templates it includes. This includes "complete" implementations, including multiple converters, for various backing types:


- `guid-full`
- `int-full`
- `long-full`
- `string-full`
- `nullablestring-full`
- `newid-full`


It also includes "standalone" EF Core, Dapper, and Newtonsoft JSON converter templates to enhance the `Guid`/`int`/`long`/`string` built-in templates. For example

- Templates for use with `Template.Guid`
  - `guid-dapper` 
  - `guid-efcore`
  - `guid-newtonsoftjson`
- Templates for use with `Template.Int`
  - `int-dapper`
  - `int-efcore`
  - `int-newtonsoftjson`
- Templates for use with `Template.Long`
  - `long-dapper`
  - `long-efcore`
  - `long-newtonsoftjson`
- Templates for use with `Template.String`
  - `string-dapper`
  - `string-efcore`
  - `string-newtonsoftjson`

For the full list of available templates, [see GitHub](https://github.com/andrewlock/StronglyTypedId/tree/master/src/StronglyTypedIds.Templates). 

You can also create your own templates package and distribute it on NuGet.

## Embedding the attributes in your project

By default, the `[StronglyTypedId]` attributes referenced in your application are contained in an external dll. It is also possible to embed the attributes directly in your project, so they appear in the dll when your project is built. If you wish to do this, you must do two things:

1. Define the MSBuild constant `STRONGLY_TYPED_ID_EMBED_ATTRIBUTES`. This ensures the attributes are embedded in your project
2. Add `compile` to the list of excluded assets in your `<PackageReference>` element. This ensures the attributes in your project are referenced, instead of the _StronglyTypedId.Attributes.dll_ library.

Your project file should look something like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <!--  Define the MSBuild constant    -->
    <DefineConstants>STRONGLY_TYPED_ID_EMBED_ATTRIBUTES</DefineConstants>
  </PropertyGroup>

  <!-- Add the package -->
  <PackageReference Include="StronglyTypedId" Version="1.0.0-beta07" 
                    PrivateAssets="all"
                    ExcludeAssets="compile;runtime" />
<!--                               ☝ Add compile to the list of excluded assets. -->

</Project>
```

## Preserving usages of the `[StronglyTypedId]` attribute

The `[StronglyTypedId]` and `[StronglyTypedIdDefaults]` attributes are decorated with the `[Conditional]` attribute, [so their usage will not appear in the build output of your project](https://andrewlock.net/conditional-compilation-for-ignoring-method-calls-with-the-conditionalattribute/#applying-the-conditional-attribute-to-classes). If you use reflection at runtime on one of your IDs, you will not find `[StronglyTypedId]` in the list of custom attributes.

If you wish to preserve these attributes in the build output, you can define the `STRONGLY_TYPED_ID_USAGES` MSBuild variable. Note that this means your project will have a runtime-dependency on _StronglyTypedId.Attributes.dll_ so you need to ensure this is included in your build output.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <!--  Define the MSBuild constant to preserve usages   -->
    <DefineConstants>STRONGLY_TYPED_ID_USAGES</DefineConstants>
  </PropertyGroup>

  <!-- Add the package -->
  <PackageReference Include="StronglyTypedId" Version="1.0.0-beta07" PrivateAssets="all" />
  <!--              ☝ You must not exclude the runtime assets in this case -->

</Project>
```

## Error CS0436 and [InternalsVisibleTo]

> In the latest version of StronglyTypedId, you should not experience error CS0436 by default. 

In previous versions of the StronglyTypedId generator, the `[StronglyTypedId]` attributes were added to your compilation as `internal` attributes by default. If you added the source generator package to multiple projects, and used the `[InternalsVisibleTo]` attribute, you could experience errors when you build:

```bash
warning CS0436: The type 'StronglyTypedIdImplementations' in 'StronglyTypedIds\StronglyTypedIds.StronglyTypedIdGenerator\StronglyTypedIdImplementations.cs' conflicts with the imported type 'StronglyTypedIdImplementations' in 'MyProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'.
```

In the latest version of _StronglyTypedId_, the attributes are not embedded by default, so you should not experience this problem. If you see this error, compare your installation to the examples in the installation guide.