using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;

[AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
[CodeGenerationAttribute("StronglyTypedId.Generator.StronglyTypedIdGenerator, StronglyTypedId.Generator")]
[Conditional("CodeGeneration")]
public class StronglyTypedIdAttribute : Attribute { }
