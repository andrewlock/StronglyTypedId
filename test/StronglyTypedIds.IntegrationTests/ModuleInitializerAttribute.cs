#if !NET5_0
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    // This is a C#9 feature, but requires this attribute to be defined
    // Only .NET 5 defines it though, it only exists in .NET 5
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class ModuleInitializerAttribute : Attribute
    {
    }
}
#endif