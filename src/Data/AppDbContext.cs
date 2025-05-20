using Microsoft.EntityFrameworkCore;
using TreeJournalApi.Models;

namespace TreeJournalApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TreeNode> TreeNodes { get; set; }
        public DbSet<ExceptionJournal> ExceptionJournals { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreeNode>()
                .HasIndex(t => new { t.TreeName, t.Name })
                .IsUnique();

            modelBuilder.Entity<TreeNode>()
                .HasOne(t => t.Parent)
                .WithMany(t => t.Children)
                .HasForeignKey(t => t.ParentId)
                .OnDelete(DeleteBehavior.Cascade); 

            base.OnModelCreating(modelBuilder);
        }

    }

}
