using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StronglyTypedId.Analyzers.Test")]

namespace StronglyTypedId.Analyzers
{
    public static class Properties
    {
        public const string AnalyzerName = "StronglyTypedId";

        private static string _versionString;

        public static string VersionString => _versionString ?? (_versionString = GetVersionString());

        private static string GetVersionString()
        {
            var assembly = typeof(Properties).GetTypeInfo().Assembly;
            return assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "";
        }
    }
}