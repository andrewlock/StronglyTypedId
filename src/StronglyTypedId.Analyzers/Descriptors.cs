using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static StronglyTypedId.Analyzers.Category;

namespace StronglyTypedId.Analyzers
{
    enum Category
    {
        Usage
    }

    static class Descriptors
    {
        const string IdPrefix = "StrongIdGen";
        const string HelpUriBase = "https://github.com/andrewlock/StronglyTypedId";

        static DiagnosticDescriptor Rule(
            int id, string title, Category category, DiagnosticSeverity defaultSeverity,
            string messageFormat, string description = null)
        {
            var isEnabledByDefault = true;
            var helpLinkUri = HelpUriBase + id;
            return new DiagnosticDescriptor(
                IdPrefix + id, title, messageFormat, category.ToString(), defaultSeverity, isEnabledByDefault, description, helpLinkUri);
        }

        public static DiagnosticDescriptor X1000_StronglyTypedIdMustBePartial { get; } =
            Rule(1000, "StronglyTypedId must be partial", Usage, Error, "Add partial modifier to type declaration");
    }
}
