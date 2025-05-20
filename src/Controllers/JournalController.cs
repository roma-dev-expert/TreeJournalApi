using Microsoft.AspNetCore.Mvc;
using TreeJournalApi.Data;
using TreeJournalApi.Models;

namespace TreeJournalApi.Controllers
{
    [ApiController]
    [Route("api.user.journal")]
    public class JournalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JournalController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("log")]
        public IActionResult LogException([FromBody] ExceptionJournal exception)
        {
            exception.CreatedAt = DateTime.UtcNow;
            _context.ExceptionJournals.Add(exception);
            _context.SaveChanges();
            return Ok(new { Message = "Exception logged successfully", Id = exception.Id });
        }

        [HttpGet("logs")]
        public IActionResult GetLogs()
        {
            var logs = _context.ExceptionJournals.OrderByDescending(e => e.CreatedAt).ToList();
            return Ok(logs);
        }

        [HttpPost("getRange")]
        public IActionResult GetRange([FromQuery] int skip, [FromQuery] int take, [FromBody] JournalFilter filter)
        {
            var query = _context.ExceptionJournals.AsQueryable();
            if (filter.From != null)
                query = query.Where(e => e.CreatedAt >= filter.From.Value);
            if (filter.To != null)
                query = query.Where(e => e.CreatedAt <= filter.To.Value);
            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(e => e.RequestParameters.Contains(filter.Search));

            var total = query.Count();
            var items = query
                .OrderByDescending(e => e.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToList();

            return Ok(new { skip, count = total, items });
        }

        [HttpPost("getSingle")]
        public IActionResult GetSingle([FromQuery] long id)
        {
            var entry = _context.ExceptionJournals.FirstOrDefault(e => e.Id == id);
            if (entry == null)
                return NotFound();
            var response = new
            {
                text = $"Request: {entry.RequestParameters} | Stack: {entry.StackTrace}",
                id = entry.Id,
                eventId = entry.EventId,
                createdAt = entry.CreatedAt
            };
            return Ok(response);
        }
    }

    public class JournalFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Search { get; set; }
    }
}
