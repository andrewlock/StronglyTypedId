using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace StronglyTypedIds.Tests
{
    public class StronglyTypedIdReceiverTests
    {
        [Theory]
        [MemberData(nameof(Data.FindProperties), MemberType = typeof(Data))]
        public async Task FindsAttributedStructs(string code, int expectedPropertyCounts)
        {
            var rootSyntaxNode = await SyntaxFactory.ParseSyntaxTree(code)
                .GetRootAsync()
                .ConfigureAwait(false);

            var receiver = new StronglyTypedIdReceiver();
            foreach (var node in rootSyntaxNode.DescendantNodes(descendIntoChildren: _ => true))
            {
                receiver.OnVisitSyntaxNode(node);
            }

            Assert.Equal(expectedPropertyCounts, receiver.StronglyTypedIdStructs.Count);
        }

        public static class Data
        {
            public static TheoryData<string, int> FindProperties { get; } = new()
            {
                { @" public partial struct TestId {}", 0 },
                { @" [StronglyTypedId] public partial struct TestId { }", 1},
                { @" [StronglyTypedIdAttribute] public partial struct TestId { }", 1},
                { @" [StronglyTypedId(generateJsonConverter: false)] public partial struct TestId { }", 1 },
                { @" [StronglyTypedId] public partial struct TestId1 { } [StronglyTypedId] public partial struct TestId1 { }", 2 },
            };
        }
    }
}