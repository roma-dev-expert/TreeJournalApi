using TreeJournalApi.Models;
using System.Text.Json;

namespace TreeJournalApi.Tests.Models
{
    public class ExceptionJournalTests
    {
        [Fact]
        public void ExceptionJournal_CreatesCorrectObject()
        {
            long expectedId = 1;
            long expectedEventId = 100;
            DateTime expectedCreatedAt = DateTime.UtcNow;
            string expectedParams = "param1=value1";
            string expectedStackTrace = "Exception at line 42";

            var journalEntry = new ExceptionJournal
            {
                Id = expectedId,
                EventId = expectedEventId,
                CreatedAt = expectedCreatedAt,
                RequestParameters = expectedParams,
                StackTrace = expectedStackTrace
            };

            Assert.Equal(expectedId, journalEntry.Id);
            Assert.Equal(expectedEventId, journalEntry.EventId);
            Assert.Equal(expectedCreatedAt, journalEntry.CreatedAt, TimeSpan.FromSeconds(1));
            Assert.Equal(expectedParams, journalEntry.RequestParameters);
            Assert.Equal(expectedStackTrace, journalEntry.StackTrace);
        }

        [Fact]
        public void ExceptionJournal_SerializesCorrectly()
        {
            var journalEntry = new ExceptionJournal
            {
                Id = 2,
                EventId = 200,
                CreatedAt = DateTime.UtcNow,
                RequestParameters = "test=value",
                StackTrace = "Some error trace"
            };

            string json = JsonSerializer.Serialize(journalEntry);

            Assert.Contains("\"Id\":2", json);
            Assert.Contains("\"EventId\":200", json);
            Assert.Contains("\"RequestParameters\":\"test=value\"", json);
            Assert.Contains("\"StackTrace\":\"Some error trace\"", json);
        }
    }
}
