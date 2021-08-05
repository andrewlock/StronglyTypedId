# Changelog

## [v1.0.0-beta01]

Features: 

* Convert package to use source generators instead of CodeGeneration.Roslyn

Breaking Changes
* `[StronglyTypedId]` attribute now exists in the `StronglyTypedIds` namespace, so you must add a `using` statement to your app
* Requires .NET Core 5 SDK+ 

## [v0.2.1]

Features:

* Fix Package description

## [v0.2.0]

Features:

* Added support for .NET Core 3.1, and converted to using CodeGeneration.Roslyn.Tool instead of dotnet-codegen (thanks [Bartłomiej Oryszak
](https://github.com/vebbo2))
* Added support for generating System.Text.Json `JsonConverters` (thanks [Bartłomiej Oryszak
](https://github.com/vebbo2))
* Added support for long backing type (thanks [Bartłomiej Oryszak
](https://github.com/vebbo2))

## [v0.1.0]

Initial release

[v0.2.0]: https://github.com/andrewlock/StronglyTypedId/compare/v0.1.0...v0.2.0