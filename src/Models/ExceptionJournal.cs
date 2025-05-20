namespace TreeJournalApi.Models
{
    public class ExceptionJournal
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RequestParameters { get; set; }
        public string? StackTrace { get; set; }
    }

}
