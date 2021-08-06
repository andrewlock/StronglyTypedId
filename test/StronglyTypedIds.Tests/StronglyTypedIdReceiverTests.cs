using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace StronglyTypedIds.Tests
{
    public class StronglyTypedIdReceiverTests
    {
        [Theory]
        [MemberData(nameof(Data.FindAttributedStructs), MemberType = typeof(Data))]
        public async Task FindsAttributedStructs(string code, int expectedPropertyCounts)
        {
            await AssertFindsExpectedAttributes(code, expectedPropertyCounts, x => x.Targets.Count);
        }

        [Theory]
        [MemberData(nameof(Data.FindAssemblyAttributes), MemberType = typeof(Data))]
        public async Task FindsAssemblyAttributes(string code, int expectedPropertyCounts)
        {
            await AssertFindsExpectedAttributes(code, expectedPropertyCounts, x => x.Defaults.Count);
        }

        private static async Task AssertFindsExpectedAttributes(string code, int expectedPropertyCounts, Func<StronglyTypedIdReceiver, int> getExpectedPropertyCounts)
        {
            var rootSyntaxNode = await SyntaxFactory.ParseSyntaxTree(code)
                .GetRootAsync()
                .ConfigureAwait(false);

            var receiver = new StronglyTypedIdReceiver();
            foreach (var node in rootSyntaxNode.DescendantNodes(descendIntoChildren: _ => true))
            {
                receiver.OnVisitSyntaxNode(node);
            }

            Assert.Equal(expectedPropertyCounts, getExpectedPropertyCounts(receiver));
        }

        public static class Data
        {
            public static TheoryData<string, int> FindAttributedStructs { get; } = new()
            {
                { @" public partial struct TestId {}", 0 },
                { @" [StronglyTypedId] public partial struct TestId { }", 1},
                { @" [StronglyTypedIds.StronglyTypedId] public partial struct TestId { }", 1},
                { @" [StronglyTypedIdAttribute] public partial struct TestId { }", 1},
                { @" [StronglyTypedIds.StronglyTypedIdAttribute] public partial struct TestId { }", 1},
                { @" [StronglyTypedId(converters: StronglyTypedIdConverter.None)] public partial struct TestId { }", 1 },
                { @" [StronglyTypedId] public partial struct TestId1 { } [StronglyTypedIds.StronglyTypedId] public partial struct TestId1 { }", 2 },
            };

            public static TheoryData<string, int> FindAssemblyAttributes { get; } = new()
            {
                { @" public partial struct TestId {}", 0 },
                { @" [StronglyTypedIdDefaults] public partial struct TestId { }", 0},
                { @" [assembly:StronglyTypedIdDefaults] ", 1},
                { @" [assembly:StronglyTypedIds.StronglyTypedIdDefaults] ", 1},
                { @" [assembly:StronglyTypedIdDefaultsAttribute] ", 1},
                { @" [assembly:StronglyTypedIds.StronglyTypedIdDefaultsAttribute] ", 1},
                { @" [assembly:StronglyTypedIdDefaults(converters: StronglyTypedIdConverter.None)] ", 1},
                { @" [assembly:StronglyTypedIdDefaults] [assembly:StronglyTypedIds.StronglyTypedIdDefaults]", 2},
            };
        }
    }
}