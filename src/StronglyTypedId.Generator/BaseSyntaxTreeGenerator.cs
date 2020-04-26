using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace StronglyTypedId.Generator
{
    public abstract class BaseSyntaxTreeGenerator
    {
        // Generated using https://roslynquoter.azurewebsites.net/
        public SyntaxList<MemberDeclarationSyntax> CreateStronglyTypedIdSyntax(StructDeclarationSyntax original, bool generateJsonConverter, StronglyTypedIdJsonConverter jsonProvider)
        {
            // Get the name of the decorated member
            var idName = original.Identifier.ValueText;
            var typeConverterName = idName + "TypeConverter";

            var attributes = new List<AttributeListSyntax>() { GetTypeConverterAttribute(typeConverterName) };
            var members = GetMembers(idName).Append(GetTypeConverter(typeConverterName, idName));

            if (generateJsonConverter)
            {
                if (jsonProvider.HasFlag(StronglyTypedIdJsonConverter.NewtonsoftJson))
                {
                    var jsonConverterName = idName + "NewtonsoftJsonConverter";
                    attributes.Add(GetNewtonsoftJsonConverterAttribute(jsonConverterName));
                    members = members.Append(GetNewtonsoftJsonConverter(jsonConverterName, idName));
                }

                if (jsonProvider.HasFlag(StronglyTypedIdJsonConverter.SystemTextJson))
                {
                    var jsonConverterName = idName + "SystemTextJsonConverter";
                    attributes.Add(GetSystemTextJsonConverterAttribute(jsonConverterName));
                    members = members.Append(GetSystemTextJsonConverter(jsonConverterName, idName));
                }
            }

            return SingletonList<MemberDeclarationSyntax>(
                StructDeclaration(idName)
                    .WithAttributeLists(List<AttributeListSyntax>(attributes))
                    .WithModifiers(
                        TokenList(
                            new[]
                            {
                                Token(SyntaxKind.ReadOnlyKeyword),
                                Token(SyntaxKind.PartialKeyword)
                            }))
                    .WithBaseList(
                        BaseList(
                            SeparatedList<BaseTypeSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    SimpleBaseType(
                                        QualifiedName(
                                            IdentifierName("System"),
                                            GenericName(
                                                    Identifier("IComparable"))
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SingletonSeparatedList<TypeSyntax>(
                                                            IdentifierName(idName)))))),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(
                                        QualifiedName(
                                            IdentifierName("System"),
                                            GenericName(
                                                    Identifier("IEquatable"))
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SingletonSeparatedList<TypeSyntax>(
                                                            IdentifierName(idName))))))
                                })))
                    .WithMembers(List<MemberDeclarationSyntax>(members))
                    .WithCloseBraceToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.CloseBraceToken,
                            TriviaList())));
        }

        protected abstract IEnumerable<MemberDeclarationSyntax> GetMembers(string idName);

        #region Type Converter

        protected abstract ClassDeclarationSyntax GetTypeConverter(string typeConverterName, string idName);

        protected AttributeListSyntax GetTypeConverterAttribute(string typeConverterName)
        {
            return AttributeList(
                SingletonSeparatedList<AttributeSyntax>(
                    Attribute(
                        QualifiedName(
                            QualifiedName(
                                IdentifierName("System"),
                                IdentifierName("ComponentModel")),
                            IdentifierName("TypeConverter")))
                    .WithArgumentList(
                        AttributeArgumentList(
                            SingletonSeparatedList<AttributeArgumentSyntax>(
                                AttributeArgument(
                                    TypeOfExpression(
                                        IdentifierName(typeConverterName))))))));
        }

        #endregion


        #region Json Converters

        protected ClassDeclarationSyntax GetJsonConverter(string jsonConverterName, string idName, StronglyTypedIdJsonConverter jsonProvider)
        {
            switch (jsonProvider)
            {
                case StronglyTypedIdJsonConverter.SystemTextJson:
                    return GetSystemTextJsonConverter(jsonConverterName, idName);
                case StronglyTypedIdJsonConverter.NewtonsoftJson:
                default:
                    return GetNewtonsoftJsonConverter(jsonConverterName, idName);
            }
        }

        protected abstract ClassDeclarationSyntax GetNewtonsoftJsonConverter(string jsonConverterName, string idName);

        protected abstract ClassDeclarationSyntax GetSystemTextJsonConverter(string jsonConverterName, string idName);

        protected AttributeListSyntax GetJsonConverterAttribute(string jsonConverterName, StronglyTypedIdJsonConverter jsonProvider)
        {
            switch (jsonProvider)
            {
                case StronglyTypedIdJsonConverter.SystemTextJson:
                    return GetSystemTextJsonConverterAttribute(jsonConverterName);
                case StronglyTypedIdJsonConverter.NewtonsoftJson:
                default:
                    return GetNewtonsoftJsonConverterAttribute(jsonConverterName);
            }
        }

        protected AttributeListSyntax GetNewtonsoftJsonConverterAttribute(string jsonConverterName)
        {
            return AttributeList(
                SingletonSeparatedList<AttributeSyntax>(
                    Attribute(
                        QualifiedName(
                            QualifiedName(
                                IdentifierName("Newtonsoft"),
                                IdentifierName("Json")),
                            IdentifierName("JsonConverter")))
                    .WithArgumentList(
                        AttributeArgumentList(
                            SingletonSeparatedList<AttributeArgumentSyntax>(
                                AttributeArgument(
                                    TypeOfExpression(
                                        IdentifierName(jsonConverterName))))))));
        }

        protected AttributeListSyntax GetSystemTextJsonConverterAttribute(string jsonConverterName)
        {
            return AttributeList(
                SingletonSeparatedList<AttributeSyntax>(
                    Attribute(
                        QualifiedName(
                            QualifiedName(
                                QualifiedName(
                                    QualifiedName(
                                        IdentifierName("System"),
                                        IdentifierName("Text")),
                                    IdentifierName("Json")),
                                IdentifierName("Serialization")),
                            IdentifierName("JsonConverter")))
                    .WithArgumentList(
                        AttributeArgumentList(
                            SingletonSeparatedList<AttributeArgumentSyntax>(
                                AttributeArgument(
                                    TypeOfExpression(
                                        IdentifierName(jsonConverterName))))))));
        }

        #endregion
    }
}