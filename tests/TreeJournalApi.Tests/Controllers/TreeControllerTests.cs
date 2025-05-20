using System.Text.Json;
using TreeJournalApi.Models;
using TreeJournalApi.Tests.Common;

namespace TreeJournalApi.Tests.Controllers
{
    [Collection("CustomWebApplicationFactory collection")]
    public class TreeControllerTests : DatabaseTestBase
    {
        private readonly HttpClient _client;

        public TreeControllerTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTree_CreatesTreeIfNotExists()
        {
            var response = await _client.PostAsync("/api.user.tree/get?treeName=MyTree", null);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var tree = JsonSerializer.Deserialize<TreeNode>(responseBody, options);

            Assert.NotNull(tree);
            Assert.Equal("MyTree", tree.TreeName);
            Assert.Null(tree.ParentId);
        }

        [Fact]
        public async Task CreateNode_AddsChildNode()
        {
            var parent = new TreeNode { Id = 1, Name = "Root", TreeName = "TestTree" };
            DbContext.TreeNodes.Add(parent);
            await DbContext.SaveChangesAsync();

            var response = await _client.PostAsync("/api.user.tree/node.create?treeName=TestTree&parentNodeId=1&nodeName=ChildNode", null);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync(); 
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var newNode = JsonSerializer.Deserialize<TreeNode>(responseBody, options);


            Assert.NotNull(newNode);
            Assert.Equal("ChildNode", newNode.Name);
            Assert.Equal(1, newNode.ParentId);
        }

        [Fact]
        public async Task DeleteNode_RemovesNode()
        {
            var node = new TreeNode { Id = 5, Name = "ToDelete", TreeName = "TestTree" };
            DbContext.TreeNodes.Add(node);
            await DbContext.SaveChangesAsync();

            var response = await _client.PostAsync("/api.user.tree/node.delete?treeName=TestTree&nodeId=5", null);
            response.EnsureSuccessStatusCode();

            await DbContext.Entry(node).ReloadAsync();
            var deletedNode = await DbContext.TreeNodes.FindAsync(5L);
            Assert.Null(deletedNode);
        }

        [Fact]
        public async Task RenameNode_ChangesNodeName()
        {
            var node = new TreeNode { Id = 3, Name = "OldName", TreeName = "TestTree" };
            DbContext.TreeNodes.Add(node);
            await DbContext.SaveChangesAsync();

            var response = await _client.PostAsync("/api.user.tree/node.rename?treeName=TestTree&nodeId=3&newNodeName=NewName", null);
            response.EnsureSuccessStatusCode();

            var updatedNode = await DbContext.TreeNodes.FindAsync(3L) ?? throw new InvalidOperationException("Node not found");
            await DbContext.Entry(updatedNode).ReloadAsync();
            Assert.NotNull(updatedNode);
            Assert.Equal("NewName", updatedNode.Name);
        }
    }
}
