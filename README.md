# StronglyTypedId

![StronglyTypedId logo](https://raw.githubusercontent.com/andrewlock/StronglyTypedId/master/logo.png)

![Build status](https://github.com/andrewlock/StronglyTypedId/actions/workflows/BuildAndPack.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/StronglyTypedId.svg)](https://www.nuget.org/packages/StronglyTypedId/)

StronglyTypedId makes creating strongly-typed IDs as easy as adding an attribute! No more [accidentally passing arguments in the wrong order to methods](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/#an-example-of-the-problem) - StronglyTypedId uses .NET 6's compile-time incremental source generators to generate [the boilerplate](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#a-full-example-implementation) required to use strongly-typed IDs.

Simply, [install the required package](#installing) add the `[StronglyTypedId]` attribute to a `struct` (in the `StronglyTypedIds` namespace):

```csharp
using StronglyTypedIds;
 
[StronglyTypedId] // <- Add this attribute to auto-generate the rest of the type
public partial struct FooId { }
```

and the source generator magically generates the backing code when you save the file! Use _Go to Definition_ to see the generated code:

<img src="https://raw.githubusercontent.com/andrewlock/StronglyTypedId/master/docs/strongly_typed_id.gif" alt="Generating a strongly-typed ID using the StronglyTypedId packages"/>

> StronglyTypedId requires requires [the .NET Core SDK v6.0.100 or greater](https://dotnet.microsoft.com/download/dotnet/6.0).

## Changes in version 1.x

Version 0.x of this library used the helper library [CodeGeneration.Roslyn](https://github.com/AArnott/CodeGeneration.Roslyn) by [AArnott](https://github.com/AArnott), for build-time source generation. In version 1.0.0 this approach has been completely replaced in favour of source generators, as these are explicitly supported in .NET 6+. As part of this change, there were a number of additional features added and breaking changes made.

### Breaking Changes

* `StronglyTypedIds` namespace is required. In version 0.x of the library, the `[StronglyTypedId]` attribute was in the global namespace. In version 1.x, the attribute is in the `StronglyTypedIds` namespace, so you must add `namespace StronglyTypedIds;`.
* The properties exposed by `StronglyTypedIds` have changed: there is no longer a `generateJsonConverter` property. Instead, this is infered based on the `StronglyTypedIdConverters` flags provided.
* The `String` backing typed ID will throw if you call the constructor with a `null` value

### New Features

* The attributes can now auto-generate additional converter types such as EF Core `ValueConverter` and Dapper `TypeHandler`, as described in [my blog posts](https://andrewlock.net/series/using-strongly-typed-entity-ids-to-avoid-primitive-obsession/). These are optional flags on the `converters` property.
* Made interface implementations (`IEquatable<T>` and `IComparable<T>` currently) optional. This is to potentially support additional interfaces in future versions.
* Added a `NullableString` backing type. Due to the behaviour of `struct`s in c#, the `String` backing type ID _may_ still be null, but you can't explicitly call the constructor with a null value. In contrast, you can do this with the `NullableString` backing type.
* Added a `[StronglyTypedIdDefaults]` attribute to set default values for all `[StronglyTypedId]` attributes in your project. This is useful if you want to customise all the attributes, for example, if you want to generate additional converters by default. You can still override all the properties of a `[StronglyTypedId]` instance.

### Bug Fixes
 
* Some converters had incorrect implementations, such as in ([#24](https://github.com/andrewlock/StronglyTypedId/issues/24)). These have been addressed in version 1.x.
* Better null handling has been added for the `String` backing type, handling issues such as [#32](https://github.com/andrewlock/StronglyTypedId/issues/32).
* The code is marked as auto generated, to avoid errors such as #CS1591 as described in [#27](https://github.com/andrewlock/StronglyTypedId/issues/27)
* An error deserializing nullable StronglyTypedIds with Newtonsoft.Json [#36](https://github.com/andrewlock/StronglyTypedId/issues/36)

## Installing

To use the the [StronglyTypedId NuGet package](https://www.nuget.org/packages/StronglyTypedId), install the [StronglyTypedId](https://www.nuget.org/packages/StronglyTypedId) package into your project. Depending on which converters you implement, you may need one or more of the following additional packages

* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) (optional, only required if [generating a Newtonsoft `JsonConverter`](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#creating-a-custom-jsonconverter)). Note that in some ASP.NET Core apps, you will likely already reference this project via transitive dependencies.
* [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) (optional, only required if [generating a System.Text `JsonConverter`](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#creating-a-custom-jsonconverter)). Note that in .NET Core apps, you will likely already reference this project via transitive dependencies.
* [Dapper](https://www.nuget.org/packages/Dapper/) (optional, only required if [generating a type mapper](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-3/#interfacing-with-external-system-using-strongly-typed-ids))
* [EF Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore) (optional, only required if [generating an EF Core ValueConverter](https://andrewlock.net/strongly-typed-ids-in-ef-core-using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-4/))

To install the packages, add the references to your _csproj_ file, for example by running

```bash
dotnet add package StronglyTypedId --version 1.0.0-beta05
```

This adds a `<PackageReference>` to your project. As the package is marked as a development-only dependency, by default, the package is added with assets excluded.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
  
  <!-- Add the package -->
  <PackageReference Include="StronglyTypedId" Version="1.0.0-beta05">
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
  <!-- -->

</Project>
```

> Note that in the default configuration above, a _StronglyTypedId.Attributes.dll_ file will appear in your project's build output. This dll is not required, and can be removed by removing `runtime` from the default generated `<IncludeAssets>` element. 

## Usage

To create a strongly-typed ID, create a `partial struct` with the desired name, and decorate it with the `[StronglyTypedId]` attribute, in the `StronglyTypedIds` namespace:

```csharp
using StronglyTypedIds;

[StronglyTypedId] // Add this attribute to auto-generate the rest of the type
public partial struct FooId { }
```

This generates the "default" strongly-typed ID using a `Guid` backing field, a custom `TypeConverter`, and a custom `JsonConverter` based on Newtonsoft.Json. 

### Customising the converters

You can customise which converters to generate by using flags. For example, to generate a `TypeConverter`, a `System.Text.JsonConverter`, and an EF Core `ValueConverter`, use

```csharp
using StronglyTypedIds;

[StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.EfCoreValueConverter)] 
public partial struct SystemTextJsonConverterId { }
```

### Using different types as a backing fields

The default strongly-typed ID uses a `Guid` backing field:

```csharp
using StronglyTypedIds;

[StronglyTypedId]
public partial struct FooId { }

var id = new FooId(Guid.NewGuid());
```

You can choose a different type backing field, by passing a value of the `StronglyTypedIdBackingType` enum in the constructor. 

```csharp
using StronglyTypedIds;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.String)]
public partial struct FooId { }

var id = new FooId("my-id-value");
```
Currently supported values are `Guid` (the default), `int`, `long`, and `string`.

## Error CS0436 and [InternalsVisibleTo]

The StronglyTypedId generator automatically adds the `[StronglyTypedId]` attributes to your compilation as `internal` attributes. If you add the source generator package to multiple projects, and use the `[InternalsVisibleTo]` attribute, you may experience errors when you build:

```bash
warning CS0436: The type 'StronglyTypedIdImplementations' in 'StronglyTypedIds\StronglyTypedIds.StronglyTypedIdGenerator\StronglyTypedIdImplementations.cs' conflicts with the imported type 'StronglyTypedIdImplementations' in 'MyProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'.
```

Removing the `[InternalsVisibleTo]` attribute will resolve the problem, but if this is not possible you can disable the auto-generation of the `[StronglyTypedId]` marker attributes, and rely on the included _StronglyTypedId.Attributes.dll_ library instead. This package contains the same attributes, but they are defined in an external library (included in the same _StronglyTypedId_ NuGet package, so it avoids the CS0436 error.

To use this approach you must do two things:

1. Define the MSBuild constant `STRONGLY_TYPED_ID_EXCLUDE_ATTRIBUTES`. This ensures the attributes are not embedded in your project
2. Add `compile` to the list of included assets in your `<PackageReference>` element. This ensures the attributes can still be referenced in your project by using the _StronglyTypedId.Attributes.dll_ library.

For example, the following project file shows the same project file as before, updated to use the attributes library:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <!--  Define the MSBuild constant    -->
    <DefineConstants>STRONGLY_TYPED_ID_EXCLUDE_ATTRIBUTES</DefineConstants>
  </PropertyGroup>

  <!-- Add the package -->
  <PackageReference Include="StronglyTypedId" Version="1.0.0-beta05">
    <!--            👇      Add compile to the list of included assets. You can also remove "runtime"-->
    <IncludeAssets>compile; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
  <!-- -->

</Project>
```

## Why do I need this library?

I have [written a blog-post series](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/) on strongly-typed IDs that explains the issues and rational behind this library. For a detailed view, I suggest starting there, but I provide a brief introduction here.

This library is designed to tackle a specific instance of [_primitive obsession_](https://lostechies.com/jimmybogard/2007/12/03/dealing-with-primitive-obsession/), whereby we use primitive objects (`Guid`/`string`/`int`/`long` etc) to represent the IDs of domain objects. The problem is that these IDs are all interchangeable - an order ID can be assigned to a product ID, despite the fact that is likely nonsensical from the domain point of view. [See here for a more concrete example](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/#an-example-of-the-problem).

By using strongly-typed IDs, we give each ID its own `Type` which _wraps_ the underlying primitive value. This ensures you can only use the ID where it makes sense: `ProductId`s can only be assigned to products, or you can only search for products using a `ProductId`, not an `OrderId`.

Unfortunately, taking this approach requires [a lot of boilerplate and ceremony](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#a-full-example-implementation) to make working with the IDs manageable. This library abstracts all that away from you, by generating the boilerplate at build-time by using a Roslyn-powered code generator.

## What code is generated?

The exact code generated depends on the arguments you provide to the `StronglyTypedId` attribute. The code is generated to the _obj_ folder of the project, so you can use _Go to Definition_ on your Id to see the _exact_ code generated in each case. 

You can see see example implementations in the test `SourceGenerationHelperSnapshotTests` in which all permutations of the attribute are tested, and examples generated in [the snapshots folder](/test/StronglyTypedIds.Tests/Snapshots).

## Requirements

The StronglyTypedId NuGet package is a .NET Standard 2.0 package. 

You must be using the .NET 6+ SDK (though you can compile for other target frameworks like .NET Core 2.1 and .NET Framework 4.8)

The `struct`s you decorate with the `StronglyTypedId` attribute must be marked `partial`.

## Credits
[Credits]: #credits

`StronglyTypedId` wouldn't work if not for [AArnott's CodeGeneration.Roslyn](https://github.com/AArnott/CodeGeneration.Roslyn) library.

The build process and general design of the library was modelled on the [RecordGenerator](https://github.com/amis92/RecordGenerator/blob/master/README.md) project, which is similar to this project, but can be used to generate immutable Record types.