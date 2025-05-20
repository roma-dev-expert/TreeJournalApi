using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TreeJournalApi.Data;
using TreeJournalApi.Models;

namespace TreeJournalApi.Tests.Data
{
    public class AppDbContextTests
    {
        private SqliteConnection GetOpenConnection()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        }

        private DbContextOptions<AppDbContext> GetSQLiteOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(GetOpenConnection())
                .Options;
        }

        [Fact]
        public void CanCreateDbContext()
        {
            var options = GetSQLiteOptions();

            using (var context = new AppDbContext(options))
            {
                Assert.NotNull(context);
                context.Database.EnsureCreated();
            }
        }

        [Fact]
        public void CanSaveAndRetrieveTreeNode()
        {
            var options = GetSQLiteOptions();

            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();
                var node = new TreeNode { Id = 1L, Name = "Root", TreeName = "TestTree" };
                context.TreeNodes.Add(node);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var retrievedNode = context.TreeNodes.Find(1L);
                Assert.NotNull(retrievedNode);
                Assert.Equal("Root", retrievedNode.Name);
                Assert.Equal("TestTree", retrievedNode.TreeName);
            }
        }

        [Fact]
        public void ParentChildRelationshipWorks()
        {
            var options = GetSQLiteOptions();

            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();

                var parentNode = new TreeNode { Id = 1, Name = "Parent", TreeName = "Tree1" };
                var childNode = new TreeNode { Id = 2, Name = "Child", TreeName = "Tree1", Parent = parentNode };

                context.TreeNodes.AddRange(parentNode, childNode);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var parent = context.TreeNodes.Include(t => t.Children).FirstOrDefault(t => t.Id == 1);
                Assert.NotNull(parent);
                Assert.Single(parent.Children);
                Assert.Equal("Child", parent.Children.First().Name);
            }
        }
    }
}
