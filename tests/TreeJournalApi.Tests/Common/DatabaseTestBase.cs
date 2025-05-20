using Microsoft.Extensions.DependencyInjection;
using TreeJournalApi.Data;


namespace TreeJournalApi.Tests.Common
{
    public abstract class DatabaseTestBase : IAsyncLifetime
    {
        protected readonly CustomWebApplicationFactory<Program> Factory;
        protected readonly AppDbContext DbContext;

        public DatabaseTestBase(CustomWebApplicationFactory<Program> factory)
        {
            Factory = factory;

            var scope = Factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        public async Task InitializeAsync()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();
            await Task.CompletedTask;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}