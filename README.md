# StronglyTypedId

![StronglyTypedId logo](https://raw.githubusercontent.com/andrewlock/StronglyTypedId/master/logo.png)

[![Build status](https://ci.appveyor.com/api/projects/status/jx3xrd33tc6vo1vn/branch/master?svg=true)](https://ci.appveyor.com/project/andrewlock/stronglytypedid/branch/master)
[![NuGet](https://img.shields.io/nuget/v/StronglyTypedId.svg)](https://www.nuget.org/packages/StronglyTypedId/)
[![MyGet CI](https://img.shields.io/myget/andrewlock-ci/v/StronglyTypedId.svg)](http://myget.org/gallery/andrewlock-ci)

StronglyTypedId makes creating strongly-typed IDs as easy as adding an attribute! No more [accidentally passing arguments in the wrong order to methods](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/#an-example-of-the-problem) - StronglyTypedId uses Roslyn-powered build-time code generation to generate [the boilerplate](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#a-full-example-implementation) required to use strongly-typed IDs.

Simply, [install the required packages](#installing) add the `[StronglyTypedId]` attribute to a `struct`:

```csharp
[StronglyTypedId] // <- Add this attribute to auto-generate the rest of the type
public partial struct FooId { }
```

and Roslyn magically generates the backing code when you save the file! Use _Go to Definition_ to see the generated code:


<picture>
    <source srcset="https://raw.githubusercontent.com/andrewlock/StronglyTypedId/master/docs/strongly_typed_id.mp4" type="video/mp4">
    <img src="https://raw.githubusercontent.com/andrewlock/StronglyTypedId/master/docs/strongly_typed_id.gif" alt="Generating a strongly-typed ID using the StronglyTypedId packages"/>
</picture>

> StronglyTypedId uses [CodeGeneration.Roslyn](https://github.com/AArnott/CodeGeneration.Roslyn) by [AArnott](https://github.com/AArnott), which requires [the .NET Core SDK v2.1+](https://dotnet.microsoft.com/download/dotnet-core/2.1).

## Installing

To use the the [StronglyTypedId NuGet package](https://www.nuget.org/packages/StronglyTypedId) you must add three packages:

* [StronglyTypedId](https://www.nuget.org/packages/StronglyTypedId)
* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) (optional, only required if [generating a custom `JsonConverter`](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#creating-a-custom-jsonconverter)). Note that in ASP.NET Core apps, you will likely already reference this project via transitive dependencies.
* [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) (optional, only required if [generating a custom `JsonConverter`](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#creating-a-custom-jsonconverter)). Note that in .NET Core apps, you will likely already reference this project via transitive dependencies.

To install the packages, add the references to your _csproj_ file so that it looks something like the following:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
  
  <!-- Add these three packages-->
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
    <PackageReference Include="StronglyTypedId" Version="0.2.0" />
  </ItemGroup>
  <!-- -->

</Project>
```

Restore the tools using `dotnet restore`. 

> Note that StronglyTypedId and CodeGeneration.Roslyn.Tool are **build time** dependencies - no extra dll's are added to your project's output! It's as though you wrote standard C# code yourself!

## Usage

To create a strongly-typed ID, create a `partial struct` with the desired name, and decorate it with the `[StronglyTypedId]` attribute, in the global namespace:

```csharp
[StronglyTypedId] // Add this attribute to auto-generate the rest of the type
public partial struct FooId { }
```

This generates the "default" strongly-typed ID using a `Guid` backing field, a custom `TypeConverter`, and a custom `JsonConverter` based on Newtonsoft.Json. 


### Removing the Newtonsoft.Json dependency

If you don't want to generate custom `JsonConverter`s, set `generateJsonConverter = false` in the attribute constructor:

```csharp
[StronglyTypedId(generateJsonConverter: false)] 
public partial struct NoJsonConverterId { }
```

If you don't generate a `JsonConverter`, you don't need the Newtonsoft.Json package dependency, and can remove it from your _.csproj_.

### Generating a System.Text.Json `JsonConverter`

If you wish to generate a _System.Text.Json_ `JsonConverter`s, set `jsonConverter = StronglyTypedIdConverter.SystemTextJson` in the attribute constructor:

```csharp
[StronglyTypedId(jsonConverter: StronglyTypedIdConverter.SystemTextJson)] 
public partial struct SystemTextJsonConverterId { }
```

If you wish, you can generate both a _System.Text.Json_ and _Newtonsoft.Json_ converter:

```csharp
[StronglyTypedId(jsonConverter: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson)] 
public partial struct BothJsonConverterId { }
```

### Using different types as a backing fields

The default strongly-typed ID uses a `Guid` backing field:

```csharp
[StronglyTypedId]
public partial struct FooId { }

var id = new FooId(Guid.NewGuid());
```

You can choose a different type backing field, by passing a value of the `StronglyTypedIdBackingType` enum in the constructor. 

```csharp
[StronglyTypedId(backingType: StronglyTypedIdBackingType.String)]
public partial struct FooId { }

var id = new FooId("my-id-value");
```
Currently supported values are `Guid` (the default), `int`, `long`, and `string`.


## Why do I need this library?

I have [written a blog-post series](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/) on strongly-typed IDs that explains the issues and rational behind this library. For a detailed view, I suggest starting there, but I provide a brief introduction here.

This library is designed to tackle a specific instance of [_primitive obsession_](https://lostechies.com/jimmybogard/2007/12/03/dealing-with-primitive-obsession/), whereby we use primitive objects (`Guid`/`string`/`int`/`long` etc) to represent the IDs of domain objects. The problem is that these IDs are all interchangeable - an order ID can be assigned to a product ID, despite the fact that is likely nonsensical from the domain point of view. [See here for a more concrete example](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/#an-example-of-the-problem).

By using strongly-typed IDs, we give each ID its own `Type` which _wraps_ the underlying primitive value. This ensures you can only use the ID where it makes sense: `ProductId`s can only be assigned to products, or you can only search for products using a `ProductId`, not an `OrderId`.

Unfortunately, taking this approach requires [a lot of boilerplate and ceremony](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/#a-full-example-implementation) to make working with the IDs manageable. This library abstracts all that away from you, by generating the boilerplate at build-time by using a Roslyn-powered code generator.

## What code is generated?

The exact code generated depends on the arguments you provide to the `StronglyTypedId` attribute. The code is generated to the _obj_ folder of the project, so you can use _Go to Definition_ on your Id to see the _exact_ code generated in each case. 

You can see see example implementations in [the templates folder](/src/StronglyTypedId.Generator/templates):

* [`Guid` StronglyTypedId](/src/StronglyTypedId.Generator/templates/GuidId.cs)
* [`int` StronglyTypedId](/src/StronglyTypedId.Generator/templates/IntId.cs)
* [`long` StronglyTypedId](/src/StronglyTypedId.Generator/templates/LongId.cs)
* [`String` StronglyTypedId](/src/StronglyTypedId.Generator/templates/StringId.cs)

## Requirements

The StronglyTypedId NuGet package is a .NET Standard 2.0 package. 

The code generation tool (`CodeGeneration.Roslyn.Tool`) is also required. These kind of tools are only supported in SDK-format _csproj_ projects

The `struct`s you decorate with the `StronglyTypedId` attribute must be marked `partial`.

## Credits
[Credits]: #credits

`StronglyTypedId` wouldn't work if not for [AArnott's CodeGeneration.Roslyn](https://github.com/AArnott/CodeGeneration.Roslyn) library.

The build process and general design of the library was modelled on the [RecordGenerator](https://github.com/amis92/RecordGenerator/blob/master/README.md) project, which is similar to this project, but can be used to generate immutable Record types.