using TreeJournalApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TreeJournalApi.Tests.Models
{
    public class TreeNodeTests
    {
        [Fact]
        public void TreeNode_CreatesCorrectObject()
        {
            long expectedId = 1;
            string expectedName = "Root";
            string expectedTreeName = "MyTree";

            var node = new TreeNode
            {
                Id = expectedId,
                Name = expectedName,
                TreeName = expectedTreeName
            };

            Assert.Equal(expectedId, node.Id);
            Assert.Equal(expectedName, node.Name);
            Assert.Equal(expectedTreeName, node.TreeName);
        }

        [Fact]
        public void TreeNode_SetsParentAndChildrenCorrectly()
        {
            var parentNode = new TreeNode { Id = 1, Name = "Parent" };
            var childNode1 = new TreeNode { Id = 2, Name = "Child 1", Parent = parentNode };
            var childNode2 = new TreeNode { Id = 3, Name = "Child 2", Parent = parentNode };

            parentNode.Children.Add(childNode1);
            parentNode.Children.Add(childNode2);

            Assert.Equal(parentNode, childNode1.Parent);
            Assert.Equal(parentNode, childNode2.Parent);
            Assert.Contains(childNode1, parentNode.Children);
            Assert.Contains(childNode2, parentNode.Children);
        }

        [Fact]
        public void TreeNode_SerializesCorrectly()
        {
            var parentNode = new TreeNode { Id = 1, Name = "Root" };
            var childNode = new TreeNode { Id = 2, Name = "Child", Parent = parentNode };
            parentNode.Children.Add(childNode);

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(parentNode, options);
            var parsedJson = JsonDocument.Parse(json).RootElement;

            Assert.Equal(1, parsedJson.GetProperty("Id").GetInt64());
            Assert.Equal("Root", parsedJson.GetProperty("Name").GetString());
            Assert.True(parsedJson.TryGetProperty("Children", out _), "Children key is missing");
        }
    }
}
