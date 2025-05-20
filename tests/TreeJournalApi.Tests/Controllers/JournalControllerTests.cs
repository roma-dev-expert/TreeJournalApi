using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TreeJournalApi.Models;
using TreeJournalApi.Tests.Common;

namespace TreeJournalApi.Tests.Controllers
{
    [Collection("CustomWebApplicationFactory collection")]
    public class JournalControllerTests : DatabaseTestBase
    {
        private readonly HttpClient _client;

        public JournalControllerTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task LogException_CreatesEntry()
        {
            var exception = new ExceptionJournal
            {
                EventId = 1001,
                RequestParameters = "param=test",
                StackTrace = "Error at line 42"
            };

            var content = new StringContent(JsonSerializer.Serialize(exception), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api.user.journal/log", content);
            response.EnsureSuccessStatusCode();

            var loggedEntry = await DbContext.ExceptionJournals.FirstOrDefaultAsync(e => e.EventId == 1001);
            Assert.NotNull(loggedEntry);
            Assert.Equal("param=test", loggedEntry.RequestParameters);
        }

        [Fact]
        public async Task GetLogs_ReturnsLogs()
        {
            DbContext.ExceptionJournals.Add(new ExceptionJournal { EventId = 1002, RequestParameters = "log1" });
            DbContext.ExceptionJournals.Add(new ExceptionJournal { EventId = 1003, RequestParameters = "log2" });
            await DbContext.SaveChangesAsync();

            var response = await _client.GetAsync("/api.user.journal/logs");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var logs = JsonSerializer.Deserialize<List<ExceptionJournal>>(responseBody);

            Assert.NotEmpty(logs);
            Assert.Equal(2, logs.Count);
        }

        [Fact]
        public async Task GetRange_ReturnsFilteredResults()
        {
            DbContext.ExceptionJournals.AddRange(
                new ExceptionJournal { Id = 1, EventId = 1001, CreatedAt = DateTime.UtcNow.AddDays(-5), RequestParameters = "error1" },
                new ExceptionJournal { Id = 2, EventId = 1002, CreatedAt = DateTime.UtcNow.AddDays(-3), RequestParameters = "critical issue" },
                new ExceptionJournal { Id = 3, EventId = 1003, CreatedAt = DateTime.UtcNow.AddDays(-1), RequestParameters = "info log" }
            );
            await DbContext.SaveChangesAsync();

            var filter = new
            {
                From = DateTime.UtcNow.AddDays(-4),
                To = DateTime.UtcNow.AddDays(0),
                Search = "critical"
            };
            var content = new StringContent(JsonSerializer.Serialize(filter), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api.user.journal/getRange?skip=0&take=5", content);
            response.EnsureSuccessStatusCode();

            using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
            string responseBody = await reader.ReadToEndAsync();
            var json = JsonDocument.Parse(responseBody).RootElement;

            Assert.Equal(1, json.GetProperty("count").GetInt32());
            Assert.Single(json.GetProperty("items").EnumerateArray());
            Assert.Contains("critical issue", json.GetProperty("items")[0].GetProperty("requestParameters").GetString());
        }

        [Fact]
        public async Task GetSingle_ReturnsCorrectEntry()
        {
            var entry = new ExceptionJournal { Id = 123, EventId = 3003, RequestParameters = "singleLog" };
            DbContext.ExceptionJournals.Add(entry);
            await DbContext.SaveChangesAsync();

            var response = await _client.PostAsync($"/api.user.journal/getSingle?id={entry.Id}", null);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(responseBody);

            Assert.Equal(entry.Id, json.GetProperty("id").GetInt64());
            Assert.Equal(entry.EventId, json.GetProperty("eventId").GetInt64());
            Assert.Contains("singleLog", json.GetProperty("text").GetString());
        }
    }
}
